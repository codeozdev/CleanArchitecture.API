using App.Domain.Entities;

namespace App.Application.Contracts.Persistence;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<List<Product>> GetTopPriceProductsAsync(int count);
}