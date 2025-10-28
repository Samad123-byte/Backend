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
            try
            {
                return await _salespersonRepository.CreateSalespersonAsync(salesperson);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    2627 => new InvalidOperationException("A salesperson with this code already exists"),
                    2601 => new InvalidOperationException("Duplicate salesperson code"),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
        }

        public async Task<bool> UpdateSalespersonAsync(Salesperson salesperson)
        {
            try
            {
                return await _salespersonRepository.UpdateSalespersonAsync(salesperson);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    2627 => new InvalidOperationException("A salesperson with this code already exists"),
                    2601 => new InvalidOperationException("Duplicate salesperson code"),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
        }


        public async Task<bool> DeleteSalespersonAsync(int id)
        {
            try
            {
                return await _salespersonRepository.DeleteSalespersonAsync(id);
            }
            catch (SqlException ex)
            {
                // Error 547 is foreign key constraint violation
                if (ex.Number == 547)
                {
                    throw new InvalidOperationException(
                        "Cannot delete salesperson. This salesperson has associated sales records in the database. " +
                        "Please reassign or delete the sales first before removing this salesperson."
                    );
                }

                // For other SQL errors, wrap them
                throw new InvalidOperationException($"Database error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Don't swallow other exceptions
                throw new InvalidOperationException($"Error deleting salesperson: {ex.Message}", ex);
            }
        }
        public async Task<bool> SalespersonExistsAsync(int id)
        {
            return await _salespersonRepository.SalespersonExistsAsync(id);
        }
    }
}
