using Backend.Data;
using Backend.IRepository;
using Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Backend.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public ProductRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<PaginatedResponse<Product>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = new PaginatedResponse<Product>();
            int totalRecords = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllProducts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PageNumber", pageNumber);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (totalRecords == 0)
                                totalRecords = reader.GetInt32("TotalRecords");

                            response.Data.Add(new Product
                            {
                                ProductId = reader.GetInt32("ProductId"),
                                Name = reader.GetString("Name"),
                                Code = reader.IsDBNull("Code") ? null : reader.GetString("Code"),
                                ImageURL = reader.IsDBNull("ImageURL") ? null : reader.GetString("ImageURL"),
                                CostPrice = reader.IsDBNull("CostPrice") ? null : reader.GetDecimal("CostPrice"),
                                RetailPrice = reader.IsDBNull("RetailPrice") ? null : reader.GetDecimal("RetailPrice"),
                                CreationDate = reader.IsDBNull("CreationDate") ? null : reader.GetDateTime("CreationDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            });
                        }
                    }
                }
            }

            response.CurrentPage = pageNumber;
            response.PageSize = pageSize;
            response.TotalRecords = totalRecords;
            response.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return response;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetProductById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductId", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Product
                            {
                                ProductId = reader.GetInt32("ProductId"),
                                Name = reader.GetString("Name"),
                                Code = reader.IsDBNull("Code") ? null : reader.GetString("Code"),
                                ImageURL = reader.IsDBNull("ImageURL") ? null : reader.GetString("ImageURL"),
                                CostPrice = reader.IsDBNull("CostPrice") ? null : reader.GetDecimal("CostPrice"),
                                RetailPrice = reader.IsDBNull("RetailPrice") ? null : reader.GetDecimal("RetailPrice"),
                                CreationDate = reader.IsDBNull("CreationDate") ? null : reader.GetDateTime("CreationDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_CreateProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Code", (object?)product.Code ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ImageURL", (object?)product.ImageURL ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CostPrice", (object?)product.CostPrice ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RetailPrice", (object?)product.RetailPrice ?? DBNull.Value);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Product
                            {
                                ProductId = reader.GetInt32("ProductId"),
                                Name = reader.GetString("Name"),
                                Code = reader.IsDBNull("Code") ? null : reader.GetString("Code"),
                                ImageURL = reader.IsDBNull("ImageURL") ? null : reader.GetString("ImageURL"),
                                CostPrice = reader.IsDBNull("CostPrice") ? null : reader.GetDecimal("CostPrice"),
                                RetailPrice = reader.IsDBNull("RetailPrice") ? null : reader.GetDecimal("RetailPrice"),
                                CreationDate = reader.IsDBNull("CreationDate") ? null : reader.GetDateTime("CreationDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            };
                        }
                    }
                }
            }

            throw new Exception("Failed to create product");
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UpdateProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductId", product.ProductId);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Code", (object?)product.Code ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ImageURL", (object?)product.ImageURL ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CostPrice", (object?)product.CostPrice ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RetailPrice", (object?)product.RetailPrice ?? DBNull.Value);

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

        public async Task<bool> DeleteProductAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductId", id);

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

        public async Task<bool> ProductExistsAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_ProductExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductId", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32("ProductCount") > 0;
                        }
                    }
                }
            }

            return false;
        }
    }
}
