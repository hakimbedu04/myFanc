using System.Linq.Expressions;

namespace MyFanc.Contracts.DAL
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();

        ValueTask<T?> GetByIdAsync(int id);

        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        void Add(T entity);

        void Delete(T entity);
        void Update(T entity);

    }
}
