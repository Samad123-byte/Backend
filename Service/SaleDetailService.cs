using Backend.IRepository;
using Backend.IServices;
using Backend.Models;

namespace Backend.Service
{
    public class SaleDetailService : ISaleDetailService
    {

        private readonly ISaleDetailRepository _saleDetailRepository;

        public SaleDetailService(ISaleDetailRepository saleDetailRepository)
        {
            _saleDetailRepository = saleDetailRepository;
        }

        public async Task<IEnumerable<SaleDetail>> GetAllSaleDetailsAsync()
        {
            return await _saleDetailRepository.GetAllSaleDetailsAsync();
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
            return await _saleDetailRepository.CreateSaleDetailAsync(saleDetail);
        }

        public async Task<bool> UpdateSaleDetailAsync(SaleDetail saleDetail)
        {
            return await _saleDetailRepository.UpdateSaleDetailAsync(saleDetail);
        }

        public async Task<bool> DeleteSaleDetailAsync(int id)
        {
            return await _saleDetailRepository.DeleteSaleDetailAsync(id);
        }

        public async Task<bool> DeleteSaleDetailsBySaleIdAsync(int saleId)
        {
            return await _saleDetailRepository.DeleteSaleDetailsBySaleIdAsync(saleId);
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
