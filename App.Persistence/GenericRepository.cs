using App.Application.Contracts.Persistence;
using App.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Persistence;

public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<List<T>> GetAllListAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async ValueTask<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async ValueTask AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<List<T>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return await _dbSet.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<bool> AnyAsync(int id)
    {
        return await _dbSet.AnyAsync(x => x.Id == id);
    }
}