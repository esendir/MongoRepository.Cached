using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Mongo
{
    /// <summary>
    /// entity based caching library
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityCache<T> where T : IEntity
    {
        /// <summary>
        /// check if item is cached
        /// </summary>
        /// <param name="item">entity</param>
        /// <returns>true if exists, otherwise false</returns>
        bool Contains(T item);

        /// <summary>
        /// check if key value is cached
        /// </summary>
        /// <param name="id">entity id</param>
        /// <returns>true if exists, otherwise false</returns>
        bool Contains(string id);

        /// <summary>
        /// remove item from cache
        /// </summary>
        /// <param name="item">entity</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Remove(T item);

        /// <summary>
        /// remove item from cache
        /// </summary>
        /// <param name="id">entity id, key value</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Remove(string id);

        /// <summary>
        /// add item into cache
        /// </summary>
        /// <param name="item">entity</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Set(T item);

        /// <summary>
        /// add items into cache
        /// </summary>
        /// <param name="items">collection of entities</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Set(IEnumerable<T> items);

        /// <summary>
        /// add item into cache
        /// </summary>
        /// <param name="key">custom key value, entity id is default</param>
        /// <param name="value">item to cache</param>
        /// <returns>true if successful, otherwise false</returns>
        bool Set(string key, T value);

        /// <summary>
        /// get item from cache
        /// </summary>
        /// <param name="id">key</param>
        /// <param name="item">value as entity item</param>
        /// <returns>true if exists, otherwise false</returns>
        bool TryGet(string id, out T item);

        /// <summary>
        /// get items from cache
        /// </summary>
        /// <param name="id">key</param>
        /// <param name="items">value as collection of entity items</param>
        /// <returns>true if exists, otherwise false</returns>
        bool TryGet(string id, out IEnumerable<T> items);
    }
}
