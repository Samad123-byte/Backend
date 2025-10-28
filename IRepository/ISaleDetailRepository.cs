using Backend.Models;

namespace Backend.IRepository
{
    public interface ISaleDetailRepository
    {
        Task<PaginatedResponse<SaleDetail>> GetAllSaleDetailsAsync(int pageNumber = 1, int pageSize = 10); // ✅ Changed
        Task<SaleDetail?> GetSaleDetailByIdAsync(int id);
        Task<IEnumerable<SaleDetail>> GetSaleDetailsBySaleIdAsync(int saleId);
        Task<SaleDetail> CreateSaleDetailAsync(SaleDetail saleDetail);
        Task<bool> UpdateSaleDetailAsync(SaleDetail saleDetail);
        Task<(bool success, string message)> DeleteSaleDetailAsync(int id);

        Task<bool> DeleteSaleDetailsBySaleIdAsync(int saleId);
        Task<bool> SaleDetailExistsAsync(int id);
        Task<decimal> GetSaleTotalAsync(int saleId);
    }
}
