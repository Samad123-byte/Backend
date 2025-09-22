using Microsoft.AspNetCore.Mvc;
using Backend.Data.Repositories;
using Backend.Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;

        public SalesController(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
        {
            var sales = await _saleRepository.GetAllSalesAsync();
            return Ok(sales);
        }

        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetSale(int id)
        {
            var sale = await _saleRepository.GetSaleByIdAsync(id);

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
            var sale = await _saleRepository.GetSaleWithDetailsAsync(id);

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
            var sales = await _saleRepository.GetSalesByDateRangeAsync(startDate, endDate);
            return Ok(sales);
        }

        // GET: api/Sales/BySalesperson/5
        [HttpGet("BySalesperson/{salespersonId}")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSalesBySalesperson(int salespersonId)
        {
            var sales = await _saleRepository.GetSalesBySalespersonAsync(salespersonId);
            return Ok(sales);
        }

        // POST: api/Sales
        [HttpPost]
        public async Task<ActionResult<Sale>> CreateSale(Sale sale)
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Set default values if not provided
                if (sale.SaleDate == default(DateTime))
                {
                    sale.SaleDate = DateTime.Now;
                }

                if (sale.UpdatedDate == null)
                {
                    sale.UpdatedDate = DateTime.Now;
                }

                var createdSale = await _saleRepository.CreateSaleAsync(sale);
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

            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if sale exists
            if (!await _saleRepository.SaleExistsAsync(id))
            {
                return NotFound();
            }

            // Update the UpdateDate
            sale.UpdatedDate = DateTime.Now;

            var success = await _saleRepository.UpdateSaleAsync(sale);

            if (!success)
            {
                return BadRequest("Failed to update sale");
            }

            return NoContent();
        }

        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            if (!await _saleRepository.SaleExistsAsync(id))
            {
                return NotFound();
            }

            var success = await _saleRepository.DeleteSaleAsync(id);

            if (!success)
            {
                return BadRequest("Failed to delete sale");
            }

            return NoContent();
        }
    }
}