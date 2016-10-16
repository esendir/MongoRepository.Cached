using MongoDB.Driver;
using Rabbit.Cache;
using System.Collections.Generic;

namespace Repository.Mongo
{
    /// <summary>
    /// repository implementation with caching feature, based on repository class
    /// </summary>
    /// <typeparam name="T">entity type</typeparam>
    public class CachedRepository<T> : Repository<T>, IRepository<T>
        where T : IEntity
    {
        /// <summary>
        /// repository with cache
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="cache">cache</param>
        public CachedRepository(string connectionString, ICache cache) :
            this(connectionString, cache, 0)
        {
        }

        /// <summary>
        /// repository with time limited cache
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="cacheDuration">cache duration in minutes</param>
        /// <param name="cache">cache</param>
        public CachedRepository(string connectionString, ICache cache, int cacheDuration) :
            base(connectionString)
        {
            Cache = new EntityCache<T>(cache, cacheDuration);
        }

        /// <summary>
        /// cache is public for custom usage
        /// </summary>
        public EntityCache<T> Cache { get; private set; }

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
        /// get by id
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
        public override bool Update(T entity, params UpdateDefinition<T>[] updates)
        {
            Cache.Remove(entity);
            return base.Update(entity, updates);
        }
    }
}