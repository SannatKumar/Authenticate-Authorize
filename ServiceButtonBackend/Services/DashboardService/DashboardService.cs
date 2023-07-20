using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;

namespace ServiceButtonBackend.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        public Task<IActionResult> GetDashboardData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Define the table names for which you want to count rows
                    string[] tableNames = { "table1", "table2", "table3" };

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

                    return Ok(result); // Sending the result back as JSON in the HTTP response
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
