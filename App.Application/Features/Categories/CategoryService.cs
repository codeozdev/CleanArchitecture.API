using App.Application.Contracts.Persistence;
using App.Application.Features.Categories.Create;
using App.Application.Features.Categories.Response;
using App.Application.Features.Categories.Update;
using App.Domain.Entities;
using AutoMapper;
using System.Net;

namespace App.Application.Features.Categories;

public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    public async Task<ServiceResult<List<CategoryResponse>>> GetAllListAsync()
    {
        var categories = await categoryRepository.GetAllListAsync();
        var categoriesAsDto = mapper.Map<List<CategoryResponse>>(categories);
        return ServiceResult<List<CategoryResponse>>.Success(categoriesAsDto);
    }

    public async Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        var categoryAsDto = mapper.Map<CategoryResponse>(category);
        return ServiceResult<CategoryResponse>.Success(categoryAsDto);
    }

    public async Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request)
    {
        var category = mapper.Map<Category>(request);
        await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult<CreateCategoryResponse>.SuccessAsCreated(new CreateCategoryResponse(category.Id), $"/api/categories/{category.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = mapper.Map<Category>(request);
        category.Id = id;
        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        categoryRepository.Delete(category!);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<CategoryWithProduct>> GetCategoryWithProductsAsync(int categoryId)
    {
        var category = await categoryRepository.GetCategoryWithProductsAsync(categoryId);

        if (category is null)
        {
            return ServiceResult<CategoryWithProduct>.Fail($"No categories found with id {categoryId}");
        }

        var categoryAsDto = mapper.Map<CategoryWithProduct>(category);
        return ServiceResult<CategoryWithProduct>.Success(categoryAsDto);
    }

    public async Task<ServiceResult<List<CategoryWithProduct>>> GetCategoryWithProductsAsync()
    {
        var category = await categoryRepository.GetAllCategoriesWithProductsAsync();
        var categoryAsDto = mapper.Map<List<CategoryWithProduct>>(category);
        return ServiceResult<List<CategoryWithProduct>>.Success(categoryAsDto);
    }
}