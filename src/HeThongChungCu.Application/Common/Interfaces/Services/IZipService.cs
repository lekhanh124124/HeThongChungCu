using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IZipService
{
    /// <summary>
    /// Kiểm tra xem stream có phải là một file zip hợp lệ không.
    /// </summary>
    bool IsValidZip(Stream stream);

    /// <summary>
    /// Giải nén và lấy ra các file hợp lệ (bỏ qua các thư mục và file rác của hệ điều hành).
    /// Người gọi chịu trách nhiệm Dispose các MemoryStream sau khi sử dụng.
    /// </summary>
    Task<List<(string FileName, MemoryStream Content)>> ExtractFilesAsync(Stream zipStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo một tệp zip chứa các tệp tin trực tiếp trong bộ nhớ RAM từ danh sách dữ liệu byte.
    /// </summary>
    Task<MemoryStream> CreateZipAsync(IEnumerable<(string FileName, byte[] Content)> files, CancellationToken cancellationToken = default);
}
