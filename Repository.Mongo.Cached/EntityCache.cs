using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rabbit.Cache;

namespace Repository.Mongo
{
    public class EntityCache<T> where T : IEntity
    {
        internal ICache Cache { get; private set; }

        internal EntityCache(ICache cache)
        {
            Cache = cache;
        }

        public bool Set(T item)
        {
            return Set(item.Id, item);
        }

        public bool Set(IEnumerable<T> items)
        {
            foreach (var item in items)
                Set(item);
            return true;
        }

        public bool Set(string key, object value)
        {
            return Set(key, value, 0);
        }

        public bool Set(string key, object value, int min)
        {
            if (Equals(value, null))
                return false;
            if (Contains(key))
                Remove(key);

            return min > 0 ? Cache.Set(key, value, TimeSpan.FromMinutes(min)) : Cache.Set(key, value);
        }

        public bool Contains(T item)
        {
            if (Equals(item, default(T)))
                return false;
            return Contains(item.Id);
        }

        public bool Contains(string id)
        {
            bool result = false;
            try
            {
                var cItem = Cache.Get<object>(id);
                result = cItem != null;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool TryGet(string id, out T item)
        {
            item = default(T);
            bool result = false;
            try
            {
                var cItem = Cache.Get<object>(id);
                if (cItem != null)
                {
                    item = (T)cItem;
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool TryGet(string id, out IEnumerable<T> items)
        {
            items = null;
            bool result = false;

            try
            {
                var cItem = Cache.Get<object>(id);
                if (cItem != null)
                {
                    items = (IEnumerable<T>)cItem;
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool Remove(T item)
        {
            if (Equals(item, default(T)))
                return false;
            return Remove(item.Id);
        }

        public bool Remove(string id)
        {
            if (Contains(id))
                return Cache.Remove(id);
            return true;
        }

    }
}
