using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietDienTich;

public record GetChiTietDienTichQuery(int Id) : IQuery<ChiTietDienTichResponse>;
