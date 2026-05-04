using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietLuyTien;

public record GetChiTietLuyTienQuery(int Id) : IQuery<ChiTietLuyTienResponse>;
