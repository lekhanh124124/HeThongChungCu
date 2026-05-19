using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IBackupService
{
    /// <summary>
    /// Xuất toàn bộ dữ liệu nghiệp vụ từ SQL Server thành danh sách các file JSON (dưới dạng mảng byte).
    /// </summary>
    Task<Result<List<(string FileName, byte[] Content)>>> ExportBusinessDataAsync(string adminEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Nhập dữ liệu nghiệp vụ từ danh sách các Stream file JSON đã giải nén vào database sử dụng SqlBulkCopy.
    /// </summary>
    Task<Result> ImportBusinessDataAsync(IEnumerable<(string FileName, Stream Content)> extractedFiles, CancellationToken cancellationToken = default);
}
