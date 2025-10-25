using Backend.Models;

namespace Backend.IServices
{
    public interface IProductService
    {
        Task<PaginatedResponse<Product>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10); // ✅ Changed
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> ProductExistsAsync(int id);
    }
}
