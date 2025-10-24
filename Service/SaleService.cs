using Backend.IRepository;
using Backend.IServices;
using Backend.Models;
using Microsoft.Data.SqlClient;

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
            try
            {
                return await _saleRepository.CreateSaleAsync(sale);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    547 => new InvalidOperationException("Cannot create sale. The referenced salesperson does not exist."),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
        }

        public async Task<bool> UpdateSaleAsync(Sale sale)
        {
            try
            {
                return await _saleRepository.UpdateSaleAsync(sale);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    547 => new InvalidOperationException("Cannot update sale. The referenced salesperson does not exist or sale has dependencies."),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
        }

        public async Task<bool> DeleteSaleAsync(int id)
        {
            try
            {
                return await _saleRepository.DeleteSaleAsync(id);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    547 => new InvalidOperationException("Cannot delete sale. This sale has associated sale details. Please delete related sale details first."),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
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
