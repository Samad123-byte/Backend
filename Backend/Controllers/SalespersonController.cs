using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.IServices;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalespersonController : ControllerBase
    {
        private readonly ISalespersonService _salespersonService;

        public SalespersonController(ISalespersonService salespersonService)
        {
            _salespersonService = salespersonService;
        }

        // ✅ GET: api/Salesperson/getall
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var salespersons = await _salespersonService.GetAllSalespersonsAsync(pageNumber, pageSize);
            return Ok(salespersons);
        }

        // ✅ GET: api/Salesperson/getbyid/{id}
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var salesperson = await _salespersonService.GetSalespersonByIdAsync(id);
            if (salesperson == null) return NotFound();
            return Ok(salesperson);
        }

        // ✅ POST: api/Salesperson/add
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] Salesperson salesperson)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _salespersonService.CreateSalespersonAsync(salesperson);
            return Ok(created);
        }

        // ✅ POST: api/Salesperson/update
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Salesperson salesperson)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _salespersonService.SalespersonExistsAsync(salesperson.SalespersonId);
            if (!exists) return NotFound();

            var success = await _salespersonService.UpdateSalespersonAsync(salesperson);
            return success ? Ok("Salesperson updated successfully.") : BadRequest("Failed to update salesperson.");
        }

        // ✅ POST: api/Salesperson/delete
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            try
            {
                var exists = await _salespersonService.SalespersonExistsAsync(request.Id);
                if (!exists) return NotFound(new { success = false, message = "Salesperson not found." });

                var success = await _salespersonService.DeleteSalespersonAsync(request.Id);

                if (!success)
                    return BadRequest(new { success = false, message = "Failed to delete salesperson." });

                return Ok(new { success = true, message = "Salesperson deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
            }
        }
    }

    // Request model for delete endpoint
    public class DeleteRequest
    {
        public int Id { get; set; }
    }
}