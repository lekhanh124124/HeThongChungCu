using HeThongChungCu.Application.Features.Catalog.DTOs;
using HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;
using HeThongChungCu.Application.Features.Catalog.Queries.LayGioiTinhForSelector;
using HeThongChungCu.Application.Features.Catalog.Queries.LayLoaiCanHoForSelector;
using HeThongChungCu.Application.Features.Catalog.Queries.LayLoaiDichVuForSelector;
using HeThongChungCu.Application.Features.Catalog.Queries.LayLoaiPhuongTienForSelector;
using HeThongChungCu.Application.Features.Catalog.Queries.LayLoaiQuanHeCuTruForSelector;
using HeThongChungCu.Application.Features.Catalog.Queries.LayLoaiTangForSelector;
using HeThongChungCu.Application.Features.Catalog.Queries.LayTinhTrangCanHoForSelector;
using HeThongChungCu.Application.Features.Catalog.Queries.LayTrangThaiToaNhaForSelector;
using HeThongChungCu.Application.Features.Catalog.Queries.LayTrangThaiPhuongTienForSelector;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/catalog")]
public class CatalogController : ApiControllerBase
{
    private readonly ISender _sender;

    public CatalogController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy cấu trúc chung cư (Toà nhà -> Tầng -> Căn hộ)
    /// </summary>
    /// <remarks>
    /// API trả về toàn bộ cấu trúc phân cấp của chung cư để hiển thị sơ đồ hoặc danh sách chọn.
    /// </remarks>
    [HttpPost("cau-truc-chung-cu")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CauTrucToaNhaResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCauTrucChungCu([FromBody] LayCauTrucChungCuQuery request, CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(request, cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách giới tính để chọn
    /// </summary>
    /// <remarks>
    /// API trả về danh sách giới tính (Nam, Nữ, Khác) để binding vào dropdown/select.
    /// </remarks>
    [HttpPost("gioi-tinh-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGioiTinhForSelector(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayGioiTinhForSelectorQuery(), cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách loại căn hộ để chọn
    /// </summary>
    [HttpPost("loai-can-ho-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoaiCanHoForSelector(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayLoaiCanHoForSelectorQuery(), cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách loại dịch vụ để chọn
    /// </summary>
    [HttpPost("loai-dich-vu-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoaiDichVuForSelector(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayLoaiDichVuForSelectorQuery(), cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách loại phương tiện để chọn
    /// </summary>
    [HttpPost("loai-phuong-tien-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoaiPhuongTienForSelector(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayLoaiPhuongTienForSelectorQuery(), cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách loại quan hệ cư trú để chọn
    /// </summary>
    [HttpPost("loai-quan-he-cu-tru-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoaiQuanHeCuTruForSelector(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayLoaiQuanHeCuTruForSelectorQuery(), cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách loại tầng để chọn
    /// </summary>
    [HttpPost("loai-tang-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoaiTangForSelector(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayLoaiTangForSelectorQuery(), cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách tình trạng căn hộ để chọn
    /// </summary>
    [HttpPost("tinh-trang-can-ho-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTinhTrangCanHoForSelector(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayTinhTrangCanHoForSelectorQuery(), cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách trạng thái toà nhà để chọn
    /// </summary>
    [HttpPost("trang-thai-toa-nha-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrangThaiToaNhaForSelector(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayTrangThaiToaNhaForSelectorQuery(), cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách trạng thái phương tiện để chọn
    /// </summary>
    [HttpPost("trang-thai-phuong-tien-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrangThaiPhuongTienForSelector(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayTrangThaiPhuongTienForSelectorQuery(), cancellationToken));
    }
}
