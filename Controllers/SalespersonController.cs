using Backend.IServices;
using Backend.Models;
using Backend.Service;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] Salesperson salesperson)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _salespersonService.CreateSalespersonAsync(salesperson);
                return Ok(new { success = true, message = "Salesperson added successfully!", data = created });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Server error: " + ex.Message });
            }
        }




        // ✅ POST: api/Salesperson/update
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Salesperson salesperson)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _salespersonService.UpdateSalespersonAsync(salesperson);

            return Ok("Salesperson updated successfully.");
        }

        // ✅ POST: api/Salesperson/delete
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            // Use the concrete service, not the interface
            var service = (SalespersonService)_salespersonService;
            var (success, message) = await service.DeleteSalespersonLogicAsync(request.Id);

            return success
                ? Ok(new { success, message })
                : BadRequest(new { success, message });
        }



        // Request model for delete endpoint
        public class DeleteRequest
        {
            public int Id { get; set; }
        }
    }
}
