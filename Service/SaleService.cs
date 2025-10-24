using Backend.IRepository;
using Backend.IServices;
using Backend.Models;

namespace Backend.Service
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;

        public SaleService(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<IEnumerable<Sale>> GetAllSalesAsync()
        {
            return await _saleRepository.GetAllSalesAsync();
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _saleRepository.GetSaleByIdAsync(id);
        }

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            return await _saleRepository.CreateSaleAsync(sale);
        }

        public async Task<bool> UpdateSaleAsync(Sale sale)
        {
            return await _saleRepository.UpdateSaleAsync(sale);
        }

        public async Task<bool> DeleteSaleAsync(int id)
        {
            return await _saleRepository.DeleteSaleAsync(id);
        }

        public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _saleRepository.GetSalesByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<Sale>> GetSalesBySalespersonAsync(int salespersonId)
        {
            return await _saleRepository.GetSalesBySalespersonAsync(salespersonId);
        }

        public async Task<Sale?> GetSaleWithDetailsAsync(int id)
        {
            return await _saleRepository.GetSaleWithDetailsAsync(id);
        }
    }
}
