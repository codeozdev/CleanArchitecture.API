using App.Application.Contracts.Caching;
using App.Application.Contracts.Persistence;
using App.Application.Features.Products.Create;
using App.Application.Features.Products.Response;
using App.Application.Features.Products.Update;
using App.Application.Features.Products.UpdateStock;
using App.Domain.Entities;
using AutoMapper;
using System.Net;

namespace App.Application.Features.Products;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper, IRedisCacheService redisCacheService) : IProductService
{
    public async Task<ServiceResult<List<ProductResponse>>> GetAllListAsync()
    {
        var cacheKey = CacheKeys.ProductsAll;
        var products = await redisCacheService.GetOrAddAsync(cacheKey, async () =>
        {
            var productsFromDb = await productRepository.GetAllListAsync();
            return mapper.Map<List<ProductResponse>>(productsFromDb);
        }, CacheDurations.Medium);

        return ServiceResult<List<ProductResponse>>.Success(products);
    }

    public async Task<ServiceResult<ProductResponse?>> GetByIdAsync(int id)
    {
        var cacheKey = CacheKeys.Product(id);
        var product = await redisCacheService.GetOrAddAsync(
            cacheKey,
            async () =>
            {
                var dbProduct = await productRepository.GetByIdAsync(id);
                return mapper.Map<ProductResponse>(dbProduct);
            },
            CacheDurations.Long
        );

        return product != null
            ? ServiceResult<ProductResponse?>.Success(product)
            : ServiceResult<ProductResponse?>.Fail("Not found", HttpStatusCode.NotFound);
    }

    public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        var product = mapper.Map<Product>(request);
        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();

        await redisCacheService.RemoveAsync(CacheKeys.ProductsAll);
        await redisCacheService.RemoveAsync(CacheKeys.CategoriesWithProductsAll);
        if (product.CategoryId > 0)
        {
            await redisCacheService.RemoveAsync(CacheKeys.CategoryWithProducts(product.CategoryId));
        }
        return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id), $"/api/products/{product.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
    {
        var product = mapper.Map<Product>(request);
        product.Id = id;
        productRepository.Update(product!);
        await unitOfWork.SaveChangesAsync();

        await redisCacheService.RemoveAsync(CacheKeys.ProductsAll);
        await redisCacheService.RemoveAsync(CacheKeys.Product(id));
        await redisCacheService.RemoveAsync(CacheKeys.CategoriesWithProductsAll);
        if (product.CategoryId > 0)
        {
            await redisCacheService.RemoveAsync(CacheKeys.CategoryWithProducts(product.CategoryId));
        }
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> UpdateStockAsync(int id, UpdateProductStockRequest request)
    {
        var product = await productRepository.GetByIdAsync(id);
        product!.Stock = request.Stock;
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();

        await redisCacheService.RemoveAsync(CacheKeys.ProductsAll);
        await redisCacheService.RemoveAsync(CacheKeys.Product(id));
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        productRepository.Delete(product!);
        await unitOfWork.SaveChangesAsync();

        await redisCacheService.RemoveAsync(CacheKeys.ProductsAll);
        await redisCacheService.RemoveAsync(CacheKeys.Product(id));
        await redisCacheService.RemoveAsync(CacheKeys.CategoriesWithProductsAll);
        if (product is { CategoryId: > 0 })
        {
            await redisCacheService.RemoveAsync(CacheKeys.CategoryWithProducts(product.CategoryId));
        }
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetPagedAllAsync(int pageNumber, int pageSize)
    {
        var cacheKey = CacheKeys.ProductsPage(pageNumber, pageSize);

        var products = await redisCacheService.GetOrAddAsync(cacheKey, async () =>
        {
            var productsFromDb = await productRepository.GetAllPagedAsync(pageNumber, pageSize);
            return mapper.Map<List<ProductResponse>>(productsFromDb);
        }, CacheDurations.Short);

        return ServiceResult<List<ProductResponse>>.Success(products);
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetTopPriceProductsAsync(int count)
    {
        var cacheKey = CacheKeys.ProductsTopPrice(count);

        var products = await redisCacheService.GetOrAddAsync(cacheKey, async () =>
        {
            var productsFromDb = await productRepository.GetTopPriceProductsAsync(count);
            return mapper.Map<List<ProductResponse>>(productsFromDb);
        }, CacheDurations.Short);

        return ServiceResult<List<ProductResponse>>.Success(products);
    }
}