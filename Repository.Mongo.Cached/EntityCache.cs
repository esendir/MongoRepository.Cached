using Rabbit.Cache;
using System;
using System.Collections.Generic;

namespace Repository.Mongo
{
    /// <summary>
    /// entity based caching library
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityCache<T> where T : IEntity
    {
        /// <summary>
        /// constructor with a cache
        /// </summary>
        /// <param name="cache">Rabbit cache</param>
        public EntityCache(ICache cache)
        {
            Cache = cache;
        }

        private ICache Cache { get; set; }

        /// <summary>
        /// check if item is cached
        /// </summary>
        /// <param name="item">entity</param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Contains(T item)
        {
            if (Equals(item, default(T)))
                return false;
            return Contains(item.Id);
        }

        /// <summary>
        /// check if key value is cached
        /// </summary>
        /// <param name="id">entity id</param>
        /// <returns>true if exists, otherwise false</returns>
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

        /// <summary>
        /// remove item from cache
        /// </summary>
        /// <param name="item">entity</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Remove(T item)
        {
            if (Equals(item, default(T)))
                return false;
            return Remove(item.Id);
        }

        /// <summary>
        /// remove item from cache
        /// </summary>
        /// <param name="id">entity id, key value</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Remove(string id)
        {
            if (Contains(id))
                return Cache.Remove(id);
            return true;
        }

        /// <summary>
        /// add item into cache
        /// </summary>
        /// <param name="item">entity</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Set(T item)
        {
            return Set(item.Id, item);
        }

        /// <summary>
        /// add items into cache
        /// </summary>
        /// <param name="items">collection of entities</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Set(IEnumerable<T> items)
        {
            foreach (var item in items)
                Set(item);
            return true;
        }

        /// <summary>
        /// add item into cache
        /// </summary>
        /// <param name="key">custom key value, entity id is default</param>
        /// <param name="value">item to cache</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Set(string key, object value)
        {
            return Set(key, value, 0);
        }

        /// <summary>
        /// add item into cache
        /// </summary>
        /// <param name="key">custom key value, entity id is default</param>
        /// <param name="value">item to cache</param>
        /// <param name="min">duration of cache time</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Set(string key, object value, int min)
        {
            if (Equals(value, null))
                return false;
            if (Contains(key))
                Remove(key);

            return min > 0 ? Cache.Set(key, value, TimeSpan.FromMinutes(min)) : Cache.Set(key, value);
        }

        /// <summary>
        /// get item from cache
        /// </summary>
        /// <param name="id">key</param>
        /// <param name="item">value as entity item</param>
        /// <returns>true if exists, otherwise false</returns>
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

        /// <summary>
        /// get items from cache
        /// </summary>
        /// <param name="id">key</param>
        /// <param name="items">value as collection of entity items</param>
        /// <returns>true if exists, otherwise false</returns>
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
    }
}