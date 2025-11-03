using Backend.IServices;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetSales([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var sales = await _saleService.GetAllSalesAsync(pageNumber, pageSize);
            return Ok(new
            {
                success = true,
                message = "Fetched sales successfully.",
                data = sales.Data,
                currentPage = sales.CurrentPage,
                pageSize = sales.PageSize,
                totalRecords = sales.TotalRecords,
                totalPages = sales.TotalPages
            });
        }

        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSale(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
                return NotFound(new { success = false, message = $"Sale with ID {id} not found." });

            return Ok(new { success = true, message = "Fetched sale successfully.", data = sale });
        }

        // GET: api/Sales/5/WithDetails
        [HttpGet("{id}/WithDetails")]
        public async Task<IActionResult> GetSaleWithDetails(int id)
        {
            var sale = await _saleService.GetSaleWithDetailsAsync(id);
            if (sale == null)
                return NotFound(new { success = false, message = "Sale not found." });

            return Ok(new
            {
                success = true,
                message = "Fetched sale with details successfully.",
                data = sale
            });
        }

        // GET: api/Sales/ByDateRange?startDate=2024-01-01&endDate=2024-12-31
        [HttpGet("ByDateRange")]
        public async Task<IActionResult> GetSalesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var sales = await _saleService.GetSalesByDateRangeAsync(startDate, endDate);
            return Ok(new { success = true, message = "Fetched sales by date range.", data = sales });
        }

        // GET: api/Sales/BySalesperson/5
        [HttpGet("BySalesperson/{salespersonId}")]
        public async Task<IActionResult> GetSalesBySalesperson(int salespersonId)
        {
            var sales = await _saleService.GetSalesBySalespersonAsync(salespersonId);
            return Ok(new { success = true, message = "Fetched sales by salesperson.", data = sales });
        }

        // POST: api/Sales
        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] Sale sale)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid sale data.", errors = ModelState });

            if (sale.SaleDate == default)
                sale.SaleDate = DateTime.Now;

            sale.UpdatedDate = null;

            var createdSale = await _saleService.CreateSaleAsync(sale);
            return CreatedAtAction(nameof(GetSale), new { id = createdSale.SaleId }, new
            {
                success = true,
                message = "Sale created successfully.",
                data = createdSale
            });
        }

        // PUT: api/Sales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, [FromBody] Sale sale)
        {
            if (id != sale.SaleId)
                return BadRequest(new { success = false, message = "Sale ID mismatch." });

            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid sale data.", errors = ModelState });

            var existingSale = await _saleService.GetSaleByIdAsync(id);
            if (existingSale == null)
                return NotFound(new { success = false, message = "Sale not found." });

            var success = await _saleService.UpdateSaleAsync(sale);
            if (!success)
                return BadRequest(new { success = false, message = "Failed to update sale." });

            return Ok(new { success = true, message = "Sale updated successfully." });
        }

        // DELETE: api/Sales/DeleteItem
        [HttpPost("DeleteItem")]
        public async Task<IActionResult> DeleteSaleItem([FromBody] Sale sale)
        {
            if (sale.SaleId <= 0 || sale.SaleDetails == null || sale.SaleDetails.Count == 0)
                return BadRequest(new { success = false, message = "Invalid SaleId or ProductId." });

            // Assuming deleting the first product in SaleDetails
            var productId = sale.SaleDetails[0].ProductId;

            var updatedSale = await _saleService.DeleteSaleDetailAsync(sale.SaleId, productId);
            if (updatedSale == null)
                return BadRequest(new { success = false, message = "Sale not found or item could not be deleted." });

            return Ok(new
            {
                success = true,
                message = "Sale item deleted successfully.",
                data = updatedSale
            });
        }
        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var existingSale = await _saleService.GetSaleByIdAsync(id);
            if (existingSale == null)
                return NotFound(new { success = false, message = "Sale not found." });

            var (success, message) = await _saleService.DeleteSaleAsync(id);
            return success ? Ok(new { success, message }) : BadRequest(new { success, message });
        }
    }
}
