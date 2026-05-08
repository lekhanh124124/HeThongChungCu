using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.HuyPhanAnh;

public record HuyPhanAnhCommand : ICommand<PhanAnhResponse>
{
    public int PhanAnhId { get; init; }
    public string LyDoHuy { get; init; } = string.Empty;
}
