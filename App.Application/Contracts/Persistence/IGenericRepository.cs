using System.Linq.Expressions;

namespace App.Application.Contracts.Persistence;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllListAsync();
    ValueTask<T?> GetByIdAsync(int id);
    ValueTask AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<List<T>> GetAllPagedAsync(int pageNumber, int pageSize);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(int id);
}