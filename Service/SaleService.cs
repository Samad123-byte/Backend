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

        public async Task<PaginatedResponse<Sale>> GetAllSalesAsync(int pageNumber, int pageSize)
        {
            return await _saleRepository.GetAllSalesAsync(pageNumber, pageSize);
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _saleRepository.GetSaleByIdAsync(id);
        }

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            // The repository method already handles Sale + SaleDetails in one go
            var createdSale = await _saleRepository.CreateSaleAsync(sale);
            if (createdSale == null)
                throw new InvalidOperationException("Failed to create sale.");

            return createdSale;
        }

        public async Task<bool> UpdateSaleAsync(Sale sale)
        {
            // The repository method handles updating Sale + SaleDetails via SP
            return await _saleRepository.UpdateSaleAsync(sale);
        }

        public async Task<(bool success, string message)> DeleteSaleAsync(int id)
        {
            if (!await SaleExistsAsync(id))
            {
                return (false, "Sale not found");
            }

            try
            {
                int result = await _saleRepository.DeleteSaleAsync(id);
                return (true, "Sale deleted successfully");
            }
            catch (Exception ex)
            {
                // Return the actual DB exception (FK violation, etc.)
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }



        public async Task<bool> SaleExistsAsync(int id)
        {
            return await _saleRepository.SaleExistsAsync(id);
        }

        public async Task<Sale?> DeleteSaleDetailAsync(int saleId, int productId)
        {
            // Check if sale exists first
            if (!await SaleExistsAsync(saleId))
                return null;

            // Delete the sale item via repository
            var updatedSale = await _saleRepository.DeleteSaleDetailAsync(saleId, productId);

            return updatedSale;
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
