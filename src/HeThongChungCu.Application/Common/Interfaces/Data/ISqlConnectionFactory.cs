using System.Data;

namespace HeThongChungCu.Application.Common.Interfaces.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
