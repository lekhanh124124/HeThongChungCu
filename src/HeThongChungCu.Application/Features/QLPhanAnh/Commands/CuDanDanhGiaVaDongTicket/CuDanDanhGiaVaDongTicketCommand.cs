using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.CuDanDanhGiaVaDongTicket;

public record CuDanDanhGiaVaDongTicketCommand : ICommand<PhanAnhResponse>
{
    public int PhanAnhId { get; init; }
    public int DiemDanhGia { get; init; }
    public string? NhanXetDanhGia { get; init; }
}
