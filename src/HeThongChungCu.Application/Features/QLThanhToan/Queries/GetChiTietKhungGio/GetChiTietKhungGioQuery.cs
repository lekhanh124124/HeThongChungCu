using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietKhungGio;

public record GetChiTietKhungGioQuery(int Id) : IQuery<ChiTietKhungGioResponse>;
