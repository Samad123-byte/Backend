using Backend.Models;

namespace Backend.IServices
{
    public interface ISalespersonService
    {
        Task<PaginatedResponse<Salesperson>> GetAllSalespersonsAsync(int pageNumber = 1, int pageSize = 10); // ✅ Changed
        Task<Salesperson?> GetSalespersonByIdAsync(int id);
        Task<Salesperson?> GetSalespersonByCodeAsync(string code);
        Task<IEnumerable<Salesperson>> GetActiveSalespersonsAsync();
        Task<Salesperson> CreateSalespersonAsync(Salesperson salesperson);
        Task<bool> UpdateSalespersonAsync(Salesperson salesperson);
        Task<bool> DeleteSalespersonAsync(int id);
        Task<bool> SalespersonExistsAsync(int id);
    }
}