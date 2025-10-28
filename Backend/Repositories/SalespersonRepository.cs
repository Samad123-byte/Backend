using Backend.Data;
using Backend.IRepository;
using Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Backend.Repositories
{
    public class SalespersonRepository : ISalespersonRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public SalespersonRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task<PaginatedResponse<Salesperson>> GetAllSalespersonsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var response = new PaginatedResponse<Salesperson>();
            int totalRecords = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllSalespersons", connection))
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

                            response.Data.Add(new Salesperson
                            {
                                SalespersonId = reader.GetInt32("SalespersonId"),
                                Name = reader.GetString("Name"),
                                Code = reader.GetString("Code"),
                                EnteredDate = reader.IsDBNull("EnteredDate") ? null : reader.GetDateTime("EnteredDate"),
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

        public async Task<Salesperson?> GetSalespersonByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetSalespersonById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SalespersonId", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Salesperson
                            {
                                SalespersonId = reader.GetInt32("SalespersonId"),
                                Name = reader.GetString("Name"),
                                Code = reader.GetString("Code"),
                                EnteredDate = reader.IsDBNull("EnteredDate") ? null : reader.GetDateTime("EnteredDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<Salesperson?> GetSalespersonByCodeAsync(string code)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetSalespersonByCode", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Code", code);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Salesperson
                            {
                                SalespersonId = reader.GetInt32("SalespersonId"),
                                Name = reader.GetString("Name"),
                                Code = reader.GetString("Code"),
                                EnteredDate = reader.IsDBNull("EnteredDate") ? null : reader.GetDateTime("EnteredDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<Salesperson>> GetActiveSalespersonsAsync()
        {
            var salespersons = new List<Salesperson>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetActiveSalespersons", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            salespersons.Add(new Salesperson
                            {
                                SalespersonId = reader.GetInt32("SalespersonId"),
                                Name = reader.GetString("Name"),
                                Code = reader.GetString("Code"),
                                EnteredDate = reader.IsDBNull("EnteredDate") ? null : reader.GetDateTime("EnteredDate"),
                                UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate")
                            });
                        }
                    }
                }
            }

            return salespersons;
        }

        public async Task<Salesperson> CreateSalespersonAsync(Salesperson salesperson)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_InsertSalesperson", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", salesperson.Name);
                    command.Parameters.AddWithValue("@Code", salesperson.Code);
                    command.Parameters.AddWithValue("@EnteredDate", (object?)salesperson.EnteredDate ?? DBNull.Value);

                    await connection.OpenAsync();
                    var result = await command.ExecuteScalarAsync();

                    if (result != null)
                    {
                        int newSalespersonId = Convert.ToInt32(result);

                        // Get the newly created salesperson to return it
                        return await GetSalespersonByIdAsync(newSalespersonId) ?? throw new Exception("Failed to retrieve created salesperson");
                    }
                }
            }

            throw new Exception("Failed to create salesperson");
        }

        public async Task<bool> UpdateSalespersonAsync(Salesperson salesperson)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UpdateSalesperson", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SalespersonId", salesperson.SalespersonId);
                    command.Parameters.AddWithValue("@Name", salesperson.Name);
                    command.Parameters.AddWithValue("@Code", salesperson.Code);
                    command.Parameters.AddWithValue("@EnteredDate", (object?)salesperson.EnteredDate ?? DBNull.Value);

                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> DeleteSalespersonAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_DeleteSalesperson", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SalespersonId", id);

                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> SalespersonExistsAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_SalespersonExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SalespersonId", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader.GetInt32("SalespersonCount") > 0;
                        }
                    }
                }
            }

            return false;
        }
    }
}