using MongoDB.Driver;
using Rabbit.Cache;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Repository.Mongo
{
    /// <summary>
    /// repository implementation with caching feature, based on repository class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CachedRepository<T> : Repository<T>, IRepository<T>
        where T : IEntity
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="cache">cache</param>
        public CachedRepository(string connectionString, ICache cache) :
            base(connectionString)
        {
            Cache = new EntityCache<T>(cache);
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="cacheDuration">cache duration for collection results</param>
        /// <param name="cache">cache</param>
        public CachedRepository(string connectionString, int cacheDuration, ICache cache) :
            this(connectionString, cache)
        {
            CacheDuration = cacheDuration;
        }

        /// <summary>
        /// cache is public for custom usage
        /// </summary>
        public EntityCache<T> Cache { get; private set; }

        private int CacheDuration { get; set; }

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">id</param>
        public override void Delete(string id)
        {
            base.Delete(id);
            Cache.Remove(id);
        }

        /// <summary>
        /// find entities and caches result for defined duration
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <returns>collection of entity</returns>
        public override IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            IEnumerable<T> result = null;
            string key = filter.ToString();
            if (!Cache.TryGet(key, out result))
            {
                result = base.Find(filter);
                Cache.Set(key, result, CacheDuration);
            }
            return result;
        }

        /// <summary>
        /// find entities with paging and ordering in direction and caches result for defined duration
        /// </summary>
        /// <param name="filter">expression filter</param>
        /// <param name="order">ordering parameters</param>
        /// <param name="pageIndex">page index, based on 0</param>
        /// <param name="size">number of items in page</param>
        /// <param name="isDescending">ordering direction</param>
        /// <returns>collection of entity</returns>
        public override IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order,
                                            int pageIndex, int size, bool isDescending)
        {
            IEnumerable<T> result = null;
            string key = filter.ToString() + order + pageIndex + size + isDescending;
            if (!Cache.TryGet(key, out result))
            {
                result = base.Find(filter, order, pageIndex, size, isDescending);
                Cache.Set(key, result, CacheDuration);
            }
            return result;
        }

        /// <summary>
        /// get by id first from cache then database
        /// </summary>
        /// <param name="id">id value</param>
        /// <returns>entity of <typeparamref name="T"/></returns>
        public override T Get(string id)
        {
            T result = default(T);
            if (!Cache.TryGet(id, out result))
            {
                result = base.Get(id);
                Cache.Set(result);
            }
            return result;
        }

        /// <summary>
        /// insert entity and set to cahce
        /// </summary>
        /// <param name="entity">entity</param>
        public override void Insert(T entity)
        {
            base.Insert(entity);
            Cache.Set(entity);
        }

        /// <summary>
        /// insert entity collection and set each item into cache
        /// </summary>
        /// <param name="entities">collection of entities</param>
        public override void Insert(IEnumerable<T> entities)
        {
            base.Insert(entities);
            Cache.Set(entities);
        }

        /// <summary>
        /// replace an existing entity both on database and cache
        /// </summary>
        /// <param name="entity">entity</param>
        public override void Replace(T entity)
        {
            base.Replace(entity);
            Cache.Set(entity);
        }

        /// <summary>
        /// update an entity with updated fields and remove item from cache
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="update">updated field(s)</param>
        /// <returns>true if successful, otherwise false</returns>
        public override bool Update(T entity, UpdateDefinition<T> update)
        {
            Cache.Remove(entity);
            return base.Update(entity, update);
        }
    }
}