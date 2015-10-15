using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImageMaker.DataContext;
using ImageMaker.DataContext.Contexts;
using ImageMaker.Entities;

namespace ImageMaker.Data.Repositories
{
    public abstract class RepositoryBase<TContext> : IRepository where TContext : DbContext
    {
        protected readonly TContext Context;

        protected RepositoryBase(TContext context)
        {
            Context = context;
        }

        public void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Context.Set<TEntity>().AddRange(entities);
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            Context.Set<TEntity>().Add(entity);
        }

        //public void Remove<TEntity>(TEntity entity) where TEntity : class
        //{
        //    Context.Set<TEntity>().Remove(entity);
        //}

        public void Remove<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Update<TEntity>(IEnumerable<TEntity> entites) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public TEntity GetSingle<TEntity>(Expression<Func<TEntity, bool>> selector) where TEntity : class
        {
            return Context.Set<TEntity>().FirstOrDefault(selector);
        }

        public Task<TEntity> GetSingleAsync<TEntity>(Expression<Func<TEntity, bool>> selector) where TEntity : class
        {
            return Context.Set<TEntity>().FirstOrDefaultAsync(selector);
        }


        public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return Context.Set<TEntity>().ToList();
        }

        public IQueryable<TEntity> QueryAll<TEntity>() where TEntity : class
        {
            return Context.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll<TEntity>(Expression<Func<TEntity, bool>> selector) where TEntity : class
        {
            return Context.Set<TEntity>().Where(selector).ToList();
        }
    }

    public interface IPatternRepository : IRepository
    {
        IEnumerable<Pattern> GetPatterns();

        void RemovePatternsData(IEnumerable<PatternData> patterns);

        void AddPatternsData(IEnumerable<PatternData> patterns);
    }

    public class PatternRepository : RepositoryBase<PatternContext>, IPatternRepository
    {
        public PatternRepository(PatternContext context) : base(context)
        {
        }

        public IEnumerable<Pattern> GetPatterns()
        {
            return QueryAll<Pattern>().Include(x => x.Children).ToList();
        }

        public void RemovePatternsData(IEnumerable<PatternData> patterns)
        {
            Remove(patterns.Select(x => x.Id).Join(QueryAll<PatternData>(), x => x, x => x.Id, (x, y) => y));
        }

        public void AddPatternsData(IEnumerable<PatternData> patterns)
        {
            Add(patterns);
        }
    }
}
