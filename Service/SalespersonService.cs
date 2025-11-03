using Backend.IRepository;
using Backend.IServices;
using Backend.Models;
using Microsoft.Data.SqlClient;

namespace Backend.Service
{
    public class SalespersonService : ISalespersonService
    {
        private readonly ISalespersonRepository _salespersonRepository;

        public SalespersonService(ISalespersonRepository salespersonRepository)
        {
            _salespersonRepository = salespersonRepository;
        }

        public async Task<PaginatedResponse<Salesperson>> GetAllSalespersonsAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _salespersonRepository.GetAllSalespersonsAsync(pageNumber, pageSize);
        }

        public async Task<Salesperson?> GetSalespersonByIdAsync(int id)
        {
            return await _salespersonRepository.GetSalespersonByIdAsync(id);
        }

        public async Task<Salesperson?> GetSalespersonByCodeAsync(string code)
        {
            return await _salespersonRepository.GetSalespersonByCodeAsync(code);
        }

        public async Task<IEnumerable<Salesperson>> GetActiveSalespersonsAsync()
        {
            return await _salespersonRepository.GetActiveSalespersonsAsync();
        }

        public async Task<Salesperson> CreateSalespersonAsync(Salesperson salesperson)
        {
            var created = await _salespersonRepository.CreateSalespersonAsync(salesperson);

            if (created != null)
            {
                return created;
            }
            else
            {
                throw new InvalidOperationException("Duplicate Name or Code");
            }
        }

        public async Task<int> UpdateSalespersonAsync(Salesperson salesperson)
        {
            var result = await _salespersonRepository.UpdateSalespersonAsync(salesperson);

            if (result == -1)
            {
                throw new InvalidOperationException("Duplicate Name or Code.");
            }
            else if (result == 0)
            {
                throw new InvalidOperationException("Update failed — salesperson not found.");
            }

            return result;
        }



        // Implements repository interface method
        public async Task<int> DeleteSalespersonAsync(int id)
        {
            return await _salespersonRepository.DeleteSalespersonAsync(id);
        }

        // Handles deletion logic and maps codes to messages
        public async Task<(bool Success, string Message)> DeleteSalespersonLogicAsync(int id)
        {
            if (!await SalespersonExistsAsync(id))
                return (false, "Salesperson not found.");

            int result = await DeleteSalespersonAsync(id);

            // Map result codes to messages
            if (result == 1) return (true, "Salesperson deleted successfully.");
            if (result == -1) return (false, "Salesperson not found.");
            if (result == -2) return (false, "Cannot delete salesperson — it has related sales records.");

            return (false, "Unexpected error occurred while deleting salesperson.");
        }

        // Checks if salesperson exists
        public async Task<bool> SalespersonExistsAsync(int id)
        {
            return await _salespersonRepository.SalespersonExistsAsync(id);
        }

    }
}
