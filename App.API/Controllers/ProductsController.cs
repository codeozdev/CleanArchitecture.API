using App.API.Filters;
using App.Application.Features.Products;
using App.Application.Features.Products.Create;
using App.Application.Features.Products.Update;
using App.Application.Features.Products.UpdateStock;
using App.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{

    public class ProductsController(IProductService productService) : CustomBaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return CreateActionResult(await productService.GetAllListAsync());
        }

        [ServiceFilter(typeof(IdCheckFilter<Product>))]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return CreateActionResult(await productService.GetByIdAsync(id));
        }

        [ServiceFilter(typeof(NameCheckFilter<Product>))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            return CreateActionResult(await productService.CreateAsync(request));
        }

        [ServiceFilter(typeof(IdCheckFilter<Product>))]
        [ServiceFilter(typeof(NameCheckFilter<Product>))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductRequest request)
        {
            return CreateActionResult(await productService.UpdateAsync(id, request));
        }

        [ServiceFilter(typeof(IdCheckFilter<Product>))]
        [HttpPatch("stock/{id:int}")]
        public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateProductStockRequest request)
        {
            return CreateActionResult(await productService.UpdateStockAsync(id, request));
        }

        [ServiceFilter(typeof(IdCheckFilter<Product>))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return CreateActionResult(await productService.DeleteAsync(id));
        }

        [HttpGet("{pageNumber:int}/{pageSize:int}")]
        public async Task<IActionResult> GetPagedAll([FromRoute] int pageNumber, [FromRoute] int pageSize)
        {
            return CreateActionResult(await productService.GetPagedAllAsync(pageNumber, pageSize));
        }

        [HttpGet("top/{count:int}")]
        public async Task<IActionResult> GetTopPriceProducts([FromRoute] int count)
        {
            return CreateActionResult(await productService.GetTopPriceProductsAsync(count));
        }
    }
}
