using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Rabbit.Cache;

namespace Repository.Mongo
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class CachedRepository<T> : Repository<T>
        where T : IEntity
    {
        public EntityCache<T> Cache { get; private set; }
        private int CacheDuration { get; set; }

        public CachedRepository(string connectionString, ICache cache) :
            base(connectionString)
        {
            Cache = new EntityCache<T>(cache);
        }

        public CachedRepository(string connectionString, int cacheDuration, ICache cache) :
            this(connectionString, cache)
        {
            CacheDuration = cacheDuration;
        }

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

        public override void Insert(T entity)
        {
            base.Insert(entity);
            Cache.Set(entity);
        }

        public override void Insert(IEnumerable<T> entities)
        {
            base.Insert(entities);
            Cache.Set(entities);
        }

        public override bool Update(T entity, UpdateDefinition<T> update)
        {
            Cache.Remove(entity);
            return base.Update(entity, update);
        }

        public override void Replace(T entity)
        {
            base.Replace(entity);
            Cache.Set(entity);
        }

        public override void Delete(string id)
        {
            base.Delete(id);
            Cache.Remove(id);
        }

    }
}
