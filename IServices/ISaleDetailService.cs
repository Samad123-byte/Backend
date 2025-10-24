using Backend.Models;

namespace Backend.IServices
{
    public interface ISaleDetailService
    {
        Task<IEnumerable<SaleDetail>> GetAllSaleDetailsAsync();
        Task<SaleDetail?> GetSaleDetailByIdAsync(int id);
        Task<IEnumerable<SaleDetail>> GetSaleDetailsBySaleIdAsync(int saleId);
        Task<SaleDetail> CreateSaleDetailAsync(SaleDetail saleDetail);
        Task<bool> UpdateSaleDetailAsync(SaleDetail saleDetail);
        Task<bool> DeleteSaleDetailAsync(int id);
        Task<bool> DeleteSaleDetailsBySaleIdAsync(int saleId);
        Task<decimal> GetSaleTotalAsync(int saleId);
        Task<bool> SaleDetailExistsAsync(int id);

    }
}
