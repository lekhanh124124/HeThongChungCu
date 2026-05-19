using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Asp.Versioning;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Application.Features.QLSystem.DTOs;
using HeThongChungCu.Application.Features.QLSystem.Commands.CreateBackup;
using HeThongChungCu.Application.Features.QLSystem.Commands.DeleteBackup;
using HeThongChungCu.Application.Features.QLSystem.Commands.RestoreBackup;
using HeThongChungCu.Application.Features.QLSystem.Queries.GetBackupHistory;
using HeThongChungCu.WebAPI.Common.Models;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/backup")]
public class BackupController : ApiControllerBase
{
    private readonly ISender _sender;

    public BackupController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy lịch sử tất cả các bản sao lưu nghiệp vụ CSDL
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản trị viên hệ thống tra cứu danh sách các bản sao lưu CSDL hiện có trên Cloud.
    /// - **Hệ thống xử lý**: Truy xuất thông tin từ danh mục thực thể TepTaiLieu được lọc theo phân loại SaoLuuDb.
    /// </remarks>
    [HttpPost("get-list")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BackupHistoryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList([FromBody] GetBackupHistoryQuery query, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }

    /// <summary>
    /// Tạo thủ công một bản sao lưu nghiệp vụ CSDL mới
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản trị viên muốn tạo điểm khôi phục nhanh trước khi thực hiện các thay đổi dữ liệu quy mô lớn.
    /// - **Hệ thống xử lý**: 
    ///     - Quét toàn bộ schema database ở runtime và trích xuất dữ liệu thô thành các file JSON trong RAM.
    ///     - Nén in-memory thành file Zip.
    ///     - Tải trực tiếp lên Azure Blob Storage và ghi nhận thực thể tài liệu.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BackupHistoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new CreateBackupCommand(), cancellationToken));
    }

    /// <summary>
    /// Xóa một bản sao lưu nghiệp vụ CSDL khỏi hệ thống
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Quản trị viên chủ động giải phóng dung lượng lưu trữ Cloud bằng cách xóa các bản sao lưu không còn giá trị.
    /// </remarks>
    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromBody] DeleteBackupCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Khôi phục dữ liệu nghiệp vụ CSDL từ một bản sao lưu
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Khi có sự cố nghiệp vụ nghiêm trọng và BQL muốn đưa hệ thống về trạng thái ổn định trước đó.
    /// - **Hệ thống xử lý**: 
    ///     - Tải tệp Zip từ Cloud về bộ nhớ tạm thời trong RAM.
    ///     - Giải nén thô in-memory.
    ///     - Tạm ngắt toàn bộ kiểm tra khóa ngoại, xóa dữ liệu hiện có và đổ bộ dữ liệu khôi phục thần tốc bằng SqlBulkCopy trong transaction an toàn.
    /// </remarks>
    [HttpPost("restore")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Restore([FromBody] RestoreBackupCommand command, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

}
