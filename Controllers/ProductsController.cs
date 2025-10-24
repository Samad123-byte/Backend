using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.IServices;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // ✅ GET: api/Products/getAll
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // ✅ GET: api/Products/getById/{id}
        [HttpGet("getById/{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // ✅ POST: api/Products/create
        [HttpPost("create")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdProduct = await _productService.CreateProductAsync(product);
                return Ok(createdProduct);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating product: {ex.Message}");
            }
        }

        // ✅ POST: api/Products/update
        [HttpPost("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _productService.ProductExistsAsync(product.ProductId))
                return NotFound();

            var success = await _productService.UpdateProductAsync(product);

            if (!success)
                return BadRequest("Failed to update product.");

            return Ok("Product updated successfully.");
        }

        // ✅ POST: api/Products/delete
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteProduct([FromBody] int id)
        {
            if (!await _productService.ProductExistsAsync(id))
                return NotFound();

            var success = await _productService.DeleteProductAsync(id);
            if (!success)
                return BadRequest("Cannot delete product. It may be referenced elsewhere.");

            return Ok("Product deleted successfully.");
        }
    }
}
