using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq.Expressions;
using System.Net;
using UsersManagement.Entities;
using UsersManagement.Persistence;

namespace UsersManagement.Repositories
{
    public class Repository : IRepository
    {
        private readonly IMainDbContext _dbContext;

        public Repository(IMainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T?> GetById<T>(int id) where T : IEntity
        {
            var ret =  await _dbContext.Set<T>().FindAsync(id);
            return ret;
        }
        public async Task<T?> GetByIdWithIncludes<T>(int id) where T : IEntity
        {
            IQueryable<T> query = _dbContext.Set<T>();
             
            var navigationProperties = _dbContext.Model.FindEntityType(typeof(T))
                .GetNavigations()
                .Select(e => e.Name);
             
            foreach (var propertyName in navigationProperties)
            {
                query = query.Include(propertyName);
            }
             
            var entity = await query.FirstOrDefaultAsync(e => e.Id == id);
            return entity;
        }


        public IQueryable<T> FindQueryable<T>(Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null) where T : IEntity
        {
            var query = _dbContext.Set<T>().Where(expression);
            return orderBy != null ? orderBy(query) : query;
        }

        public Task<List<T>> FindListAsync<T>(Expression<Func<T, bool>>? expression, Func<IQueryable<T>, 
            IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default)
            where T : class
        {
            var query = expression != null ? _dbContext.Set<T>().Where(expression) : _dbContext.Set<T>();
            return orderBy != null
                ? orderBy(query).ToListAsync(cancellationToken)
                : query.ToListAsync(cancellationToken);
        }

        public Task<List<T>> FindAllAsync<T>(CancellationToken cancellationToken) where T : IEntity
        {
            return _dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public Task<T?> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> expression, string includeProperties) where T : IEntity
        {
            var query = _dbContext.Set<T>().AsQueryable();

            query = includeProperties.Split(new char[] { ',' }, 
                StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) 
                => current.Include(includeProperty));

            return query.SingleOrDefaultAsync(expression);
        }
        public async Task<List<T>> FindListAsyncWithIncludes<T>(Expression<Func<T, bool>>? expression, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default) where T : IEntity
        {
             IQueryable<T> query = _dbContext.Set<T>();

            var entityType = _dbContext.Model.FindEntityType(typeof(T));
            var navigationProperties = entityType.GetNavigations().Select(e => e.Name);
            foreach (var propertyName in navigationProperties)
            {
                query = query.Include(propertyName);
            }

            
            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public T Add<T>(T entity) where T : IEntity
        {
            return _dbContext.Set<T>().Add(entity).Entity;
        }

        public void Update<T>(T entity) where T : IEntity
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateRange<T>(IEnumerable<T> entities) where T : IEntity
        {
            _dbContext.Set<T>().UpdateRange(entities);
        }

      
        public void Delete<T>(T entity) where T : IEntity
        {
             var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Set<T>().Attach(entity);
            }

            _dbContext.Set<T>().Remove(entity);
        }


    }
}
