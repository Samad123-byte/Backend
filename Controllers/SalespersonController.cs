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

        // GET: api/Salesperson
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Salesperson>>> GetSalespersons()
        {
            var salespersons = await _salespersonService.GetAllSalespersonsAsync();
            return Ok(salespersons);
        }

        // GET: api/Salesperson/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Salesperson>> GetSalesperson(int id)
        {
            var salesperson = await _salespersonService.GetSalespersonByIdAsync(id);

            if (salesperson == null)
            {
                return NotFound();
            }

            return Ok(salesperson);
        }

        // GET: api/Salesperson/ByCode/SP001
        [HttpGet("ByCode/{code}")]
        public async Task<ActionResult<Salesperson>> GetSalespersonByCode(string code)
        {
            var salesperson = await _salespersonService.GetSalespersonByCodeAsync(code);

            if (salesperson == null)
            {
                return NotFound();
            }

            return Ok(salesperson);
        }

        // GET: api/Salesperson/Active
        [HttpGet("Active")]
        public async Task<ActionResult<IEnumerable<Salesperson>>> GetActiveSalespersons()
        {
            var salespersons = await _salespersonService.GetActiveSalespersonsAsync();
            return Ok(salespersons);
        }

        // POST: api/Salesperson
        [HttpPost]
        public async Task<ActionResult<Salesperson>> CreateSalesperson(Salesperson salesperson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (salesperson.EnteredDate == null)
                {
                    salesperson.EnteredDate = DateTime.Now;
                }

                var createdSalesperson = await _salespersonService.CreateSalespersonAsync(salesperson);
                return CreatedAtAction(nameof(GetSalesperson), new { id = createdSalesperson.SalespersonId }, createdSalesperson);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating salesperson: {ex.Message}");
            }
        }

        // PUT: api/Salesperson/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalesperson(int id, Salesperson salesperson)
        {
            if (id != salesperson.SalespersonId)
            {
                return BadRequest("Salesperson ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _salespersonService.SalespersonExistsAsync(id))
            {
                return NotFound();
            }

            var success = await _salespersonService.UpdateSalespersonAsync(salesperson);

            if (!success)
            {
                return BadRequest("Failed to update salesperson. They may have associated sales or dependencies.");
            }

            return NoContent();
        }

        // DELETE: api/Salesperson/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesperson(int id)
        {
            if (!await _salespersonService.SalespersonExistsAsync(id))
            {
                return NotFound();
            }

            var success = await _salespersonService.DeleteSalespersonAsync(id);

            if (!success)
            {
                return BadRequest("Cannot delete salesperson. They have associated sales records.");
            }

            return NoContent();
        }
    }
}
