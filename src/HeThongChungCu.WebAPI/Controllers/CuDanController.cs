using HeThongChungCu.Application.Features.CuDan.DTOs;
using HeThongChungCu.Application.Features.CuDan.Queries.LayDSCuTruCuaNguoiDung;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;
using HeThongChungCu.WebAPI.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers
{

    [Authorize]
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
        /// API trả về danh sách các quan hệ cư trú đang hoạt động của người dùng, bao gồm thông tin căn hộ.
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
        /// API trả về danh sách những cư dân đang sinh sống (chưa kết thúc cư trú) tại một căn hộ xác định bởi `CanHoId`.
        /// Bao gồm thông tin: Tên, Loại quan hệ (Chủ hộ, Vợ/Chồng...), Ngày bắt đầu và Ảnh đại diện.
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
