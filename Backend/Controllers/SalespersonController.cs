
// SalespersonController.cs
using Microsoft.AspNetCore.Mvc;
using Backend.Data.Repositories;
using Backend.Models;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalespersonController : ControllerBase
    {
        private readonly ISalespersonRepository _salespersonRepository;

        public SalespersonController(ISalespersonRepository salespersonRepository)
        {
            _salespersonRepository = salespersonRepository;
        }

        // GET: api/Salesperson
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Salesperson>>> GetSalespersons()
        {
            var salespersons = await _salespersonRepository.GetAllSalespersonsAsync();
            return Ok(salespersons);
        }

        // GET: api/Salesperson/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Salesperson>> GetSalesperson(int id)
        {
            var salesperson = await _salespersonRepository.GetSalespersonByIdAsync(id);

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
            var salesperson = await _salespersonRepository.GetSalespersonByCodeAsync(code);

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
            var salespersons = await _salespersonRepository.GetActiveSalespersonsAsync();
            return Ok(salespersons);
        }

        // POST: api/Salesperson
        [HttpPost]
        public async Task<ActionResult<Salesperson>> CreateSalesperson(Salesperson salesperson)
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Set default values if not provided
                if (salesperson.EnteredDate == null)
                {
                    salesperson.EnteredDate = DateTime.Now;
                }

                var createdSalesperson = await _salespersonRepository.CreateSalespersonAsync(salesperson);
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

            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if salesperson exists
            if (!await _salespersonRepository.SalespersonExistsAsync(id))
            {
                return NotFound();
            }

            var success = await _salespersonRepository.UpdateSalespersonAsync(salesperson);

            if (!success)
            {
                return BadRequest("Failed to update salesperson. The salesperson may have associated sales or other dependencies that prevent modification.");
            }

            return NoContent();
        }

        // DELETE: api/Salesperson/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesperson(int id)
        {
            if (!await _salespersonRepository.SalespersonExistsAsync(id))
            {
                return NotFound();
            }

            var success = await _salespersonRepository.DeleteSalespersonAsync(id);

            if (!success)
            {
                return BadRequest("Cannot delete salesperson. This salesperson has associated sales records in the database. Please reassign or delete the sales first before removing this salesperson.");
            }

            return NoContent();
        }
    }
}
