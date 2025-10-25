using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace IdleTycoon.Scripts.Utils
{
    public unsafe struct FastPool<T> : IDisposable where T : unmanaged
    {
        private const int MemoryBlockAlignment = 64;

        private T* _entities;
        private int* _ids;
        private ulong* _activeBits;

        private int _capacity;
        private int _count;
        private int _growthBacking;

        private int _firstWordWithFreeBit;
        private int _firstWordWithActiveBits;
        private int _lastWordWithActiveBits;

        public int Count => _count;
        public int Capacity => _capacity;

        public int Growth
        {
            get => _growthBacking;
            set => _growthBacking = Align64(Math.Max(64, value));
        }

        public FastPool(int initialCapacity = 1024)
        {
            _capacity = Align64(Math.Max(64, initialCapacity));
            _count = 0;

            int byteEntities = _capacity * sizeof(T);
            int byteIds = _capacity * sizeof(int);
            int bitArraySize = _capacity / 64;
            int byteActiveBits = bitArraySize * sizeof(ulong);

            _entities = (T*)UnsafeUtility.Malloc(byteEntities, MemoryBlockAlignment, Allocator.Persistent);
            _ids = (int*)UnsafeUtility.Malloc(byteIds, MemoryBlockAlignment, Allocator.Persistent);
            _activeBits = (ulong*)UnsafeUtility.Malloc(byteActiveBits, MemoryBlockAlignment, Allocator.Persistent);

            UnsafeUtility.MemClear(_entities, byteEntities);
            UnsafeUtility.MemClear(_ids, byteIds);
            UnsafeUtility.MemClear(_activeBits, byteActiveBits);

            _growthBacking = 1024;

            _firstWordWithFreeBit = 0;
            _firstWordWithActiveBits = int.MaxValue;
            _lastWordWithActiveBits = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Align64(int value) => (value + 63) & ~63;

        public void Dispose()
        {
            FreeAligned(_entities);
            FreeAligned(_ids);
            FreeAligned(_activeBits);

            _entities = null;
            _ids = null;
            _activeBits = null;
            _capacity = 0;
            _count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetActive(int index, bool value)
        {
            int wordIndex = index >> 6;
            int bit = index & 63;
            ulong mask = 1UL << bit;

            if (value)
                _activeBits[wordIndex] |= mask;
            else
                _activeBits[wordIndex] &= ~mask;

            if (!value && wordIndex < _firstWordWithFreeBit)
                _firstWordWithFreeBit = wordIndex;
            else if (value && _firstWordWithFreeBit == wordIndex && _activeBits[wordIndex] == ulong.MaxValue)
                _firstWordWithFreeBit++;

            if (value)
            {
                if (wordIndex < _firstWordWithActiveBits) _firstWordWithActiveBits = wordIndex;
                if (wordIndex > _lastWordWithActiveBits) _lastWordWithActiveBits = wordIndex;
            }
            else if (_activeBits[wordIndex] == 0)
            {
                if (wordIndex == _firstWordWithActiveBits) _firstWordWithActiveBits = FindNextActiveWord(wordIndex + 1);
                if (wordIndex == _lastWordWithActiveBits) _lastWordWithActiveBits = FindPrevActiveWord(wordIndex - 1);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsActive(int index)
        {
            int wordIndex = index >> 6;
            int bit = index & 63;

            return (_activeBits[wordIndex] & (1UL << bit)) != 0;
        }

        public int Add(T entity)
        {
            int index = FindFreeSlot();
            if (index < 0)
            {
                Grow(_capacity + Growth);
                index = FindFreeSlot();
                if (index < 0) throw new InvalidOperationException("Failed to find slot after grow");
            }

            _entities[index] = entity;

            int gen = ((_ids[index] >> 16) + 1) & 0xFFFF;
            _ids[index] = (gen << 16) | index;

            SetActive(index, true);
            _count++;

            return _ids[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindFreeSlot()
        {
            int words = _capacity / 64;
            for (int w = _firstWordWithFreeBit; w < words; w++)
            {
                ulong word = _activeBits[w];
                if (word != ulong.MaxValue)
                {
                    ulong notWord = ~word;
                    int bit = BitUtil.TrailingZeroCount(notWord);
                    int idx = (w << 6) + bit;
                    if (idx < _capacity)
                    {
                        if ((word | (1UL << bit)) == ulong.MaxValue)
                            _firstWordWithFreeBit = w + 1;
                        else
                            _firstWordWithFreeBit = w;

                        return idx;
                    }
                }
            }

            return -1;
        }

        public void Grow(int newCapacity)
        {
            if (newCapacity <= _capacity) return;
            newCapacity = Align64(newCapacity);

            int newByteEntities = newCapacity * sizeof(T);
            int newByteIds = newCapacity * sizeof(int);
            int newBitSize = newCapacity / 64;
            int newByteActiveBits = newBitSize * sizeof(ulong);

            T* newEntities = (T*)UnsafeUtility.Malloc(newByteEntities, MemoryBlockAlignment, Allocator.Persistent);
            int* newIds = (int*)UnsafeUtility.Malloc(newByteIds, MemoryBlockAlignment, Allocator.Persistent);
            ulong* newActiveBits =
                (ulong*)UnsafeUtility.Malloc(newByteActiveBits, MemoryBlockAlignment, Allocator.Persistent);

            Buffer.MemoryCopy(_entities, newEntities, newByteEntities, _capacity * sizeof(T));
            Buffer.MemoryCopy(_ids, newIds, newByteIds, _capacity * sizeof(int));
            int oldBitSize = _capacity / 64;
            Buffer.MemoryCopy(_activeBits, newActiveBits, newByteActiveBits, oldBitSize * sizeof(ulong));

            if (newCapacity > _capacity)
            {
                UnsafeUtility.MemClear((byte*)newEntities + _capacity * sizeof(T),
                    (newCapacity - _capacity) * sizeof(T));
                UnsafeUtility.MemClear((byte*)newIds + _capacity * sizeof(int),
                    (newCapacity - _capacity) * sizeof(int));
                UnsafeUtility.MemClear((byte*)newActiveBits + oldBitSize * sizeof(ulong),
                    (newBitSize - oldBitSize) * sizeof(ulong));
            }

            FreeAligned(_entities);
            FreeAligned(_ids);
            FreeAligned(_activeBits);

            _entities = newEntities;
            _ids = newIds;
            _activeBits = newActiveBits;
            _capacity = newCapacity;
        }

        public ref T Get(int entityId)
        {
            int index = entityId & 0xFFFF;
            if (index < 0 || index >= _capacity) throw new InvalidOperationException("Invalid entity index");
            if (!IsActive(index) || _ids[index] != entityId)
                throw new InvalidOperationException("Invalid entity ID");

            return ref _entities[index];
        }

        public ActiveSpanEnumerable AsSpanEnumerable() => new ActiveSpanEnumerable(this);

        public readonly ref struct EntityRef<TValue> where TValue : unmanaged
        {
            public readonly int Id;
            private readonly TValue* _ptr;

            public ref TValue Entity => ref *_ptr;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public EntityRef(int id, TValue* ptr)
            {
                Id = id;
                _ptr = ptr;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out int id, out TValue* ptr)
            {
                id = Id;
                ptr = _ptr;
            }
        }

        public readonly ref struct ActiveSpanEnumerable
        {
            private readonly FastPool<T> _pool;

            public ActiveSpanEnumerable(FastPool<T> pool) => _pool = pool;
            public ActiveSpanEnumerator GetEnumerator() => new ActiveSpanEnumerator(_pool);
        }

        public ref struct ActiveSpanEnumerator
        {
            private readonly FastPool<T> _pool;
            private ulong _bitMask;
            private int _wordIndex;
            private int _index;
            private int _remaining;

            public ActiveSpanEnumerator(FastPool<T> pool)
            {
                _pool = pool;
                _bitMask = 0;
                _wordIndex = pool._firstWordWithActiveBits - 1;
                _index = -1;
                _remaining = pool._count;
            }

            public EntityRef<T> Current => new EntityRef<T>(_pool._ids[_index], &_pool._entities[_index]);

            public bool MoveNext()
            {
                if (_remaining <= 0 || _wordIndex > _pool._lastWordWithActiveBits) return false;

                while (true)
                {
                    if (_bitMask != 0)
                    {
                        int bit = BitUtil.TrailingZeroCount(_bitMask);
                        _bitMask &= ~(1UL << bit);
                        _index = (_wordIndex << 6) + bit;
                        _remaining--;

                        return true;
                    }

                    _wordIndex++;
                    if (_wordIndex > _pool._lastWordWithActiveBits) return false;

                    _bitMask = _pool._activeBits[_wordIndex];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FreeAligned(void* alignedPtr)
        {
            if (alignedPtr == null) return;
            UnsafeUtility.Free(alignedPtr, Allocator.Persistent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindNextActiveWord(int start)
        {
            int words = _capacity / 64;
            for (int w = start; w < words; w++)
                if (_activeBits[w] != 0)
                    return w;

            return int.MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindPrevActiveWord(int start)
        {
            for (int w = start; w >= 0; w--)
                if (_activeBits[w] != 0)
                    return w;

            return -1;
        }
    }
}