using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.CreateBangGia;

public record CreateBangGiaCommand : ICommand<BangGiaResponse>
{
    public int DichVuId { get; init; }
    public string TenBangGia { get; init; } = string.Empty;
    public DateTimeOffset NgayApDung { get; init; }
    public DateTimeOffset? NgayKetThuc { get; init; }
    public int LoaiDinhGiaId { get; init; }
    public bool IsDinhKy { get; init; }
    public decimal? DonGiaCoDinh { get; init; } // For CoDinh
    public List<CreateChiTietGiaLuyTienDto> GiaLuyTiens { get; init; } = [];
    public List<CreateChiTietGiaKhungGioDto> GiaKhungGios { get; init; } = [];
    public List<CreateChiTietGiaLoaiCanHoDto> GiaLoaiCanHos { get; init; } = [];
}

public record CreateChiTietGiaLuyTienDto(decimal TuMuc, decimal? DenMuc, decimal DonGia);
public record CreateChiTietGiaKhungGioDto(int KhungGioId, decimal DonGia);
public record CreateChiTietGiaLoaiCanHoDto(int? LoaiCanHoId, decimal DonGia);
