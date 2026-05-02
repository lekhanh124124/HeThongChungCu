using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetDotThanhToanById;

public record GetDotThanhToanByIdQuery(int Id) : IQuery<DotThanhToanDetailResponse>;
