using System.Collections.Generic;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.CreatePhanAnh;

public record CreatePhanAnhCommand : ICommand<PhanAnhResponse>
{
    public int CanHoId { get; init; }
    public string TieuDe { get; init; } = string.Empty;
    public string NoiDung { get; init; } = string.Empty;
    public int LoaiPhanAnhId { get; init; }
    public List<int>? DanhSachTepIds { get; init; }
    public bool IsSubmit { get; init; } = true;
}
