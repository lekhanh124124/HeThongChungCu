using Microsoft.AspNetCore.Http;

namespace HeThongChungCu.WebAPI.Controllers.AI;

/// <summary>
/// Request DTO cho endpoint kiểm tra chunking văn bản từ file Markdown.
/// </summary>
public sealed class RequestTestChunking
{
    /// <summary>File Markdown (.md) cần kiểm tra chia nhỏ.</summary>
    public IFormFile File { get; init; } = default!;

    /// <summary>Kích thước mỗi chunk (số token ước tính). Mặc định: 400.</summary>
    public int ChunkSize { get; init; } = 400;

    /// <summary>Độ chồng lấp giữa các chunk (số token). Mặc định: 60.</summary>
    public int ChunkOverlap { get; init; } = 60;
}
