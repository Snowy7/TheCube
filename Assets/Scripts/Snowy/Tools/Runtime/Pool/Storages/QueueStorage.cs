using System.Collections.Generic;
#if !UNITY_2021_2_OR_NEWER
using Snowy.CSharp.Collections;
#endif

namespace Snowy.Pool.Storages
{
    public class QueueStorage<T> : Queue<T>, IPoolStorage<T> where T : class
    {
        public QueueStorage() { }
        public QueueStorage(int capacity) : base(capacity) { }
        public QueueStorage(IEnumerable<T> collection) : base(collection) { }

        public bool TryAdd(T value)
        {
            Enqueue(value);
            return true;
        }

        public bool TryGet(out T value)
        {
#if UNITY_2021_2_OR_NEWER
            return TryDequeue(out value);
#else
            return this.TryDequeue(out value);
#endif
        }
    }
}
