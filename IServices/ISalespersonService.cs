using Backend.Models;

namespace Backend.IServices
{
    public interface ISalespersonService
    {
        Task<IEnumerable<Salesperson>> GetAllSalespersonsAsync();
        Task<Salesperson?> GetSalespersonByIdAsync(int id);
        Task<Salesperson?> GetSalespersonByCodeAsync(string code);
        Task<IEnumerable<Salesperson>> GetActiveSalespersonsAsync();
        Task<Salesperson> CreateSalespersonAsync(Salesperson salesperson);
        Task<bool> UpdateSalespersonAsync(Salesperson salesperson);
        Task<bool> DeleteSalespersonAsync(int id);

        Task<bool> SalespersonExistsAsync(int id);
    }
}
