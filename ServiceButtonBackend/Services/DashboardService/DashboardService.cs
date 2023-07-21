using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;

namespace ServiceButtonBackend.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private IConfiguration _configuration;

        public DashboardService(IConfiguration configuration) 
        {
            _configuration = configuration;

        }

        public async Task<ActionResult<Dictionary<string, int>>> GetDashboardData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    if(connection is not null)
                    {
                        await connection.OpenAsync();
                    }
                    

                    // Define the table names for which you want to count rows
                    string[] tableNames = { "Users", "Characters", "UserRefreshToken" };

                    var result = new System.Collections.Generic.Dictionary<string, int>();

                    foreach (var tableName in tableNames)
                    {
                        // Execute the SQL command to count rows in each table
                        string query = $"SELECT COUNT(*) FROM {tableName}";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            int rowCount = (int)await command.ExecuteScalarAsync();
                            result.Add(tableName, rowCount);
                        }
                    }

                    return result; // Sending the result back as JSON in the HTTP response
                }
            }
            catch (Exception ex)
            {
                return new ActionResult<Dictionary<string, int>>(
                    new ObjectResult(ex.Message)
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    }
                );
            }
        }

    }
}
