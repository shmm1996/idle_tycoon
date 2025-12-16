#nullable enable
using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Utils
{
    public unsafe struct MaskBatchMap<T> : IDisposable where T : unmanaged
    {
        public struct Pair
        {
            public T key;
            public uint value;
        }

        private T* _keys;
        private uint* _values;
        private int _count;
        private int _denseCap;

        private int* _bucketIdx;
        private uint* _bucketEpoch;
        private int _bucketCap;
        private int _bucketMask;
        private uint _epoch;

        private Pair* _outPairs;
        private int _outCap;

        public int Count => _count;

        public MaskBatchMap(int capacity)
        {
            if (capacity < 1) capacity = 1;

            _count = 0;
            _epoch = 1;

            _denseCap = UnsafeUtilityUtils.RoundUpPow2(capacity);

            _keys   = UnsafeUtilityUtils.MallocArray<T>(_denseCap);
            _values = UnsafeUtilityUtils.MallocArray<uint>(_denseCap);

            _bucketCap = UnsafeUtilityUtils.RoundUpPow2(capacity);
            if (_bucketCap < 8) _bucketCap = 8;
            _bucketMask = _bucketCap - 1;

            _bucketIdx   = UnsafeUtilityUtils.MallocArray<int>(_bucketCap);
            _bucketEpoch = UnsafeUtilityUtils.MallocArray<uint>(_bucketCap);
            UnsafeUtility.MemClear(_bucketEpoch, (long)_bucketCap * sizeof(uint));

            _outPairs = null;
            _outCap = 0;
        }

        public void Dispose()
        {
            UnsafeUtilityUtils.Free(_keys);
            UnsafeUtilityUtils.Free(_values);
            UnsafeUtilityUtils.Free(_bucketIdx);
            UnsafeUtilityUtils.Free(_bucketEpoch);
            UnsafeUtilityUtils.Free(_outPairs);

            _keys = null;
            _values = null;
            _bucketIdx = null;
            _bucketEpoch = null;
            _outPairs = null;

            _count = 0;
            _denseCap = 0;
            _bucketCap = 0;
            _bucketMask = 0;
            _epoch = 0;
            _outCap = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddOr(T key, uint mask)
        {
            if (mask == 0) return;

            EnsureDenseForOneMore();

            while (true)
            {
                int hash = key.GetHashCode();

                if (TryFindSlot(key, hash, out int slot, out int existingIdx))
                {
                    if (existingIdx >= 0)
                    {
                        _values[existingIdx] |= mask;
                        return;
                    }

                    int idx = _count++;
                    _keys[idx] = key;
                    _values[idx] = mask;

                    _bucketEpoch[slot] = _epoch;
                    _bucketIdx[slot] = idx + 1;
                
                    return;
                }

                RebuildBuckets(_bucketCap * 2);
            }
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<Pair> ToSpan()
        {
            EnsureOutCapacity(_count);

            int w = 0;
            for (int i = 0; i < _count; i++)
            {
                uint v = _values[i];
                if (v == 0) continue;

                _outPairs[w].key = _keys[i];
                _outPairs[w].value = v;
                w++;
            }

            return new Span<Pair>(_outPairs, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            if (_count == 0) return;
            
            _count = 0;
            _epoch++;

            if (_epoch != 0) return;
        
            UnsafeUtility.MemClear(_bucketEpoch, (long)_bucketCap * sizeof(uint));
            _epoch = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureDenseForOneMore()
        {
            if (_count < _denseCap) return;

            int newCap = _denseCap * 2;
            _keys   = UnsafeUtilityUtils.ReAllocArray(_keys, _denseCap, newCap);
            _values = UnsafeUtilityUtils.ReAllocArray(_values, _denseCap, newCap);
            _denseCap = newCap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureOutCapacity(int needed)
        {
            if (needed <= _outCap) return;

            int newCap = _outCap == 0 ? UnsafeUtilityUtils.RoundUpPow2(math.max(8, needed)) : _outCap;
            while (newCap < needed) newCap *= 2;

            _outPairs = UnsafeUtilityUtils.ReAllocArray(_outPairs, _outCap, newCap);
            _outCap = newCap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryFindSlot(T key, int hash, out int slot, out int existingDenseIdx)
        {
            int start = hash & _bucketMask;
            slot = start;

            while (true)
            {
                if (_bucketEpoch[slot] != _epoch)
                {
                    existingDenseIdx = -1;
                    return true;
                }

                int idx = _bucketIdx[slot] - 1;
                T k = _keys[idx];
                if (k.Equals(key))
                {
                    existingDenseIdx = idx;
                    return true;
                }

                slot = (slot + 1) & _bucketMask;
                if (slot == start)
                {
                    existingDenseIdx = -1;
                    return false;
                }
            }
        }

        private void RebuildBuckets(int newCap)
        {
            newCap = UnsafeUtilityUtils.RoundUpPow2(newCap);
            if (newCap < 8) newCap = 8;

            int*  newIdx   = UnsafeUtilityUtils.MallocArray<int>(newCap);
            uint* newEpoch = UnsafeUtilityUtils.MallocArray<uint>(newCap);
            UnsafeUtility.MemClear(newEpoch, (long)newCap * sizeof(uint));

            int newMask = newCap - 1;
            uint epoch = 1;

            for (int i = 0; i < _count; i++)
            {
                T k = _keys[i];
                int slot = k.GetHashCode() & newMask;

                while (newEpoch[slot] == epoch)
                    slot = (slot + 1) & newMask;

                newEpoch[slot] = epoch;
                newIdx[slot] = i + 1;
            }

            UnsafeUtilityUtils.Free(_bucketIdx);
            UnsafeUtilityUtils.Free(_bucketEpoch);

            _bucketIdx = newIdx;
            _bucketEpoch = newEpoch;
            _bucketCap = newCap;
            _bucketMask = newMask;
            _epoch = epoch;
        }
    }
}
