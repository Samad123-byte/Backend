using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.IServices;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleDetailsController : ControllerBase
    {
        private readonly ISaleDetailService _saleDetailService;

        public SaleDetailsController(ISaleDetailService saleDetailService)
        {
            _saleDetailService = saleDetailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetail>>> GetSaleDetails()
        {
            var saleDetails = await _saleDetailService.GetAllSaleDetailsAsync();
            return Ok(saleDetails);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDetail>> GetSaleDetail(int id)
        {
            var saleDetail = await _saleDetailService.GetSaleDetailByIdAsync(id);

            if (saleDetail == null)
            {
                return NotFound();
            }

            return Ok(saleDetail);
        }

        [HttpGet("BySale/{saleId}")]
        public async Task<ActionResult<IEnumerable<SaleDetail>>> GetSaleDetailsBySaleId(int saleId)
        {
            var saleDetails = await _saleDetailService.GetSaleDetailsBySaleIdAsync(saleId);
            return Ok(saleDetails);
        }

        [HttpGet("Total/{saleId}")]
        public async Task<ActionResult<decimal>> GetSaleTotal(int saleId)
        {
            var total = await _saleDetailService.GetSaleTotalAsync(saleId);
            return Ok(total);
        }

        [HttpPost]
        public async Task<ActionResult<SaleDetail>> CreateSaleDetail(SaleDetail saleDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSaleDetail = await _saleDetailService.CreateSaleDetailAsync(saleDetail);
                return CreatedAtAction(nameof(GetSaleDetail), new { id = createdSaleDetail.SaleDetailId }, createdSaleDetail);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating sale detail: {ex.Message}");
            }
        }

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
                    var createdSaleDetail = await _saleDetailService.CreateSaleDetailAsync(saleDetail);
                    createdSaleDetails.Add(createdSaleDetail);
                }

                return Ok(createdSaleDetails);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating sale details: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSaleDetail(int id, SaleDetail saleDetail)
        {
            if (id != saleDetail.SaleDetailId)
            {
                return BadRequest("Sale Detail ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _saleDetailService.SaleDetailExistsAsync(id))
            {
                return NotFound();
            }

            var success = await _saleDetailService.UpdateSaleDetailAsync(saleDetail);

            if (!success)
            {
                return BadRequest("Failed to update sale detail.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleDetail(int id)
        {
            if (!await _saleDetailService.SaleDetailExistsAsync(id))
            {
                return NotFound();
            }

            var success = await _saleDetailService.DeleteSaleDetailAsync(id);

            if (!success)
            {
                return BadRequest("Cannot delete sale detail.");
            }

            return NoContent();
        }

        [HttpDelete("BySale/{saleId}")]
        public async Task<IActionResult> DeleteSaleDetailsBySaleId(int saleId)
        {
            var success = await _saleDetailService.DeleteSaleDetailsBySaleIdAsync(saleId);

            if (!success)
            {
                return BadRequest("Cannot delete sale details.");
            }

            return NoContent();
        }
    }
}
