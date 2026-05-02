using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;

public record GetHoaDonByIdQuery(int Id) : IQuery<HoaDonDetailResponse>;
