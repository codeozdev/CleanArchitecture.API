namespace App.Application.Contracts.Caching;

public static class CacheKeys
{
    // Product Cache Keys
    public static string ProductsAll => "products:all";
    public static string Product(int id) => $"product:{id}";
    public static string ProductsPage(int pageNumber, int pageSize) => $"products:page:{pageNumber}:{pageSize}";
    public static string ProductsTopPrice(int count) => $"products:topprice:{count}";

    // Category Cache Keys
    public static string CategoriesAll => "categories:all";
    public static string Category(int id) => $"category:{id}";
    public static string CategoriesWithProductsAll => "categorieswithproducts:all";
    public static string CategoryWithProducts(int id) => $"categorywithproducts:{id}";
}