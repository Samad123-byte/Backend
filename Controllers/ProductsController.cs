using Backend.IServices;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;


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
        public async Task<ActionResult<PaginatedResponse<Product>>> GetProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetAllProductsAsync(pageNumber, pageSize);
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdProduct = await _productService.CreateProductAsync(product);
                return Ok(new { success = true, message = "Product created successfully", data = createdProduct });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        // ✅ POST: api/Products/update
        [HttpPost("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _productService.UpdateProductAsync(product);

           
                //return BadRequest("Failed to update product.");

            return Ok("Product updated successfully.");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            var service = (ProductService)_productService; // cast to concrete service
            var result = await service.DeleteProductLogicAsync(request.Id);

            if (result.Success)
                return Ok(new { success = true, message = result.Message });
            else if (result.Message.Contains("used in sales"))
                return BadRequest(new { success = false, message = result.Message });
            else if (result.Message.Contains("not found"))
                return NotFound(new { success = false, message = result.Message });
            else
                return BadRequest(new { success = false, message = result.Message });
        }
    }

}
