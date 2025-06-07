using App.API.Filters;
using App.Application.Features.Categories;
using App.Application.Features.Categories.Create;
using App.Application.Features.Categories.Update;
using App.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    public class CategoriesController(ICategoryService categoryService) : CustomBaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return CreateActionResult(await categoryService.GetAllListAsync());
        }

        [ServiceFilter(typeof(IdCheckFilter<Category>))]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return CreateActionResult(await categoryService.GetByIdAsync(id));
        }

        [ServiceFilter(typeof(NameCheckFilter<Category>))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            return CreateActionResult(await categoryService.CreateAsync(request));
        }

        [ServiceFilter(typeof(IdCheckFilter<Category>))]
        [ServiceFilter(typeof(NameCheckFilter<Category>))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCategoryRequest request)
        {
            return CreateActionResult(await categoryService.UpdateAsync(id, request));
        }

        [ServiceFilter(typeof(IdCheckFilter<Category>))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return CreateActionResult(await categoryService.DeleteAsync(id));
        }

        [HttpGet("{id:int}/products")]
        public async Task<IActionResult> GetCategoryWithProducts([FromRoute] int id)
        {
            return CreateActionResult(await categoryService.GetCategoryWithProductsAsync(id));
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetCategoryWithProducts()
        {
            return CreateActionResult(await categoryService.GetCategoryWithProductsAsync());
        }
    }
}