using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.UpdateYeuCauThiCong;

public record UpdateYeuCauThiCongCommand : ICommand<YeuCauThiCongResponse>
{
    public int Id { get; init; }
    public string? HangMucThiCong { get; init; }
    public DateTimeOffset? DuKienBatDau { get; init; }
    public DateTimeOffset? DuKienKetThuc { get; init; }
    public string? NoiDung { get; init; }
    public string? TenDonViThiCong { get; init; }
    public string? NguoiDaiDien { get; init; }
    public string? SoDienThoaiDaiDien { get; init; }
    public List<NhanSuThiCongRequest>? DanhSachNhanSu { get; init; }
    public List<int>? DanhSachTepIds { get; init; }
    public bool IsSubmit { get; init; }
    public bool IsWithdraw { get; init; }
}
