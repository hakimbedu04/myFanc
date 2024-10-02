using Microsoft.EntityFrameworkCore;
using MyFanc.Contracts.DAL;
using System.Linq.Expressions;

namespace MyFanc.DAL
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<T>();
        }

        public Task<List<T>> GetAllAsync()
        {
            return _dbSet.ToListAsync();
        }

        public ValueTask<T?> GetByIdAsync(int id)
        {
            return _dbSet.FindAsync(id);
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            var entry = _dbContext.Entry(entity);

            entry.State = EntityState.Detached;

            entry.State = EntityState.Modified;
        }
    }
}
