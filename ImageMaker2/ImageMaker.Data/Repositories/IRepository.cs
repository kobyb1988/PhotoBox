using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ImageMaker.Data.Repositories
{
    public interface IRepository
    {
        void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void Add<TEntity>(TEntity entity) where TEntity : class;

        //void Remove<TEntity>(TEntity entity) where TEntity : class;

        void Remove<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void Update<TEntity>(TEntity entity) where TEntity : class;

        void Update<TEntity>(IEnumerable<TEntity> entites) where TEntity : class ;

        void Commit();

        TEntity GetSingle<TEntity>(Expression<Func<TEntity, bool>> selector) where TEntity : class;

        Task<TEntity> GetSingleAsync<TEntity>(Expression<Func<TEntity, bool>> selector) where TEntity : class;

        IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class;

        IQueryable<TEntity> QueryAll<TEntity>() where TEntity : class;

        IEnumerable<TEntity> GetAll<TEntity>(Expression<Func<TEntity, bool>> selector) where TEntity : class;
    }
}