using System;
using System.Collections.Generic;

namespace IdleTycoon.Scripts.Utils
{
    public class HashQueue<T>
    {
        private readonly Queue<T> _queue = new();
        private readonly HashSet<T> _queued = new();
        
        public int Count => _queue.Count;
        
        public void Enqueue(T item)
        {
            if (_queued.Add(item))
                _queue.Enqueue(item);
        }

        public void EnqueueRange(ReadOnlySpan<T> items)
        {
            foreach (var item in items)
                Enqueue(item);
        }
        
        public bool TryDequeue(out T item)
        {
            if (!_queue.TryDequeue(out item)) return false;
            _queued.Remove(item);

            return true;
        }
        
        public void Clear()
        {
            _queue.Clear();
            _queued.Clear();
        }
    }
}