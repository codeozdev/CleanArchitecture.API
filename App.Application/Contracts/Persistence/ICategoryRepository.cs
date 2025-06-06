using App.Domain.Entities;

namespace App.Application.Contracts.Persistence;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetCategoryWithProductsAsync(int id);
    Task<List<Category>> GetAllCategoriesWithProductsAsync();
}