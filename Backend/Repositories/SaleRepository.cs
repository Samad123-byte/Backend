using Backend.Data;
using Backend.IRepository;
using Backend.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Backend.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public SaleRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task<PaginatedResponse<Sale>> GetAllSalesAsync(int pageNumber, int pageSize)
        {
            var sales = new List<Sale>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllSales", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sales.Add(new Sale
                            {
                                SaleId = reader.GetInt32("SaleId"),
                                Total = reader.GetDecimal("Total"),
                                SaleDate = reader.GetDateTime("SaleDate"),

                                SalespersonId = reader.IsDBNull("SalespersonId") ? null : reader.GetInt32("SalespersonId"),
                                Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            });
                        }
                    }
                }
            }

            // Paginate in memory
            var totalRecords = sales.Count;
            var paginatedSales = sales
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResponse<Sale>
            {
                Data = paginatedSales,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }
        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetSaleById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleId", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Sale
                            {
                                SaleId = reader.GetInt32("SaleId"),
                                Total = reader.GetDecimal("Total"),
                                SaleDate = reader.GetDateTime("SaleDate"),
                                SalespersonId = reader.IsDBNull("SalespersonId") ? null : reader.GetInt32("SalespersonId"),
                                Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_CreateSale", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Total", sale.Total);
                    command.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                    command.Parameters.AddWithValue("@SalespersonId", (object?)sale.SalespersonId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Comments", (object?)sale.Comments ?? DBNull.Value);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Sale
                            {
                                SaleId = reader.GetInt32("SaleId"),
                                Total = reader.GetDecimal("Total"),
                                SaleDate = reader.GetDateTime("SaleDate"),
                                SalespersonId = reader.IsDBNull("SalespersonId") ? null : reader.GetInt32("SalespersonId"),
                                Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            };
                        }
                    }
                }
            }

            throw new Exception("Failed to create sale");
        }

        public async Task<bool> UpdateSaleAsync(Sale sale)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UpdateSale", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleId", sale.SaleId);
                    command.Parameters.AddWithValue("@Total", sale.Total);
                    command.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                    command.Parameters.AddWithValue("@SalespersonId", (object?)sale.SalespersonId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Comments", (object?)sale.Comments ?? DBNull.Value);

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

        // ✅ FIXED: Return tuple with success and message
        public async Task<(bool success, string message)> DeleteSaleAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteSale", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleId", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            bool success = reader.GetBoolean(reader.GetOrdinal("Success"));
                            string message = reader.GetString(reader.GetOrdinal("Message"));
                            return (success, message);
                        }
                    }
                }
            }

            return (false, "No response from database.");
        }

        public async Task<bool> SaleExistsAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_SaleExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SaleId", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32("SaleCount") > 0;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = new List<Sale>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetSalesByDateRange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sales.Add(new Sale
                            {
                                SaleId = reader.GetInt32("SaleId"),
                                Total = reader.GetDecimal("Total"),
                                SaleDate = reader.GetDateTime("SaleDate"),
                                SalespersonId = reader.IsDBNull("SalespersonId") ? null : reader.GetInt32("SalespersonId"),
                                Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            });
                        }
                    }
                }
            }

            return sales;
        }

        public async Task<IEnumerable<Sale>> GetSalesBySalespersonAsync(int salespersonId)
        {
            var sales = new List<Sale>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetSalesBySalesperson", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SalespersonId", salespersonId);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sales.Add(new Sale
                            {
                                SaleId = reader.GetInt32("SaleId"),
                                Total = reader.GetDecimal("Total"),
                                SaleDate = reader.GetDateTime("SaleDate"),
                                SalespersonId = reader.IsDBNull("SalespersonId") ? null : reader.GetInt32("SalespersonId"),
                                Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                                CreatedDate = reader.GetDateTime("CreatedDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            });
                        }
                    }
                }
            }

            return sales;
        }

        public async Task<Sale?> GetSaleWithDetailsAsync(int id)
        {
            return await GetSaleByIdAsync(id);
        }
    }
}