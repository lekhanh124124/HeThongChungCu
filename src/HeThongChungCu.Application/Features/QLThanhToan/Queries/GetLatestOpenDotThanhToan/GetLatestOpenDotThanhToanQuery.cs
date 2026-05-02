using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetLatestOpenDotThanhToan;

public record GetLatestOpenDotThanhToanQuery(int Thang, int Nam) : IQuery<DotThanhToanDetailResponse>;
