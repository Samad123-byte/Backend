using Backend.IRepository;
using Backend.IServices;
using Backend.Models;
using Microsoft.Data.SqlClient;

namespace Backend.Service
{
    public class SaleDetailService : ISaleDetailService
    {
        private readonly ISaleDetailRepository _saleDetailRepository;

        public SaleDetailService(ISaleDetailRepository saleDetailRepository)
        {
            _saleDetailRepository = saleDetailRepository;
        }

        public async Task<PaginatedResponse<SaleDetail>> GetAllSaleDetailsAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _saleDetailRepository.GetAllSaleDetailsAsync(pageNumber, pageSize);
        }

        public async Task<SaleDetail?> GetSaleDetailByIdAsync(int id)
        {
            return await _saleDetailRepository.GetSaleDetailByIdAsync(id);
        }

        public async Task<IEnumerable<SaleDetail>> GetSaleDetailsBySaleIdAsync(int saleId)
        {
            return await _saleDetailRepository.GetSaleDetailsBySaleIdAsync(saleId);
        }

        public async Task<SaleDetail> CreateSaleDetailAsync(SaleDetail saleDetail)
        {
            try
            {
                return await _saleDetailRepository.CreateSaleDetailAsync(saleDetail);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    547 => new InvalidOperationException("Cannot create sale detail. The referenced sale or product does not exist."),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
        }

        public async Task<bool> UpdateSaleDetailAsync(SaleDetail saleDetail)
        {
            try
            {
                return await _saleDetailRepository.UpdateSaleDetailAsync(saleDetail);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    547 => new InvalidOperationException("Cannot update sale detail. The referenced sale or product does not exist."),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
        }

        public async Task<(bool success, string message)> DeleteSaleDetailAsync(int id)
        {
            return await _saleDetailRepository.DeleteSaleDetailAsync(id);
        }


        public async Task<bool> DeleteSaleDetailsBySaleIdAsync(int saleId)
        {
            try
            {
                return await _saleDetailRepository.DeleteSaleDetailsBySaleIdAsync(saleId);
            }
            catch (SqlException ex)
            {
                throw ex.Number switch
                {
                    547 => new InvalidOperationException("Cannot delete sale details. These records are referenced by other records in the database."),
                    _ => new InvalidOperationException($"Database error: {ex.Message}")
                };
            }
        }

        public async Task<decimal> GetSaleTotalAsync(int saleId)
        {
            return await _saleDetailRepository.GetSaleTotalAsync(saleId);
        }

        public async Task<bool> SaleDetailExistsAsync(int id)
        {
            return await _saleDetailRepository.SaleDetailExistsAsync(id);
        }
    }
}