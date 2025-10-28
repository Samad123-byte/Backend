using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.IServices;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        // GET: api/Sales
        
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<Sale>>> GetSales(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var sales = await _saleService.GetAllSalesAsync(pageNumber, pageSize);
            return Ok(sales);
        }

        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetSale(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);

            if (sale == null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        // GET: api/Sales/5/WithDetails
        [HttpGet("{id}/WithDetails")]
        public async Task<ActionResult<Sale>> GetSaleWithDetails(int id)
        {
            var sale = await _saleService.GetSaleWithDetailsAsync(id);

            if (sale == null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        // GET: api/Sales/ByDateRange?startDate=2024-01-01&endDate=2024-12-31
        [HttpGet("ByDateRange")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSalesByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var sales = await _saleService.GetSalesByDateRangeAsync(startDate, endDate);
            return Ok(sales);
        }

        // GET: api/Sales/BySalesperson/5
        [HttpGet("BySalesperson/{salespersonId}")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSalesBySalesperson(int salespersonId)
        {
            var sales = await _saleService.GetSalesBySalespersonAsync(salespersonId);
            return Ok(sales);
        }

        // POST: api/Sales
        [HttpPost]
        public async Task<ActionResult<Sale>> CreateSale(Sale sale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (sale.SaleDate == default(DateTime))
                {
                    sale.SaleDate = DateTime.Now;
                }

                sale.UpdatedDate = null;

                var createdSale = await _saleService.CreateSaleAsync(sale);
                return CreatedAtAction(nameof(GetSale), new { id = createdSale.SaleId }, createdSale);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating sale: {ex.Message}");
            }
        }

        // PUT: api/Sales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, Sale sale)
        {
            if (id != sale.SaleId)
            {
                return BadRequest("Sale ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingSale = await _saleService.GetSaleByIdAsync(id);
            if (existingSale == null)
            {
                return NotFound();
            }

            var success = await _saleService.UpdateSaleAsync(sale);

            if (!success)
            {
                return BadRequest("Failed to update sale. The sale may have related sale details or other dependencies that prevent modification.");
            }

            return NoContent();
        }

        // ✅ FIXED: DELETE with proper error handling
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            try
            {
                var existingSale = await _saleService.GetSaleByIdAsync(id);
                if (existingSale == null)
                {
                    return NotFound(new { success = false, message = "Sale not found." });
                }

                var (success, message) = await _saleService.DeleteSaleAsync(id);

                if (!success)
                {
                    return BadRequest(new { success = false, message });
                }

                return Ok(new { success = true, message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }
    }
}
