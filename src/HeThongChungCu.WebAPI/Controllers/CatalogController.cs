using HeThongChungCu.Application.Features.Catalog.DTOs;
using HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;
using HeThongChungCu.Application.Features.Catalog.Queries.LayDichVuForSelector;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
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
    public IActionResult GetGioiTinhForSelector()
    {
        var result = GioiTinh.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <summary>
    /// Lấy danh sách loại căn hộ để chọn
    /// </summary>
    [HttpPost("loai-can-ho-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetLoaiCanHoForSelector()
    {
        var result = LoaiCanHo.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <summary>
    /// Lấy danh sách dịch vụ để chọn
    /// </summary>
    [HttpPost("dich-vu-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDichVuForSelector(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new LayDichVuForSelectorQuery(), cancellationToken));
    }

    /// <summary>
    /// Lấy danh sách loại phương tiện để chọn
    /// </summary>
    [HttpPost("loai-phuong-tien-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetLoaiPhuongTienForSelector()
    {
        var result = LoaiPhuongTien.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <summary>
    /// Lấy danh sách loại quan hệ cư trú để chọn
    /// </summary>
    [HttpPost("loai-quan-he-cu-tru-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetLoaiQuanHeCuTruForSelector()
    {
        var result = LoaiQuanHeCuTru.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <summary>
    /// Lấy danh sách loại tầng để chọn
    /// </summary>
    [HttpPost("loai-tang-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetLoaiTangForSelector()
    {
        var result = LoaiTang.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <summary>
    /// Lấy danh sách tình trạng căn hộ để chọn
    /// </summary>
    [HttpPost("tinh-trang-can-ho-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetTinhTrangCanHoForSelector()
    {
        var result = TrangThaiCanHo.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <summary>
    /// Lấy danh sách trạng thái toà nhà để chọn
    /// </summary>
    [HttpPost("trang-thai-toa-nha-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetTrangThaiToaNhaForSelector()
    {
        var result = TrangThaiToaNha.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <summary>
    /// Lấy danh sách trạng thái phương tiện để chọn
    /// </summary>
    [HttpPost("trang-thai-phuong-tien-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetTrangThaiPhuongTienForSelector()
    {
        var result = TrangThaiPhuongTien.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <summary>
    /// Lấy danh sách trạng thái cư trú để chọn
    /// </summary>
    [HttpPost("trang-thai-cu-tru-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetTrangThaiCuTruForSelector()
    {
        var result = TrangThaiCuTru.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <summary>
    /// Lấy danh sách loại giấy tờ để chọn
    /// </summary>
    [HttpPost("loai-giay-to-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetLoaiGiayToForSelector()
    {
        var result = LoaiGiayTo.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <summary>
    /// Lấy danh sách loại yêu cầu để chọn
    /// </summary>
    [HttpPost("loai-yeu-cau-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetLoaiYeuCauForSelector()
    {
        var result = LoaiYeuCau.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }
}
