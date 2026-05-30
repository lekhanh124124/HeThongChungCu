using Asp.Versioning;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.CreateTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.DeleteTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.ImportTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.SyncTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.ToggleActiveTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.UpdateTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetDanhMucTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetListTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetTriThucChatbotById;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

/// <summary>
/// Quản lý tri thức tĩnh của Chatbot AI.
/// Luồng điển hình: Import / CRUD nội dung → Review → Sync lên Qdrant.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/tri-thuc-chatbot")]
[Authorize(Roles = "Admin")]
public class TriThucChatbotController : ApiControllerBase
{
    private readonly ISender _sender;

    public TriThucChatbotController(ISender sender)
    {
        _sender = sender;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // QUERIES
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Lấy chi tiết một mục tri thức chatbot theo ID
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Admin muốn xem nội dung đầy đủ của một mục tri thức để kiểm tra hoặc chỉnh sửa.
    /// - **Hệ thống xử lý**: Truy vấn Dapper từ bảng TriThucChatbot.
    /// - **Yêu cầu dữ liệu**:
    ///     - **Bắt buộc**: `Id`.
    /// </remarks>
    [HttpPost("get-by-id")]
    [ProducesResponseType(typeof(ApiResponse<TriThucChatbotResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromBody] RequestGetTriThucChatbotById request,
        CancellationToken cancellationToken)
    {
        var query = new GetTriThucChatbotByIdQuery(request.Id);
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách danh mục tri thức chatbot
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Lấy tất cả danh mục phân biệt hiện có để dùng cho dropdown/filter.
    /// - **Hệ thống xử lý**: Truy vấn DISTINCT DanhMuc từ bảng TriThucChatbot.
    /// - **Không yêu cầu tham số**.
    /// </remarks>
    [HttpPost("get-danh-muc")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDanhMuc(CancellationToken cancellationToken)
    {
        var query = new GetDanhMucTriThucChatbotQuery();
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách mục tri thức chatbot (phân trang, lọc, tìm kiếm)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Admin tra cứu, quản lý toàn bộ kho tri thức chatbot.
    /// - **Hệ thống xử lý**: Áp dụng bộ lọc, phân trang và sắp xếp kết quả.
    /// - **Yêu cầu dữ liệu**:
    ///     - **Tùy chọn**: `DanhMuc`, `IsActive`, `IsSynced`, `Keyword`, `PageNumber`, `PageSize`, `SortCol`, `IsAsc`.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.Models.PagedResult<TriThucChatbotResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList(
        [FromBody] RequestGetListTriThucChatbot request,
        CancellationToken cancellationToken)
    {
        var query = new GetListTriThucChatbotQuery(
            request.DanhMuc,
            request.IsActive,
            request.IsSynced,
            request.Keyword,
            request.PageNumber,
            request.PageSize,
            request.SortCol,
            request.IsAsc);

        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COMMANDS — CRUD
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Tạo mới một mục tri thức cho chatbot
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Admin muốn thêm câu hỏi thường gặp, nội quy, quy trình mới vào kho tri thức.
    /// - **Hệ thống xử lý**: Tạo entity với `IsActive = true`, `IsSynced = false`. Cần gọi `/sync` sau đó.
    /// - **Yêu cầu dữ liệu**:
    ///     - **Bắt buộc**: `TieuDe`, `NoiDung`, `DanhMuc`.
    ///     - **Tùy chọn**: `ThuTuHienThi` (mặc định: 0).
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TriThucChatbotResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] RequestCreateTriThucChatbot request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTriThucChatbotCommand(
            request.TieuDe,
            request.NoiDung,
            request.DanhMuc,
            request.ThuTuHienThi);

        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Cập nhật nội dung một mục tri thức chatbot
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Admin chỉnh sửa nội dung lỗi thời.
    /// - **Hệ thống xử lý**: Gọi domain `Update()` → reset `IsSynced = false`. Cần gọi `/sync` sau đó.
    /// - **Yêu cầu dữ liệu**:
    ///     - **Bắt buộc**: `Id`, `TieuDe`, `NoiDung`, `DanhMuc`.
    ///     - **Tùy chọn**: `ThuTuHienThi`.
    /// </remarks>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<TriThucChatbotResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        [FromBody] RequestUpdateTriThucChatbot request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTriThucChatbotCommand(
            request.Id,
            request.TieuDe,
            request.NoiDung,
            request.DanhMuc,
            request.ThuTuHienThi);

        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Xóa (soft-delete) một hoặc nhiều mục tri thức chatbot
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Admin xóa nội dung không còn phù hợp.
    /// - **Quy tắc**: Chỉ được xóa mục đang **inactive** (`IsActive = false`). Nếu bất kỳ ID nào đang active sẽ bị từ chối toàn bộ batch.
    /// - **Hệ thống xử lý**: Soft-delete qua EF Core. Vector trong Qdrant sẽ được dọn ở lần sync tiếp theo.
    /// - **Yêu cầu**: Deactivate trước, sau đó mới được xóa.
    /// - **Yêu cầu dữ liệu**: **Bắt buộc**: `Ids` (danh sách ID cần xóa).
    /// </remarks>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromBody] RequestDeleteTriThucChatbot request,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTriThucChatbotCommand(request.Ids);
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Kích hoạt một mục tri thức chatbot
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khôi phục mục tri thức đã bị tắt. Reset `IsSynced = false` để sync lại.
    /// - **Yêu cầu dữ liệu**: **Bắt buộc**: `Id`.
    /// </remarks>
    [HttpPut("activate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Activate(
        [FromBody] RequestToggleTriThucChatbot request,
        CancellationToken cancellationToken)
    {
        var command = new ToggleActiveTriThucChatbotCommand(request.Id, Activate: true);
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Vô hiệu hóa một mục tri thức chatbot
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Tạm ẩn nội dung khỏi chatbot mà không xóa. Reset `IsSynced = false` để sync xóa khỏi Qdrant.
    /// - **Yêu cầu dữ liệu**: **Bắt buộc**: `Id`.
    /// </remarks>
    [HttpPut("deactivate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deactivate(
        [FromBody] RequestToggleTriThucChatbot request,
        CancellationToken cancellationToken)
    {
        var command = new ToggleActiveTriThucChatbotCommand(request.Id, Activate: false);
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COMMANDS — SYNC & IMPORT
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Đồng bộ tri thức chatbot từ SQL DB lên Qdrant vector store
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Sau khi activate tri thức, gọi endpoint này để phản ánh lên Qdrant.
    /// - **Hệ thống xử lý**:
    ///     - `IsActive = true` → upsert vector lên Qdrant (idempotent).
    ///     - `IsActive = false` và đã từng sync → xóa vector khỏi Qdrant.
    ///     - Ghi nhận `IsSynced`, `LastSyncedAt` sau mọi thao tác thành công.
    /// - **Idempotent**: Gọi nhiều lần cho cùng kết quả.
    /// </remarks>
    [HttpPost("sync")]
    [ProducesResponseType(typeof(ApiResponse<SyncTriThucChatbotResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Sync(CancellationToken cancellationToken)
    {
        var command = new SyncTriThucChatbotCommand();
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Import file Markdown chứa tri thức vào kho tri thức chatbot
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Admin muốn nhập nhanh nhiều mục tri thức từ tài liệu có sẵn thay vì gõ thủ công.
    /// - **Hệ thống xử lý**:
    ///     - Parse Markdown: H1 (#) → DanhMuc, mỗi H2 (##) → 1 mục tri thức.
    ///     - Tạo các bản ghi `TriThucChatbot` với `IsActive = true`, `IsSynced = false`.
    ///     - Gọi `/sync` sau khi import để đồng bộ lên Qdrant.
    /// - **Yêu cầu dữ liệu**:
    ///     - **Bắt buộc**: `File` (.md, tối đa 5MB).
    ///     - **Tùy chọn**: `DefaultThuTuHienThi` (thứ tự hiển thị bắt đầu, mặc định: 0).
    /// - **Cấu trúc file mẫu**:
    ///   ```markdown
    ///   # Dịch vụ chung cư
    ///   ## Phí quản lý hàng tháng
    ///   Nội dung mô tả phí...
    ///   ## Đăng ký giữ xe
    ///   Quy trình đăng ký...
    ///   ```
    /// </remarks>
    [HttpPost("import")]
    [ProducesResponseType(typeof(ApiResponse<ImportTriThucChatbotResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Import(
        [FromForm] RequestImportTriThucChatbot request,
        CancellationToken cancellationToken)
    {
        var command = new ImportTriThucChatbotCommand(
            request.File.OpenReadStream(),
            request.File.FileName,
            request.ThuTuHienThi,
            request.DanhMuc);

        return HandleResult(await _sender.Send(command, cancellationToken));
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// REQUEST DTOs
// ─────────────────────────────────────────────────────────────────────────────

public sealed class RequestGetTriThucChatbotById
{
    public int Id { get; init; }
}

public sealed class RequestGetListTriThucChatbot
{
    public string? DanhMuc { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsSynced { get; init; }
    public string? Keyword { get; init; }
    public int? PageNumber { get; init; } = 1;
    public int? PageSize { get; init; } = 20;
    public string? SortCol { get; init; }
    public bool? IsAsc { get; init; } = true;
}

public sealed class RequestCreateTriThucChatbot
{
    public required string TieuDe { get; init; }
    public required string NoiDung { get; init; }
    public required string DanhMuc { get; init; }
    public int ThuTuHienThi { get; init; } = 0;
}

public sealed class RequestUpdateTriThucChatbot
{
    public int Id { get; init; }
    public required string TieuDe { get; init; }
    public required string NoiDung { get; init; }
    public required string DanhMuc { get; init; }
    public int ThuTuHienThi { get; init; } = 0;
}

public sealed class RequestDeleteTriThucChatbot
{
    public required List<int> Ids { get; init; }
}

public sealed class RequestToggleTriThucChatbot
{
    public int Id { get; init; }
}

public sealed class RequestSyncTriThucChatbot;

public sealed class RequestImportTriThucChatbot
{
    public required IFormFile File { get; init; }

    /// <summary>
    /// Danh mục ghi đè (tùy chọn). Mặc định lấy từ H1 trong file.
    /// Dùng khi muốn phân loại vào nhóm khác với tiêu đề của file.
    /// </summary>
    public string? DanhMuc { get; init; }

    /// <summary>Thứ tự hiển thị (mặc định: 0).</summary>
    public int ThuTuHienThi { get; init; } = 0;
}
