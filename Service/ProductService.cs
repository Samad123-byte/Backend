using Backend.IRepository;
using Backend.IServices;
using Backend.Models;
using Microsoft.Data.SqlClient;

namespace Backend.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                return await _productRepository.CreateProductAsync(product);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    2627 => new InvalidOperationException("A product with this code already exists"),
                    2601 => new InvalidOperationException("Duplicate product code"),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                return await _productRepository.UpdateProductAsync(product);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    2627 => new InvalidOperationException("A product with this code already exists"),
                    2601 => new InvalidOperationException("Duplicate product code"),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                return await _productRepository.DeleteProductAsync(id);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    547 => new InvalidOperationException("Cannot delete product. This product is referenced in existing sale details. Please remove it from all sales before deleting the product."),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _productRepository.ProductExistsAsync(id);
        }
    }
}
