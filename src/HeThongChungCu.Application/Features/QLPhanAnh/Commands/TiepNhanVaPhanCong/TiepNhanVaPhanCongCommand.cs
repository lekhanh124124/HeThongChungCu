using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.TiepNhanVaPhanCong;

public record TiepNhanVaPhanCongCommand : ICommand<PhanAnhResponse>
{
    public int PhanAnhId { get; init; }
}
