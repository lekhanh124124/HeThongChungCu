using HeThongChungCu.Application.Features.Catalog.DTOs;
using HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace HeThongChungCu.WebAPI.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
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
    /// - **Hoàn cảnh sử dụng**: Frontend cần hiển thị sơ đồ phân cấp từ Tòa nhà xuống Căn hộ để quản lý hạ tầng hoặc tìm kiếm vị trí.
    /// - **Hệ thống xử lý**: Truy vấn toàn bộ cấu trúc hạ tầng hiện có, tổ chức dữ liệu theo dạng cây (Tree structure) hỗ trợ tìm kiếm theo mã/tên của Tòa nhà, Tầng hoặc Căn hộ qua `Keyword`.
    /// - **Yêu cầu dữ liệu**: 
    ///     - **Bắt buộc**: Không có.
    ///     - **Tùy chọn**: `Keyword`.
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
    /// - **Hoàn cảnh sử dụng**: Cung cấp tùy chọn giới tính cho các form nhập liệu hồ sơ cư dân.
    /// - **Hệ thống xử lý**: Trả về danh sách các giá trị giới tính được định nghĩa sẵn trong hệ thống (Nam, Nữ, Khác).
    /// - **Yêu cầu dữ liệu**: Không có.
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

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp danh sách các loại hình căn hộ (ví dụ: Chung cư, Penthouse, Duplex) cho form quản lý căn hộ.
    /// - **Hệ thống xử lý**: Lấy danh sách các loại căn hộ từ bảng danh mục chuẩn.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
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


    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp tùy chọn loại phương tiện (Xe máy, Ô tô, Xe đạp) khi đăng ký gửi xe.
    /// - **Hệ thống xử lý**: Trả về danh sách các loại phương tiện được hệ thống hỗ trợ.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
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

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp danh sách các vai trò trong căn hộ (Chủ hộ, Thành viên, Khách thuê) khi thiết lập cư trú.
    /// - **Hệ thống xử lý**: Lấy danh sách phân loại quan hệ cư trú từ cấu hình hệ thống.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
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

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Phân loại tầng khi thiết lập hạ tầng tòa nhà (Tầng ở, Tầng thương mại, Hầm).
    /// - **Hệ thống xử lý**: Trả về danh sách các loại tầng được định nghĩa.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
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

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp các trạng thái của căn hộ (Trống, Đang ở, Đang sửa chữa) để quản lý hoặc lọc dữ liệu.
    /// - **Hệ thống xử lý**: Lấy danh sách trạng thái căn hộ hiện hành.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
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

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Hiển thị trạng thái vận hành của tòa nhà (Đang hoạt động, Bảo trì, Chờ bàn giao).
    /// - **Hệ thống xử lý**: Trả về danh sách trạng thái tòa nhà từ cấu hình Enums.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
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

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp trạng thái quản lý phương tiện (Đang hoạt động, Đã hủy, Tạm khóa).
    /// - **Hệ thống xử lý**: Trả về danh sách trạng thái phương tiện.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
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

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Phân loại tình trạng cư trú thực tế của cư dân (Đang cư trú, Đã chuyển đi, Tạm vắng).
    /// - **Hệ thống xử lý**: Lấy danh sách trạng thái cư trú chuẩn.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
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

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Danh mục các loại tài liệu cần thiết (CCCD, Sổ hộ khẩu, Tạm trú) trong form hồ sơ.
    /// - **Hệ thống xử lý**: Trả về danh sách các loại giấy tờ pháp lý được yêu cầu.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
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

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Phân loại các loại yêu cầu (Thêm, Sửa, Xóa) cho quy trình phê duyệt cư trú/phương tiện.
    /// - **Hệ thống xử lý**: Trả về danh sách các loại hình yêu cầu thay đổi dữ liệu.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
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

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp trạng thái xử lý yêu cầu (Chờ duyệt, Đã duyệt, Từ chối, Đã lưu).
    /// - **Hệ thống xử lý**: Trả về danh sách trạng thái của quy trình phê duyệt cư trú.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("trang-thai-yeu-cau-cu-tru-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetTrangThaiYeuCauCuTruForSelector()
    {
        var result = TrangThaiYeuCauCuTru.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp danh sách các loại nhân viên (Kỹ thuật, Vệ sinh, Bảo vệ, Quản lý) cho form quản lý nhân viên.
    /// - **Hệ thống xử lý**: Lấy danh sách các loại nhân viên từ cấu hình Enums.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("loai-nhan-vien-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetLoaiNhanVienForSelector()
    {
        var result = LoaiNhanVien.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp trạng thái của nhân viên (Đang làm việc, Tạm nghỉ, Đã nghỉ việc) cho form quản lý nhân viên.
    /// - **Hệ thống xử lý**: Trả về danh sách trạng thái nhân viên từ cấu hình Enums.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("trang-thai-nhan-vien-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetTrangThaiNhanVienForSelector()
    {
        var result = TrangThaiNhanVien.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp danh sách các loại hình dịch vụ (Điện, Nước, Quản lý,...) để phân loại dịch vụ.
    /// - **Hệ thống xử lý**: Lấy danh sách các loại dịch vụ từ cấu hình Enums.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("loai-dich-vu-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetLoaiDichVuForSelector()
    {
        var result = LoaiDichVu.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp các trạng thái vận hành của dịch vụ (Đang hoạt động, Ngừng cung cấp,...) để quản lý hoặc lọc dữ liệu.
    /// - **Hệ thống xử lý**: Trả về danh sách các trạng thái dịch vụ từ cấu hình Enums.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("trang-thai-dich-vu-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetTrangThaiDichVuForSelector()
    {
        var result = TrangThaiDichVu.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp các trạng thái của hợp đồng (Chờ ký, Hiệu lực, Đã thanh lý,...) để quản lý vòng đời hợp đồng.
    /// - **Hệ thống xử lý**: Lấy danh sách trạng thái hợp đồng từ cấu hình Enums.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("trang-thai-hop-dong-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetTrangThaiHopDongForSelector()
    {
        var result = TrangThaiHopDong.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Phân loại trạng thái phê duyệt đăng ký dịch vụ (Chờ duyệt, Thành công, Từ chối,...).
    /// - **Hệ thống xử lý**: Trả về danh sách trạng thái đăng ký dịch vụ từ cấu hình Enums.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("trang-thai-dang-ky-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetTrangThaiDangKyForSelector()
    {
        var result = TrangThaiDangKy.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp các phương thức định giá dịch vụ (Theo chỉ số, Theo căn hộ, Miễn phí,...) khi thiết lập bảng giá.
    /// - **Hệ thống xử lý**: Lấy danh sách các loại định giá từ cấu hình Enums.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("loai-dinh-gia-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetLoaiDinhGiaForSelector()
    {
        var result = LoaiDinhGia.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Phân loại các loại thông báo gửi đến cư dân (Phí dịch vụ, Bảo trì, Thông báo chung,...).
    /// - **Hệ thống xử lý**: Trả về danh sách các loại thông báo từ cấu hình Enums.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("loai-thong-bao-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetLoaiThongBaoForSelector()
    {
        var result = LoaiThongBao.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }

    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cung cấp danh sách các ngày trong tuần khi thiết lập lịch trình hoặc khung giờ dịch vụ.
    /// - **Hệ thống xử lý**: Trả về danh sách các ngày từ Thứ Hai đến Chủ Nhật từ cấu hình Enums.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("ngay-trong-tuan-for-selector")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ItemForSelectorResponse>>), StatusCodes.Status200OK)]
    public IActionResult GetNgayTrongTuanForSelector()
    {
        var result = NgayTrongTuan.GetAll()
            .Select(x => new ItemForSelectorResponse
            {
                Id = x.Value,
                Name = x.Name
            })
            .ToList();

        return HandleResult(Result.Success<IReadOnlyList<ItemForSelectorResponse>>(result));
    }
}
