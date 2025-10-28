
// ✅ ISaleRepository.cs
using Backend.Models;

namespace Backend.IRepository
{
    public interface ISaleRepository
    {
        Task<PaginatedResponse<Sale>> GetAllSalesAsync(int pageNumber, int pageSize);
        Task<Sale?> GetSaleByIdAsync(int id);
        Task<Sale> CreateSaleAsync(Sale sale);
        Task<bool> UpdateSaleAsync(Sale sale);
        Task<(bool success, string message)> DeleteSaleAsync(int id); // ✅ Changed return type
        Task<bool> SaleExistsAsync(int id);
        Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Sale>> GetSalesBySalespersonAsync(int salespersonId);
        Task<Sale?> GetSaleWithDetailsAsync(int id);
    }
}