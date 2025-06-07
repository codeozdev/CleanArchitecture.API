using App.Application.Features.Categories.Create;
using App.Application.Features.Categories.Response;
using App.Application.Features.Categories.Update;

namespace App.Application.Features.Categories;

public interface ICategoryService
{
    Task<ServiceResult<List<CategoryResponse>>> GetAllListAsync();
    Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id);
    Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request);
    Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult<CategoryWithProduct>> GetCategoryWithProductsAsync(int categoryId);
    Task<ServiceResult<List<CategoryWithProduct>>> GetCategoryWithProductsAsync();
}