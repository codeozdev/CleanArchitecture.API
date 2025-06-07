using App.Application.Features.Products.Response;

namespace App.Application.Features.Categories.Response;

public record CategoryWithProduct(int Id, string Name, List<ProductResponse> Products);