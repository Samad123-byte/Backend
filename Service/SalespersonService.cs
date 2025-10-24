using Backend.IRepository;
using Backend.IServices;
using Backend.Models;

namespace Backend.Service
{
    public class SalespersonService : ISalespersonService
    {
        private readonly ISalespersonRepository _salespersonRepository;

        public SalespersonService(ISalespersonRepository salespersonRepository)
        {
            _salespersonRepository = salespersonRepository;
        }

        public async Task<IEnumerable<Salesperson>> GetAllSalespersonsAsync()
        {
            return await _salespersonRepository.GetAllSalespersonsAsync();
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
            return await _salespersonRepository.CreateSalespersonAsync(salesperson);
        }

        public async Task<bool> UpdateSalespersonAsync(Salesperson salesperson)
        {
            return await _salespersonRepository.UpdateSalespersonAsync(salesperson);
        }

        public async Task<bool> DeleteSalespersonAsync(int id)
        {
            return await _salespersonRepository.DeleteSalespersonAsync(id);
        }

        public async Task<bool> SalespersonExistsAsync(int id)
        {
            return await _salespersonRepository.SalespersonExistsAsync(id);
        }

    }
}
