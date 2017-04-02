using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

namespace Repository.Mongo
{
    /// <summary>
    /// entity based caching library
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityCache<T> where T : IEntity
    {
        /// <summary>
        /// constructor with limited duration cache
        /// </summary>
        /// <param name="cache">rabbit cache</param>
        /// <param name="cacheDuration">duration in minutes</param>
        public EntityCache(IDistributedCache cache, int cacheDuration)
        {
            Cache = cache;
            CacheDuration = cacheDuration;
            EntityName = typeof(T).Name;
        }

        /// <summary>
        /// check if item is cached
        /// </summary>
        /// <param name="item">entity</param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Contains(T item)
        {
            if (Equals(item, default(T)))
                return false;
            return Contains(Key(item.Id));
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
                var cItem = Cache.Get(Key(id));
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
            return Remove(Key(item.Id));
        }

        /// <summary>
        /// remove item from cache
        /// </summary>
        /// <param name="id">entity id, key value</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Remove(string id)
        {
            if (Contains(id))
                Cache.Remove(Key(id));
            return true;
        }

        /// <summary>
        /// add item into cache
        /// </summary>
        /// <param name="item">entity</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Set(T item)
        {
            if (item == null)
                return false;
            return Set(Key(item.Id), item);
        }

        /// <summary>
        /// add items into cache
        /// </summary>
        /// <param name="items">collection of entities</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool Set(IEnumerable<T> items)
        {
            if (items == null)
                return false;
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
        public bool Set(string key, T value)
        {
            if (value == null)
                return false;
            key = Key(key);
            if (Contains(key))
                Remove(key);

            var cacheValue = JsonConvert.SerializeObject(value);

            if (CacheDuration > 0)
            {
                Cache.SetString(key, cacheValue, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheDuration) });
            }
            else
            {
                Cache.SetString(key, cacheValue);
            }

            return true;
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
                var cItem = Cache.GetString(Key(id));
                if (cItem != null)
                {
                    item = (T)JsonConvert.DeserializeObject(cItem);
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
                var cItem = Cache.GetString(Key(id));
                if (cItem != null)
                {
                    items = (IEnumerable<T>)JsonConvert.DeserializeObject(cItem);
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private IDistributedCache Cache { get; set; }
        private int CacheDuration { get; set; } = 0;
        private string EntityName { get; set; }

        private string Key(string id)
        {
            return EntityName + id;
        }


    }
}