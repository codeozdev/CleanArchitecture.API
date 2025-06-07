using App.Application.Features.Products.Create;
using App.Application.Features.Products.Response;
using App.Application.Features.Products.Update;
using App.Application.Features.Products.UpdateStock;

namespace App.Application.Features.Products;

public interface IProductService
{
    Task<ServiceResult<List<ProductResponse>>> GetAllListAsync();
    Task<ServiceResult<ProductResponse?>> GetByIdAsync(int id);
    Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request);
    Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request);
    Task<ServiceResult> UpdateStockAsync(int id, UpdateProductStockRequest request);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult<List<ProductResponse>>> GetPagedAllAsync(int pageNumber, int pageSize);
    Task<ServiceResult<List<ProductResponse>>> GetTopPriceProductsAsync(int count);
}