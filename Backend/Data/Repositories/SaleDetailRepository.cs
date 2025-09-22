using Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Backend.Data.Repositories
{
    public class SaleDetailRepository : ISaleDetailRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public SaleDetailRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<SaleDetail>> GetAllSaleDetailsAsync()
        {
            var saleDetails = new List<SaleDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllSaleDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            saleDetails.Add(new SaleDetail
                            {
                                SaleDetailId = reader.GetInt32("SaleDetailId"),
                                SaleId = reader.GetInt32("SaleId"),
                                ProductId = reader.GetInt32("ProductId"),
                                RetailPrice = reader.GetDecimal("RetailPrice"),
                                Quantity = reader.GetInt32("Quantity"),
                                Discount = reader.IsDBNull("Discount") ? null : reader.GetDecimal("Discount")
                            });
                        }
                    }
                }
            }

            return saleDetails;
        }

        public async Task<SaleDetail?> GetSaleDetailByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetSaleDetailById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleDetailId", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new SaleDetail
                            {
                                SaleDetailId = reader.GetInt32("SaleDetailId"),
                                SaleId = reader.GetInt32("SaleId"),
                                ProductId = reader.GetInt32("ProductId"),
                                RetailPrice = reader.GetDecimal("RetailPrice"),
                                Quantity = reader.GetInt32("Quantity"),
                                Discount = reader.IsDBNull("Discount") ? null : reader.GetDecimal("Discount")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<SaleDetail>> GetSaleDetailsBySaleIdAsync(int saleId)
        {
            var saleDetails = new List<SaleDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetSaleDetailsBySaleId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleId", saleId);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            saleDetails.Add(new SaleDetail
                            {
                                SaleDetailId = reader.GetInt32("SaleDetailId"),
                                SaleId = reader.GetInt32("SaleId"),
                                ProductId = reader.GetInt32("ProductId"),
                                RetailPrice = reader.GetDecimal("RetailPrice"),
                                Quantity = reader.GetInt32("Quantity"),
                                Discount = reader.IsDBNull("Discount") ? null : reader.GetDecimal("Discount")
                            });
                        }
                    }
                }
            }

            return saleDetails;
        }

        public async Task<SaleDetail> CreateSaleDetailAsync(SaleDetail saleDetail)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_CreateSaleDetail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleId", saleDetail.SaleId);
                    command.Parameters.AddWithValue("@ProductId", saleDetail.ProductId);
                    command.Parameters.AddWithValue("@RetailPrice", saleDetail.RetailPrice);
                    command.Parameters.AddWithValue("@Quantity", saleDetail.Quantity);
                    command.Parameters.AddWithValue("@Discount", (object?)saleDetail.Discount ?? DBNull.Value);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new SaleDetail
                            {
                                SaleDetailId = reader.GetInt32("SaleDetailId"),
                                SaleId = reader.GetInt32("SaleId"),
                                ProductId = reader.GetInt32("ProductId"),
                                RetailPrice = reader.GetDecimal("RetailPrice"),
                                Quantity = reader.GetInt32("Quantity"),
                                Discount = reader.IsDBNull("Discount") ? null : reader.GetDecimal("Discount")
                            };
                        }
                    }
                }
            }

            throw new Exception("Failed to create sale detail");
        }

        public async Task<bool> UpdateSaleDetailAsync(SaleDetail saleDetail)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UpdateSaleDetail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleDetailId", saleDetail.SaleDetailId);
                    command.Parameters.AddWithValue("@SaleId", saleDetail.SaleId);
                    command.Parameters.AddWithValue("@ProductId", saleDetail.ProductId);
                    command.Parameters.AddWithValue("@RetailPrice", saleDetail.RetailPrice);
                    command.Parameters.AddWithValue("@Quantity", saleDetail.Quantity);
                    command.Parameters.AddWithValue("@Discount", (object?)saleDetail.Discount ?? DBNull.Value);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32("Success") == 1;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> DeleteSaleDetailAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteSaleDetail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleDetailId", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32("Success") == 1;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> DeleteSaleDetailsBySaleIdAsync(int saleId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteSaleDetailsBySaleId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleId", saleId);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32("Success") == 1;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> SaleDetailExistsAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_SaleDetailExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleDetailId", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32("SaleDetailCount") > 0;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<decimal> GetSaleTotalAsync(int saleId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetSaleTotal", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleId", saleId);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.IsDBNull("Total") ? 0 : reader.GetDecimal("Total");
                        }
                    }
                }
            }

            return 0;
        }
    }
}