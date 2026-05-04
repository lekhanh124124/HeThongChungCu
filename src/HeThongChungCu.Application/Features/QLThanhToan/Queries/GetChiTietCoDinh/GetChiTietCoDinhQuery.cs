using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLThanhToan.DTOs;

namespace HeThongChungCu.Application.Features.QLThanhToan.Queries.GetChiTietCoDinh;

public record GetChiTietCoDinhQuery(int Id) : IQuery<ChiTietCoDinhResponse>;
