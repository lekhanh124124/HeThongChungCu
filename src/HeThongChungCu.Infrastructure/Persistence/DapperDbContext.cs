using HeThongChungCu.Application.Common.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace HeThongChungCu.Infrastructure.Persistence;

public class DapperDbContext
{
    private readonly string _connectionString;

    public DapperDbContext(IOptions<PersistenceOptions> options)
    {
        _connectionString = options.Value.DefaultConnection
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}