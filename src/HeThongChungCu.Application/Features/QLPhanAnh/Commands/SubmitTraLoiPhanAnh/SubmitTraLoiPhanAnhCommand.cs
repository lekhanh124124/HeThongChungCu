using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.SubmitTraLoiPhanAnh;

public record SubmitTraLoiPhanAnhCommand : ICommand<PhanAnhResponse>
{
    public int PhanAnhId { get; init; }
    public string NoiDung { get; init; } = string.Empty;
}
