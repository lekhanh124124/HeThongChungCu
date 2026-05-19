using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence;

namespace HeThongChungCu.Infrastructure.Services;

public class BackupService : IBackupService
{
    private readonly AppDbContext _context;

    public BackupService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<(string FileName, byte[] Content)>>> ExportBusinessDataAsync(string adminEmail, CancellationToken cancellationToken = default)
    {
        var result = new List<(string FileName, byte[] Content)>();

        // 1. Phản chiếu và lấy danh sách các bảng vật lý độc bản tại runtime
        var targetTables = _context.Model.GetEntityTypes()
            .Select(e => new { TableName = e.GetTableName(), Schema = e.GetSchema() ?? "dbo" })
            .Where(t => t.TableName != null 
                        && t.TableName != "__EFMigrationsHistory" 
                        && !t.TableName.Equals("Tokens", StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToList();

        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        foreach (var target in targetTables)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM [{target.Schema}].[{target.TableName}]";
            
            if (_context.Database.CurrentTransaction != null)
            {
                command.Transaction = _context.Database.CurrentTransaction.GetDbTransaction();
            }

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var tableRows = new List<Dictionary<string, object?>>();

            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var colName = reader.GetName(i);
                    var val = reader.GetValue(i);
                    row[colName] = val == DBNull.Value ? null : val;
                }
                tableRows.Add(row);
            }

            // Đóng reader trước khi tiếp tục
            await reader.DisposeAsync();

            // 2. Tuần tự hóa dữ liệu thô của bảng thành JSON bytes
            var jsonString = JsonSerializer.Serialize(tableRows, new JsonSerializerOptions 
            { 
                WriteIndented = false
            });
            
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            result.Add(($"{target.TableName}.json", bytes));
        }

