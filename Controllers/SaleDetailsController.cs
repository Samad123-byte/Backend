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

        // ✅ GET: api/SaleDetails/getall
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var saleDetails = await _saleDetailService.GetAllSaleDetailsAsync();
            return Ok(new
            {
                success = true,
                message = "Fetched all sale details successfully.",
                data = saleDetails
            });
        }

        // ✅ GET: api/SaleDetails/getbyid/{id}
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var saleDetail = await _saleDetailService.GetSaleDetailByIdAsync(id);
            if (saleDetail == null)
                return NotFound(new
                {
                    success = false,
                    message = $"Sale detail with ID {id} not found."
                });

            return Ok(new
            {
                success = true,
                message = "Fetched sale detail successfully.",
                data = saleDetail
            });
        }

        // ✅ GET: api/SaleDetails/bysale/{saleId}
        [HttpGet("bysale/{saleId}")]
        public async Task<IActionResult> GetBySaleId(int saleId)
        {
            var saleDetails = await _saleDetailService.GetSaleDetailsBySaleIdAsync(saleId);

            // ✅ Always return plain array — avoids frontend map() error
            if (saleDetails == null || !saleDetails.Any())
                return Ok(new List<SaleDetail>()); // return empty array instead of null

            return Ok(saleDetails);
        }

        // ✅ GET: api/SaleDetails/total/{saleId}
        [HttpGet("total/{saleId}")]
        public async Task<IActionResult> GetTotal(int saleId)
        {
            var total = await _saleDetailService.GetSaleTotalAsync(saleId);
            return Ok(new
            {
                success = true,
                message = $"Fetched total for Sale ID {saleId}.",
                total = total
            });
        }

        // ✅ POST: api/SaleDetails/add
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] SaleDetail saleDetail)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data submitted.",
                    errors = ModelState
                });

            var created = await _saleDetailService.CreateSaleDetailAsync(saleDetail);
            return Ok(new
            {
                success = true,
                message = "Sale detail added successfully.",
                data = created
            });
        }

        // ✅ POST: api/SaleDetails/update
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] SaleDetail saleDetail)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data submitted.",
                    errors = ModelState
                });

            var exists = await _saleDetailService.SaleDetailExistsAsync(saleDetail.SaleDetailId);
            if (!exists)
                return NotFound(new
                {
                    success = false,
                    message = $"Sale detail with ID {saleDetail.SaleDetailId} not found."
                });

            var success = await _saleDetailService.UpdateSaleDetailAsync(saleDetail);
            return success
                ? Ok(new { success = true, message = "Sale detail updated successfully." })
                : BadRequest(new { success = false, message = "Failed to update sale detail." });
        }

        // ✅ DELETE: api/SaleDetails/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _saleDetailService.SaleDetailExistsAsync(id);
            if (!exists)
                return NotFound(new
                {
                    success = false,
                    message = $"Sale detail with ID {id} not found."
                });

            var success = await _saleDetailService.DeleteSaleDetailAsync(id);
            return success
                ? Ok(new { success = true, message = "Sale detail deleted successfully." })
                : BadRequest(new { success = false, message = "Failed to delete sale detail." });
        }

        // ✅ DELETE: api/SaleDetails/deleteBySale/{saleId}
        [HttpDelete("deleteBySale/{saleId}")]
        public async Task<IActionResult> DeleteBySale(int saleId)
        {
            var success = await _saleDetailService.DeleteSaleDetailsBySaleIdAsync(saleId);
            return success
                ? Ok(new { success = true, message = "Sale details deleted successfully." })
                : BadRequest(new { success = false, message = "Failed to delete sale details." });
        }

        // ✅ POST: api/SaleDetails/batch
        [HttpPost("batch")]
        public async Task<IActionResult> AddBatch([FromBody] IEnumerable<SaleDetail> saleDetails)
        {
            if (saleDetails == null || !saleDetails.Any())
                return BadRequest(new { success = false, message = "No sale details provided." });

            try
            {
                var createdSaleDetails = new List<SaleDetail>();

                foreach (var saleDetail in saleDetails)
                {
                    var created = await _saleDetailService.CreateSaleDetailAsync(saleDetail);
                    createdSaleDetails.Add(created);
                }

                return Ok(new
                {
                    success = true,
                    message = "Batch sale details created successfully.",
                    data = createdSaleDetails
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error creating sale details: {ex.Message}"
                });
            }
        }
    }
}
