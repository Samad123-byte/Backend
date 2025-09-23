// SaleDetailsController.cs
using Microsoft.AspNetCore.Mvc;
using Backend.Data.Repositories;
using Backend.Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleDetailsController : ControllerBase
    {
        private readonly ISaleDetailRepository _saleDetailRepository;

        public SaleDetailsController(ISaleDetailRepository saleDetailRepository)
        {
            _saleDetailRepository = saleDetailRepository;
        }

        // GET: api/SaleDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetail>>> GetSaleDetails()
        {
            var saleDetails = await _saleDetailRepository.GetAllSaleDetailsAsync();
            return Ok(saleDetails);
        }

        // GET: api/SaleDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDetail>> GetSaleDetail(int id)
        {
            var saleDetail = await _saleDetailRepository.GetSaleDetailByIdAsync(id);

            if (saleDetail == null)
            {
                return NotFound();
            }

            return Ok(saleDetail);
        }

        // GET: api/SaleDetails/BySale/5
        [HttpGet("BySale/{saleId}")]
        public async Task<ActionResult<IEnumerable<SaleDetail>>> GetSaleDetailsBySaleId(int saleId)
        {
            var saleDetails = await _saleDetailRepository.GetSaleDetailsBySaleIdAsync(saleId);
            return Ok(saleDetails);
        }

        // GET: api/SaleDetails/Total/5
        [HttpGet("Total/{saleId}")]
        public async Task<ActionResult<decimal>> GetSaleTotal(int saleId)
        {
            var total = await _saleDetailRepository.GetSaleTotalAsync(saleId);
            return Ok(total);
        }

        // POST: api/SaleDetails
        [HttpPost]
        public async Task<ActionResult<SaleDetail>> CreateSaleDetail(SaleDetail saleDetail)
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSaleDetail = await _saleDetailRepository.CreateSaleDetailAsync(saleDetail);
                return CreatedAtAction(nameof(GetSaleDetail), new { id = createdSaleDetail.SaleDetailId }, createdSaleDetail);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating sale detail: {ex.Message}");
            }
        }

        // POST: api/SaleDetails/Batch
        [HttpPost("Batch")]
        public async Task<ActionResult<IEnumerable<SaleDetail>>> CreateMultipleSaleDetails(IEnumerable<SaleDetail> saleDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdSaleDetails = new List<SaleDetail>();

            try
            {
                foreach (var saleDetail in saleDetails)
                {
                    var createdSaleDetail = await _saleDetailRepository.CreateSaleDetailAsync(saleDetail);
                    createdSaleDetails.Add(createdSaleDetail);
                }

                return Ok(createdSaleDetails);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating sale details: {ex.Message}");
            }
        }

        // PUT: api/SaleDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSaleDetail(int id, SaleDetail saleDetail)
        {
            if (id != saleDetail.SaleDetailId)
            {
                return BadRequest("Sale Detail ID mismatch");
            }

            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if sale detail exists
            if (!await _saleDetailRepository.SaleDetailExistsAsync(id))
            {
                return NotFound();
            }

            var success = await _saleDetailRepository.UpdateSaleDetailAsync(saleDetail);

            if (!success)
            {
                return BadRequest("Failed to update sale detail. The sale detail may be referenced by other records or does not exist.");
            }

            return NoContent();
        }

        // DELETE: api/SaleDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleDetail(int id)
        {
            if (!await _saleDetailRepository.SaleDetailExistsAsync(id))
            {
                return NotFound();
            }

            var success = await _saleDetailRepository.DeleteSaleDetailAsync(id);

            if (!success)
            {
                return BadRequest("Cannot delete sale detail. This record may be referenced by other transactions in the database.");
            }

            return NoContent();
        }

        // DELETE: api/SaleDetails/BySale/5
        [HttpDelete("BySale/{saleId}")]
        public async Task<IActionResult> DeleteSaleDetailsBySaleId(int saleId)
        {
            var success = await _saleDetailRepository.DeleteSaleDetailsBySaleIdAsync(saleId);

            if (!success)
            {
                return BadRequest("Cannot delete sale details. These records may be referenced by other transactions in the database.");
            }

            return NoContent();
        }
    }
}