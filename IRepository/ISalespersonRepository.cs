using Backend.Models;

namespace Backend.IRepository
{
    public interface ISalespersonRepository
    {
        Task<PaginatedResponse<Salesperson>> GetAllSalespersonsAsync(int pageNumber = 1, int pageSize = 10); // ✅ Changed
        Task<Salesperson?> GetSalespersonByIdAsync(int id);
        Task<Salesperson?> GetSalespersonByCodeAsync(string code);
        Task<IEnumerable<Salesperson>> GetActiveSalespersonsAsync();
        Task<Salesperson> CreateSalespersonAsync(Salesperson salesperson);
        Task<int> UpdateSalespersonAsync(Salesperson salesperson);
        Task<int> DeleteSalespersonAsync(int id);

        Task<bool> SalespersonExistsAsync(int id);
    }
}
