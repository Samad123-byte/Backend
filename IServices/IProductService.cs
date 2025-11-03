using Backend.Models;

namespace Backend.IServices
{
    public interface IProductService
    {
        Task<PaginatedResponse<Product>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10); // ✅ Changed
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<int> UpdateProductAsync(Product product);
        Task<int> DeleteProductAsync(int id);
        Task<bool> ProductExistsAsync(int id);

        Task<(bool Success, string Message, Product? Data)> CreateProductLogicAsync(Product product);

    }
}
