using Backend.IRepository;
using Backend.IServices;
using Backend.Models;
using Microsoft.Data.SqlClient;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PaginatedResponse<Product>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10)
        => await _productRepository.GetAllProductsAsync(pageNumber, pageSize);

    public async Task<Product?> GetProductByIdAsync(int id)
        => await _productRepository.GetProductByIdAsync(id);

    public async Task<Product> CreateProductAsync(Product product)
    {
        // Just call the logic method internally
        var result = await CreateProductLogicAsync(product);

        if (!result.Success || result.Data == null)
            throw new InvalidOperationException(result.Message);

        return result.Data;
    }


    // Interface
    public async Task<(bool Success, string Message, Product? Data)> CreateProductLogicAsync(Product product)
    {
        // Check duplicate before calling repository
        var allProducts = await _productRepository.GetAllProductsAsync(1, int.MaxValue);
        bool duplicateExists = allProducts.Data.Any(p =>
            p.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase) ||
            (!string.IsNullOrEmpty(product.Code) && p.Code == product.Code));

        if (duplicateExists)
            return (false, "Duplicate product Name or Code.", null);

        // Call repository to create product
        var createdProduct = await _productRepository.CreateProductAsync(product);

        return (true, "Product created successfully.", createdProduct);
    }


    public async Task<int> UpdateProductAsync(Product product)
    {
        // 1️⃣ Check for duplicate excluding the current product
        //var allProducts = await _productRepository.GetAllProductsAsync(1, int.MaxValue);
        //bool duplicateExists = allProducts.Data.Any(p =>
        //    p.ProductId != product.ProductId &&
        //    (p.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase) ||
        //     (!string.IsNullOrEmpty(product.Code) && p.Code == product.Code)));

        //if (duplicateExists)
        //    throw new InvalidOperationException("Duplicate product Name or Code."); // handled via if/else

        // 2️⃣ Call repository/SP (returns bool, no conversion needed)
       var  result = await _productRepository.UpdateProductAsync(product);

        if(result== -1)
        {
            throw new InvalidOperationException("Duplicate product Name or Code."); // handled via if/else
        }
        return result;
    }


    // ✅ Required by interface
    public async Task<int> DeleteProductAsync(int id)
        => await _productRepository.DeleteProductAsync(id);

    // ✅ Required by interface
    public async Task<bool> ProductExistsAsync(int id)
        => await _productRepository.ProductExistsAsync(id);

    // 🔹 New method with full logic (not in interface)
    public async Task<(bool Success, string Message)> DeleteProductLogicAsync(int id)
    {
        if (!await ProductExistsAsync(id))
            return (false, "Product not found.");

        int result = await DeleteProductAsync(id);

        if (result == 1)
            return (true, "Product deleted successfully.");
        else if (result == -1)
            return (false, "Cannot delete product — it may be used in sales records.");
        else
            return (false, "Unexpected error occurred while deleting product.");
    }
}
