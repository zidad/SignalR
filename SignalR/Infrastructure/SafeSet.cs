using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SignalR.Infrastructure
{
    internal class SafeSet<T>
    {
        private readonly ConcurrentDictionary<T, object> _items;
        private readonly CustomStack<T> _allKeys = new CustomStack<T>();

        public SafeSet()
        {
            _items = new ConcurrentDictionary<T, object>();
        }

        public SafeSet(IEqualityComparer<T> comparer)
        {
            _items = new ConcurrentDictionary<T, object>(comparer);
        }

        public SafeSet(IEnumerable<T> items)
        {
            _items = new ConcurrentDictionary<T, object>(items.Select(x => new KeyValuePair<T, object>(x, null)));
        }

        public IEnumerable<T> GetSnapshot()
        {
            // The Keys property locks, so Select instead
            // return _items.Select(item => item.Key);
            return _allKeys.GetAll();
        }

        public void Add(T item)
        {
            _items.TryAdd(item, null);
            _allKeys.Add(item);
        }

        public void Remove(T item)
        {
            object _;
            _items.TryRemove(item, out _);
        }

        public long Count
        {
            get { return _allKeys.Count; }
        }
    }
}