        return result;
    }

    public async Task<Result> ImportBusinessDataAsync(IEnumerable<(string FileName, Stream Content)> extractedFiles, CancellationToken cancellationToken = default)
    {
        var filesList = extractedFiles.ToList();
        
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        // Thực thi toàn bộ quá trình khôi phục.
        // Hỗ trợ tái sử dụng transaction hiện tại (ambient transaction) từ MediatR Pipeline hoặc tự tạo mới nếu chưa có.
        IDbContextTransaction? transaction = null;
        if (_context.Database.CurrentTransaction == null)
        {
            transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        try
        {
            var dbTransaction = _context.Database.CurrentTransaction!.GetDbTransaction();

            // Lấy thông tin metadata động từ EF Core để tránh hardcode tên bảng/cột và giá trị Smart Enum
            var tepTaiLieuType = typeof(TepTaiLieu);
            var tepTaiLieuEntity = _context.Model.FindEntityType(tepTaiLieuType);
            var tepTaiLieuTableName = tepTaiLieuEntity?.GetTableName() ?? "TepTaiLieu";
            var loaiTepIdProperty = tepTaiLieuEntity?.FindProperty(nameof(TepTaiLieu.LoaiTepId));
            var loaiTepIdColumnName = loaiTepIdProperty?.GetColumnName() ?? "LoaiTepId";
            var saoLuuDbValue = LoaiTepTaiLieu.SaoLuuDb.Value;

            // 1. Vô hiệu hóa toàn bộ ràng buộc khóa ngoại (Foreign Key) trên tất cả các bảng trong CSDL
            using (var disableCmd = connection.CreateCommand())
            {
                disableCmd.Transaction = dbTransaction;
                disableCmd.CommandText = @"
                    DECLARE @sql NVARCHAR(MAX) = N'';
                    SELECT @sql += N'ALTER TABLE ' + QUOTENAME(cs.name) + '.' + QUOTENAME(ct.name) 
                        + ' NOCHECK CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(13)
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.tables ct ON fk.parent_object_id = ct.object_id
                    INNER JOIN sys.schemas cs ON ct.schema_id = cs.schema_id;
                    IF @sql <> N'' EXEC sp_executesql @sql;";
                await disableCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            // Phản chiếu để lấy danh sách bảng và lập chỉ mục để tìm Schema
            var entityTypes = _context.Model.GetEntityTypes().ToList();

            foreach (var file in filesList)
            {
                var tableName = Path.GetFileNameWithoutExtension(file.FileName);
                var schema = entityTypes.FirstOrDefault(e => e.GetTableName() == tableName)?.GetSchema() ?? "dbo";

                // B. Xóa dữ liệu cũ trong bảng trước khi Bulk Copy (chừa lại lịch sử sao lưu)
                using (var deleteCmd = connection.CreateCommand())
                {
                    deleteCmd.Transaction = dbTransaction;
                    if (tableName.Equals(tepTaiLieuTableName, StringComparison.OrdinalIgnoreCase))
                    {
                        deleteCmd.CommandText = $"DELETE FROM [{schema}].[{tableName}] WHERE [{loaiTepIdColumnName}] <> {saoLuuDbValue};";
                    }
                    else
                    {
                        deleteCmd.CommandText = $"DELETE FROM [{schema}].[{tableName}];";
                    }
                    await deleteCmd.ExecuteNonQueryAsync(cancellationToken);
                }

                // C. Phân tích cú pháp JSON thành danh sách dòng dữ liệu
                var rows = await JsonSerializer.DeserializeAsync<List<Dictionary<string, JsonElement>>>(file.Content, cancellationToken: cancellationToken);
                
                if (rows == null || rows.Count == 0)
                {
                    continue; // Bảng rỗng, không cần Bulk Copy
                }

                // D. Dựng DataTable động
                var dataTable = new DataTable();
                foreach (var colName in rows[0].Keys)
                {
                    dataTable.Columns.Add(colName, typeof(object));
                }

                foreach (var row in rows)
                {
                    // Lọc tệp tin sao lưu cơ sở dữ liệu để giữ lại lịch sử sống trên DB hiện tại
                    if (tableName.Equals(tepTaiLieuTableName, StringComparison.OrdinalIgnoreCase))
                    {
                        var loaiTepKey = row.Keys.FirstOrDefault(k => k.Equals(loaiTepIdColumnName, StringComparison.OrdinalIgnoreCase));
                        if (loaiTepKey != null)
                        {
                            var loaiTepVal = GetValueFromElement(row[loaiTepKey]);
                            if (loaiTepVal != null && Convert.ToInt32(loaiTepVal) == saoLuuDbValue)
                            {
                                continue;
                            }
                        }
                    }

                    var dataRow = dataTable.NewRow();
                    foreach (var colName in row.Keys)
                    {
                        dataRow[colName] = GetValueFromElement(row[colName]) ?? DBNull.Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }

                // Nếu sau khi lọc không còn dòng nào, không cần Bulk Copy
                if (dataTable.Rows.Count == 0)
                {
                    continue;
                }

                // E. Thực thi SqlBulkCopy với tùy chọn giữ nguyên Identity và Null
                using (var bulkCopy = new SqlBulkCopy((SqlConnection)connection, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls, (SqlTransaction)dbTransaction))
                {
                    bulkCopy.DestinationTableName = $"[{schema}].[{tableName}]";
                    bulkCopy.BulkCopyTimeout = 600; // 10 phút

                    // Ánh xạ cột tường minh để tránh lệch thứ tự cột vật lý
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
                }
            }

            // 2. Kích hoạt lại toàn bộ ràng buộc khóa ngoại và yêu cầu SQL Server kiểm tra lại tính toàn vẹn (WITH CHECK)
            using (var enableCmd = connection.CreateCommand())
            {
                enableCmd.Transaction = dbTransaction;
                enableCmd.CommandText = @"
                    DECLARE @sql NVARCHAR(MAX) = N'';
                    SELECT @sql += N'ALTER TABLE ' + QUOTENAME(cs.name) + '.' + QUOTENAME(ct.name) 
                        + ' WITH CHECK CHECK CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(13)
                    FROM sys.foreign_keys fk
                    INNER JOIN sys.tables ct ON fk.parent_object_id = ct.object_id
                    INNER JOIN sys.schemas cs ON ct.schema_id = cs.schema_id;
                    IF @sql <> N'' EXEC sp_executesql @sql;";
                await enableCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            // 3. Cam kết giao dịch thành công nếu chúng ta tự khởi tạo nó
            if (transaction != null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            return Result.Success();
        }
        catch (Exception)
        {
            // Nếu có bất kỳ lỗi nào và đây là transaction do chúng ta khởi tạo, rollback ngay lập tức để bảo vệ toàn vẹn dữ liệu
            if (transaction != null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }
            throw;
        }
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }
    }

    private static object? GetValueFromElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Number:
                if (element.TryGetInt64(out long l)) return l;
                if (element.TryGetDouble(out double d)) return d;
                return element.GetDecimal();
            case JsonValueKind.String:
                var str = element.GetString();
                if (Guid.TryParse(str, out Guid guid)) return guid;
                if (str != null && str.Contains(':') && TimeSpan.TryParse(str, out TimeSpan ts)) return ts;
                if (DateTime.TryParse(str, out DateTime dt)) return dt;
                return str;
            default:
                return element.ToString();
        }
    }
}
