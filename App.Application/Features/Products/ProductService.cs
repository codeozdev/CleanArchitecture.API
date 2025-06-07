using App.Application.Contracts.Persistence;
using App.Application.Features.Products.Create;
using App.Application.Features.Products.Response;
using App.Application.Features.Products.Update;
using App.Application.Features.Products.UpdateStock;
using App.Domain.Entities;
using AutoMapper;
using System.Net;

namespace App.Application.Features.Products;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper) : IProductService
{
    public async Task<ServiceResult<List<ProductResponse>>> GetAllListAsync()
    {
        var products = await productRepository.GetAllListAsync();
        var productsAsDto = mapper.Map<List<ProductResponse>>(products);
        return ServiceResult<List<ProductResponse>>.Success(productsAsDto);
    }

    public async Task<ServiceResult<ProductResponse?>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        var productAsDto = mapper.Map<ProductResponse?>(product);
        return ServiceResult<ProductResponse?>.Success(productAsDto);
    }

    public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        var product = mapper.Map<Product>(request);
        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id), $"/api/products/{product.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
    {
        var product = mapper.Map<Product>(request);
        product.Id = id;
        productRepository.Update(product!);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> UpdateStockAsync(int id, UpdateProductStockRequest request)
    {
        var product = await productRepository.GetByIdAsync(id);
        product!.Stock = request.Stock;
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        productRepository.Delete(product!);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetPagedAllAsync(int pageNumber, int pageSize)
    {
        var products = await productRepository.GetAllPagedAsync(pageNumber, pageSize);
        var productsDto = mapper.Map<List<ProductResponse>>(products);
        return ServiceResult<List<ProductResponse>>.Success(productsDto);
    }

    public async Task<ServiceResult<List<ProductResponse>>> GetTopPriceProductsAsync(int count)
    {
        var products = await productRepository.GetTopPriceProductsAsync(count);
        var productsAsDto = mapper.Map<List<ProductResponse>>(products);
        return ServiceResult<List<ProductResponse>>.Success(productsAsDto);
    }
}