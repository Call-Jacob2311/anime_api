using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace anime_api.Services
{
    // Interface for database operations
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }

    // Implement the interface in a cincreate class that wraps the conenctions
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
