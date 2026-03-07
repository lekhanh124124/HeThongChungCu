using HeThongChungCu.Application.Common.Interfaces.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence;

public class DapperDbContext
{
    private readonly string _connectionString;
    private readonly IConfiguration _configuration;
    public DapperDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
