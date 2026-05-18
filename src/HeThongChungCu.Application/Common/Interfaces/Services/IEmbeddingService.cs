namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IEmbeddingService
{
    /// <summary>
    /// Chuyển đổi văn bản thành vector embedding (mặc định 3072 chiều).
    /// </summary>
    /// <param name="text">Văn bản cần vector hóa.</param>
    /// <param name="cancellationToken">Token thông báo hủy tác vụ liên kết.</param>
    /// <returns>Mảng số thực đại diện cho vector embedding có kích thước 3072 chiều.</returns>
    /// <exception cref="ArgumentException">Ném ra khi văn bản đầu vào trống hoặc null.</exception>
    /// <exception cref="InvalidOperationException">Ném ra khi API không phản hồi hoặc kích thước vector trả về không khớp cấu hình.</exception>
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Định danh của model embedding đang sử dụng (ví dụ: models/gemini-embedding-2).
    /// </summary>
    string ModelId { get; }
}
