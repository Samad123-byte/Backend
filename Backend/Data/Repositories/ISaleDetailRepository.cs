using Backend.Models;

namespace Backend.Data.Repositories
{
    public interface ISaleDetailRepository
    {
        Task<IEnumerable<SaleDetail>> GetAllSaleDetailsAsync();
        Task<SaleDetail?> GetSaleDetailByIdAsync(int id);
        Task<IEnumerable<SaleDetail>> GetSaleDetailsBySaleIdAsync(int saleId);
        Task<SaleDetail> CreateSaleDetailAsync(SaleDetail saleDetail);
        Task<bool> UpdateSaleDetailAsync(SaleDetail saleDetail);
        Task<bool> DeleteSaleDetailAsync(int id);
        Task<bool> DeleteSaleDetailsBySaleIdAsync(int saleId);
        Task<bool> SaleDetailExistsAsync(int id);
        Task<decimal> GetSaleTotalAsync(int saleId);
    }
}