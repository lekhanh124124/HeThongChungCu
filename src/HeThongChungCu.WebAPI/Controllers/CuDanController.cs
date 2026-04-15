using HeThongChungCu.Application.Features.CuDan.DTOs;
using HeThongChungCu.Application.Features.CuDan.Queries.LayDSCuTruCuaNguoiDung;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace HeThongChungCu.WebAPI.Controllers
{

    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/cu-dan")]
    public class CuDanController : ApiControllerBase
    {
        private readonly ISender _sender;

        public CuDanController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Lấy quan hệ cư trú của người dùng đang đăng nhập
        /// </summary>
        /// <remarks>
        /// - **Hoàn cảnh sử dụng**: Người dùng muốn kiểm tra danh sách các căn hộ mà mình đang hoặc đã từng cư trú.
        /// - **Hệ thống xử lý**: 
        ///     - Tự động lấy UserId từ phiên đăng nhập hiện tại.
        ///     - Truy vấn tất cả lịch sử quan hệ cư trú (hiện tại và quá khứ) của người dùng này.
        /// - **Yêu cầu dữ liệu**: Không có.
        /// </remarks>
        [HttpPost("quan-he-cu-tru")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<QuanHeCuTruResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetQuanHeCuTru(CancellationToken cancellationToken)
        {
            return HandleResult(await _sender.Send(new LayDSCuTruCuaNguoiDungQuery(), cancellationToken));
        }

        /// <summary>
        /// Lấy thông tin chi tiết cư dân và quan hệ cư trú
        /// </summary>
        /// <remarks>
        /// - **Hoàn cảnh sử dụng**: Tra cứu thông tin hồ sơ chi tiết của một cư dân dựa trên quan hệ cư trú cụ thể.
        /// - **Hệ thống xử lý**: Kết hợp dữ liệu từ hồ sơ người dùng (NguoiDung) và chi tiết quan hệ cư trú (QuanHeCuTru).
        /// - **Yêu cầu dữ liệu**: 
        ///     - **Bắt buộc**: `QuanHeCuTruId`.
        /// </remarks>
        [HttpPost("thong-tin")]
        [ProducesResponseType(typeof(ApiResponse<LayThongTinCuDanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LayThongTinCuDan([FromBody] LayThongTinCuDanQuery query, CancellationToken cancellationToken)
        {
            return HandleResult(await _sender.Send(query, cancellationToken));
        }

        /// <summary>
        /// Lấy danh sách thành viên cư trú hiện tại trong căn hộ
        /// </summary>
        /// <remarks>
        /// - **Hoàn cảnh sử dụng**: Chủ hộ hoặc BQL muốn xem danh sách tất cả những người đang thực tế sinh sống trong cùng một căn hộ.
        /// - **Hệ thống xử lý**: Truy xuất toàn bộ danh sách cư dân có trạng thái cư trú là "Đang cư trú" (`DangCuTru`) tại căn hộ được chỉ định.
        /// - **Yêu cầu dữ liệu**: 
        ///     - **Bắt buộc**: `CanHoId`.
        /// </remarks>
        [HttpPost("thanh-vien-cu-tru")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ThanhVienCuTruResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LayThanhVienCuTru([FromBody] LayThanhVienCuTruQuery query, CancellationToken cancellationToken)
        {
            return HandleResult(await _sender.Send(query, cancellationToken));
        }
    }
}
