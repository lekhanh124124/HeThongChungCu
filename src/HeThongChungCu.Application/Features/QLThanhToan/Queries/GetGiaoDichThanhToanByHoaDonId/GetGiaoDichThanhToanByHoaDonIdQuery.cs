using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetGiaoDichThanhToanByHoaDonId;

public record GetGiaoDichThanhToanByHoaDonIdQuery : IQuery<List<GiaoDichThanhToanResponse>>
{
    public int HoaDonId { get; init; }
}
