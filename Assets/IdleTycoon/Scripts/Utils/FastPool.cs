using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace IdleTycoon.Scripts.Utils
{
    public unsafe struct FastPool<T> : IDisposable where T : unmanaged
    {
        private const int MemoryBlockAlignment = 64;
        private const int PoolCapacityAlignment = 64; //word size
        private const Unity.Collections.Allocator Allocator = Unity.Collections.Allocator.Persistent;

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
            set => _growthBacking = Align64(Math.Max(PoolCapacityAlignment, value));
        }
        
        public FastPool(int initialCapacity = 1024, int growthBacking = 1024)
        {
            _capacity = Align64(Math.Max(PoolCapacityAlignment, initialCapacity));
            _growthBacking = Align64(Math.Max(PoolCapacityAlignment, growthBacking));
            _count = 0;

            int byteEntities = _capacity * sizeof(T);
            int byteIds = _capacity * sizeof(int);
            int bitArraySize = _capacity / PoolCapacityAlignment;
            int byteActiveBits = bitArraySize * sizeof(ulong);

            _entities = (T*)UnsafeUtility.Malloc(byteEntities, MemoryBlockAlignment, Allocator);
            _ids = (int*)UnsafeUtility.Malloc(byteIds, MemoryBlockAlignment, Allocator);
            _activeBits = (ulong*)UnsafeUtility.Malloc(byteActiveBits, MemoryBlockAlignment, Allocator);

            UnsafeUtility.MemClear(_entities, byteEntities);
            UnsafeUtility.MemClear(_ids, byteIds);
            UnsafeUtility.MemClear(_activeBits, byteActiveBits);
            
            _firstWordWithFreeBit = 0;
            _firstWordWithActiveBits = int.MaxValue;
            _lastWordWithActiveBits = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Align64(int value) => (value + 63) & ~63;

        public void Dispose()
        {
            Free(_entities);
            Free(_ids);
            Free(_activeBits);

            _entities = null;
            _ids = null;
            _activeBits = null;
            _capacity = 0;
            _count = 0;
        }
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetAsActive(int index)
        {
            int wordIndex = index >> 6;

            _activeBits[wordIndex] |= 1UL << (index & 63);
            
            if (_firstWordWithFreeBit == wordIndex && _activeBits[wordIndex] == ulong.MaxValue)
                _firstWordWithFreeBit++;

            if (wordIndex < _firstWordWithActiveBits) _firstWordWithActiveBits = wordIndex;
            if (wordIndex > _lastWordWithActiveBits) _lastWordWithActiveBits = wordIndex;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetAsInactive(int index)
        {
            int wordIndex = index >> 6;

            _activeBits[wordIndex] &= ~(1UL << (index & 63));

            if (wordIndex < _firstWordWithFreeBit)
                _firstWordWithFreeBit = wordIndex;

            if (_activeBits[wordIndex] != 0) return;
            
            if (wordIndex == _firstWordWithActiveBits) _firstWordWithActiveBits = FindNextActiveWord(wordIndex + 1);
            if (wordIndex == _lastWordWithActiveBits) _lastWordWithActiveBits = FindPrevActiveWord(wordIndex - 1);
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

            SetAsActive(index);
            _count++;

            return _ids[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindFreeSlot()
        {
            int words = _capacity / PoolCapacityAlignment;
            for (int w = _firstWordWithFreeBit; w < words; w++)
            {
                ulong word = _activeBits[w];
                if (word == ulong.MaxValue) continue;
                
                ulong notWord = ~word;
                int bit = BitUtil.TrailingZeroCount(notWord);
                int freeSlot = (w << 6) + bit;
                _firstWordWithFreeBit = (word | (1UL << bit)) == ulong.MaxValue ? w + 1 : w;
                    
                return freeSlot;
            }

            return -1;
        }

        public void Grow(int newCapacity)
        {
            newCapacity = Align64(newCapacity);
            if (newCapacity <= _capacity) return;

            int newByteEntities = newCapacity * sizeof(T);
            int newByteIds = newCapacity * sizeof(int);
            int newBitSize = newCapacity / PoolCapacityAlignment;
            int newByteActiveBits = newBitSize * sizeof(ulong);

            T* newEntities = (T*)UnsafeUtility.Malloc(newByteEntities, MemoryBlockAlignment, Allocator);
            int* newIds = (int*)UnsafeUtility.Malloc(newByteIds, MemoryBlockAlignment, Allocator);
            ulong* newActiveBits = (ulong*)UnsafeUtility.Malloc(newByteActiveBits, MemoryBlockAlignment, Allocator);

            long copyEntitiesBytes = (long)_capacity * sizeof(T);
            long copyIdsBytes = (long)_capacity * sizeof(int);
            int oldBitSize = _capacity / PoolCapacityAlignment;
            long copyActiveBitsBytes = (long)oldBitSize * sizeof(ulong);

            UnsafeUtility.MemCpy(newEntities, _entities, copyEntitiesBytes);
            UnsafeUtility.MemCpy(newIds, _ids, copyIdsBytes);
            UnsafeUtility.MemCpy(newActiveBits, _activeBits, copyActiveBitsBytes);

            UnsafeUtility.MemClear((byte*)newEntities + _capacity * sizeof(T), (newCapacity - _capacity) * sizeof(T));
            UnsafeUtility.MemClear((byte*)newIds + _capacity * sizeof(int), (newCapacity - _capacity) * sizeof(int));
            UnsafeUtility.MemClear((byte*)newActiveBits + oldBitSize * sizeof(ulong), (newBitSize - oldBitSize) * sizeof(ulong));

            Free(_entities);
            Free(_ids);
            Free(_activeBits);

            _entities = newEntities;
            _ids = newIds;
            _activeBits = newActiveBits;
            _capacity = newCapacity;
            
            _firstWordWithActiveBits = int.MaxValue;
            _lastWordWithActiveBits = -1;
            int words = _capacity / PoolCapacityAlignment;
            for (int w = 0; w < words; w++)
            {
                if (_activeBits[w] == 0) continue;
                if (w < _firstWordWithActiveBits) _firstWordWithActiveBits = w;
                if (w > _lastWordWithActiveBits) _lastWordWithActiveBits = w;
            }
        }

        public ref T Get(int entityId)
        {
            int index = entityId & 0xFFFF;
            if (index < 0 || index >= _capacity) throw new InvalidOperationException("Invalid entity index");
            if (!IsActive(index) || _ids[index] != entityId)
                throw new InvalidOperationException("Invalid entity ID");

            return ref _entities[index];
        }

        public bool Remove(int entityId)
        {
            int index = entityId & 0xFFFF;
            if (index < 0 || index >= _capacity) return false;
            if (!IsActive(index) || _ids[index] != entityId) return false;
            
            SetAsInactive(index);
            _count--;
            return true;
        }

        public ActiveSpanEnumerable AsSpanEnumerable() => new(this);

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
            public ActiveSpanEnumerator GetEnumerator() => new(_pool);
        }

        public ref struct ActiveSpanEnumerator
        {
            private readonly FastPool<T> _pool;
            private ulong _bitMask;
            private int _wordIndex;
            private int _index;
            private int _remaining;
            private readonly int _lastWord;

            public ActiveSpanEnumerator(FastPool<T> pool)
            {
                _pool = pool;
                _bitMask = 0;
                _wordIndex = pool._firstWordWithActiveBits - 1;
                _index = -1;
                _remaining = pool._count;
                _lastWord = _pool._lastWordWithActiveBits;
            }

            public EntityRef<T> Current => new(_pool._ids[_index], &_pool._entities[_index]);

            public bool MoveNext()
            {
                if (_remaining <= 0 || _wordIndex > _lastWord) return false;

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
                    if (_wordIndex > _lastWord) return false;

                    _bitMask = _pool._activeBits[_wordIndex];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Free(void* ptr)
        {
            if (ptr == null) return;
            UnsafeUtility.Free(ptr, Allocator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindNextActiveWord(int start)
        {
            int words = _capacity / PoolCapacityAlignment;
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