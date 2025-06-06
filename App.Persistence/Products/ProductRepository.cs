using App.Application.Contracts.Persistence;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Persistence.Products;

public class ProductRepository(AppDbContext context) : GenericRepository<Product>(context), IProductRepository
{
    public async Task<List<Product>> GetTopPriceProductsAsync(int count)
    {
        return await Context.Products.OrderByDescending(x => x.Price).Take(count).ToListAsync();
    }
}