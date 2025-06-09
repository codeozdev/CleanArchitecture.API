using App.Application.Contracts.Caching;
using App.Application.Contracts.Persistence;
using App.Application.Features.Categories.Create;
using App.Application.Features.Categories.Response;
using App.Application.Features.Categories.Update;
using App.Domain.Entities;
using AutoMapper;
using System.Net;

namespace App.Application.Features.Categories;

public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper, IRedisCacheService redisCacheService) : ICategoryService
{
    public async Task<ServiceResult<List<CategoryResponse>>> GetAllListAsync()
    {
        var cacheKey = CacheKeys.CategoriesAll;
        var categories = await redisCacheService.GetOrAddAsync(cacheKey, async () =>
        {
            var categoriesFromDb = await categoryRepository.GetAllListAsync();
            return mapper.Map<List<CategoryResponse>>(categoriesFromDb);
        }, CacheDurations.Medium);

        return ServiceResult<List<CategoryResponse>>.Success(categories);
    }

    public async Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id)
    {
        var cacheKey = CacheKeys.Category(id);
        var category = await redisCacheService.GetOrAddAsync(
            cacheKey,
            async () =>
            {
                var dbCategory = await categoryRepository.GetByIdAsync(id);
                return mapper.Map<CategoryResponse>(dbCategory);
            },
            CacheDurations.Long
        );

        return (category != null
            ? ServiceResult<CategoryResponse>.Success(category)
            : ServiceResult<CategoryResponse>.Fail("Not found", HttpStatusCode.NotFound))!;
    }

    public async Task<ServiceResult<CreateCategoryResponse>> CreateAsync(CreateCategoryRequest request)
    {
        var category = mapper.Map<Category>(request);
        await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync();

        await redisCacheService.RemoveAsync(CacheKeys.CategoriesAll);
        await redisCacheService.RemoveAsync(CacheKeys.CategoriesWithProductsAll);

        return ServiceResult<CreateCategoryResponse>.SuccessAsCreated(new CreateCategoryResponse(category.Id), $"/api/categories/{category.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = mapper.Map<Category>(request);
        category.Id = id;
        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();

        await redisCacheService.RemoveAsync(CacheKeys.CategoriesAll);
        await redisCacheService.RemoveAsync(CacheKeys.Category(id));
        await redisCacheService.RemoveAsync(CacheKeys.CategoriesWithProductsAll);
        await redisCacheService.RemoveAsync(CacheKeys.CategoryWithProducts(id));

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        categoryRepository.Delete(category!);
        await unitOfWork.SaveChangesAsync();

        await redisCacheService.RemoveAsync(CacheKeys.CategoriesAll);
        await redisCacheService.RemoveAsync(CacheKeys.Category(id));
        await redisCacheService.RemoveAsync(CacheKeys.CategoriesWithProductsAll);
        await redisCacheService.RemoveAsync(CacheKeys.CategoryWithProducts(id));

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<CategoryWithProduct>> GetCategoryWithProductsAsync(int categoryId)
    {
        var cacheKey = CacheKeys.CategoryWithProducts(categoryId);
        var category = await redisCacheService.GetOrAddAsync(
            cacheKey,
            async () => await categoryRepository.GetCategoryWithProductsAsync(categoryId),
            CacheDurations.Long
        );

        if (category is null)
        {
            return ServiceResult<CategoryWithProduct>.Fail($"No categories found with id {categoryId}");
        }

        var categoryAsDto = mapper.Map<CategoryWithProduct>(category);
        return ServiceResult<CategoryWithProduct>.Success(categoryAsDto);
    }

    public async Task<ServiceResult<List<CategoryWithProduct>>> GetCategoryWithProductsAsync()
    {
        var cacheKey = CacheKeys.CategoriesWithProductsAll;
        var categories = await redisCacheService.GetOrAddAsync(
            cacheKey,
            async () => await categoryRepository.GetAllCategoriesWithProductsAsync(),
            CacheDurations.Long
        );

        var categoriesAsDto = mapper.Map<List<CategoryWithProduct>>(categories);
        return ServiceResult<List<CategoryWithProduct>>.Success(categoriesAsDto);
    }
}