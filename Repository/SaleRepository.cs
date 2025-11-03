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

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllSales", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                sales.Add(new Sale
                {
                    SaleId = reader.GetInt32("SaleId"),
                    Total = reader.GetDecimal("Total"),
                    SaleDate = reader.GetDateTime("SaleDate"),
                    SalespersonName = reader.IsDBNull("SalespersonName") ? null : reader.GetString("SalespersonName"),
                    SalespersonId = reader.IsDBNull("SalespersonId") ? null : reader.GetInt32("SalespersonId"),
                    Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate"),
                    SaleDetails = new List<Sale.SaleDetailDto>() // Initialize empty list
                });
            }

            // Paginate
            var totalRecords = sales.Count;
            var paginatedSales = sales.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

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
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetSaleById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SaleId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
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
                    UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate"),
                    SaleDetails = new List<Sale.SaleDetailDto>() // Initialize empty list
                };
            }

            return null;
        }



        public async Task<Sale> CreateSaleAsync(Sale sale)
        {

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateSale", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Sale parameters
            command.Parameters.AddWithValue("@SalespersonId", (object?)sale.SalespersonId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Comments", (object?)sale.Comments ?? DBNull.Value);
            command.Parameters.Add("@SaleDate", SqlDbType.DateTime).Value =
     (object?)sale.SaleDate?.ToUniversalTime() ?? DBNull.Value;


            // Convert SaleDetails to DataTable
            var dt = new DataTable();
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("RetailPrice", typeof(decimal));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Discount", typeof(decimal));

            if (sale.SaleDetails != null)
            {
                foreach (var detail in sale.SaleDetails)
                {
                    dt.Rows.Add(detail.ProductId, detail.RetailPrice, detail.Quantity, detail.Discount ?? 0);
                }
            }

            var tvp = command.Parameters.AddWithValue("@SaleDetails", dt);
            tvp.SqlDbType = SqlDbType.Structured;
            tvp.TypeName = "dbo.SaleDetailType";

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            Sale createdSale = null;

            // Read the first result set (Sale)
            if (await reader.ReadAsync())
            {
                createdSale = new Sale
                {
                    SaleId = reader.GetInt32(reader.GetOrdinal("SaleId")),
                    Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                    SaleDate = DateTime.SpecifyKind(reader.GetDateTime(reader.GetOrdinal("SaleDate")), DateTimeKind.Utc),

                    SalespersonId = reader.IsDBNull(reader.GetOrdinal("SalespersonId")) ? null : reader.GetInt32(reader.GetOrdinal("SalespersonId")),
                    Comments = reader.IsDBNull(reader.GetOrdinal("Comments")) ? null : reader.GetString(reader.GetOrdinal("Comments")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedDate")),
                    SaleDetails = new List<Sale.SaleDetailDto>()
                };
            }

            // Move to second result set (SaleDetails)
            if (await reader.NextResultAsync())
            {
                var details = new List<Sale.SaleDetailDto>();
                while (await reader.ReadAsync())
                {
                    details.Add(new Sale.SaleDetailDto
                    {
                        ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                        RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        Discount = reader.GetDecimal(reader.GetOrdinal("Discount"))
                    });
                }
                if (createdSale != null)
                    createdSale.SaleDetails = details;
            }

            return createdSale!;
        }


        public async Task<bool> UpdateSaleAsync(Sale sale)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var dt = new DataTable();
            dt.Columns.Add("ProductId", typeof(int));
            dt.Columns.Add("RetailPrice", typeof(decimal));
            dt.Columns.Add("Quantity", typeof(int));
            dt.Columns.Add("Discount", typeof(decimal));

            if (sale.SaleDetails != null)
            {
                foreach (var detail in sale.SaleDetails)
                {
                    if (detail.RowState == "Added" || detail.RowState == "Modified")
                        dt.Rows.Add(detail.ProductId, detail.RetailPrice, detail.Quantity, detail.Discount ?? 0);
                }
            }

            using var cmd = new SqlCommand("sp_UpdateSale", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@SaleId", sale.SaleId);
            cmd.Parameters.AddWithValue("@SalespersonId", (object?)sale.SalespersonId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Comments", (object?)sale.Comments ?? DBNull.Value);
            cmd.Parameters.Add("@SaleDate", SqlDbType.DateTime).Value =
                (object?)sale.SaleDate?.ToUniversalTime() ?? DBNull.Value;

            var tvp = cmd.Parameters.AddWithValue("@SaleDetails", dt);
            tvp.SqlDbType = SqlDbType.Structured;
            tvp.TypeName = "dbo.SaleDetailType";

            await cmd.ExecuteNonQueryAsync();

            return true; // success
        }




        public async Task<Sale?> DeleteSaleDetailAsync(int saleId, int productId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_DeleteSaleDetailFromSale", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@SaleId", saleId);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();

            // ExecuteReader because SP returns updated Sale and details
            using var reader = await command.ExecuteReaderAsync();

            Sale? updatedSale = null;

            // First result set: updated Sale
            if (await reader.ReadAsync())
            {
                updatedSale = new Sale
                {
                    SaleId = reader.GetInt32("SaleId"),
                    Total = reader.GetDecimal("Total"),
                    SaleDate = reader.GetDateTime("SaleDate"),
                    SalespersonId = reader.IsDBNull("SalespersonId") ? null : reader.GetInt32("SalespersonId"),
                    Comments = reader.IsDBNull("Comments") ? null : reader.GetString("Comments"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate"),
                    SaleDetails = new List<Sale.SaleDetailDto>()
                };
            }

            // Second result set: remaining SaleDetails
            if (await reader.NextResultAsync())
            {
                var details = new List<Sale.SaleDetailDto>();
                while (await reader.ReadAsync())
                {
                    details.Add(new Sale.SaleDetailDto
                    {
                        ProductId = reader.GetInt32("ProductId"),
                        RetailPrice = reader.GetDecimal("RetailPrice"),
                        Quantity = reader.GetInt32("Quantity"),
                        Discount = reader.GetDecimal("Discount")
                    });
                }
                if (updatedSale != null) updatedSale.SaleDetails = details;
            }

            return updatedSale;
        }


        public async Task<int> DeleteSaleAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_DeleteSale", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SaleId", id);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync(); // Read the SELECT result
            return Convert.ToInt32(result ?? 0);
        }


        public async Task<bool> SaleExistsAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_SaleExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SaleId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32("SaleCount") > 0;
            }

            return false;
        }


        public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sales = new List<Sale>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetSalesByDateRange", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
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
                    UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate"),
                    SaleDetails = new List<Sale.SaleDetailDto>()
                });
            }

            return sales;
        }

        public async Task<IEnumerable<Sale>> GetSalesBySalespersonAsync(int salespersonId)
        {
            var sales = new List<Sale>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetSalesBySalesperson", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SalespersonId", salespersonId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
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
                    UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate"),
                    SaleDetails = new List<Sale.SaleDetailDto>()
                });
            }

            return sales;
        }

        public async Task<Sale?> GetSaleWithDetailsAsync(int id)
        {
            var sale = await GetSaleByIdAsync(id);
            if (sale == null) return null;

            // Load SaleDetails
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetSaleDetailsBySaleId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SaleId", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var details = new List<Sale.SaleDetailDto>();
            while (await reader.ReadAsync())
            {
                details.Add(new Sale.SaleDetailDto
                {
                    ProductId = reader.GetInt32("ProductId"),
                    RetailPrice = reader.GetDecimal("RetailPrice"),
                    Quantity = reader.GetInt32("Quantity"),
                    Discount = reader.GetDecimal("Discount")
                });
            }

            sale.SaleDetails = details;
            return sale;
        }
    }
}
