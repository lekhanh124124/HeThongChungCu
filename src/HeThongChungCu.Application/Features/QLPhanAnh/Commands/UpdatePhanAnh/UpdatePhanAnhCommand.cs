using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.UpdatePhanAnh;

public record UpdatePhanAnhCommand : ICommand<PhanAnhResponse>
{
    public int Id { get; init; }
    public string? TieuDe { get; init; }
    public string? NoiDung { get; init; }
    public int? LoaiPhanAnhId { get; init; }
    public List<int>? DanhSachTepIds { get; init; }
    public bool IsWithdraw { get; init; }
    public bool IsSubmit { get; init; }
}
