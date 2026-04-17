using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class NhanSuThiCong : NhanSuYeuCau
{
    private NhanSuThiCong() : base() { } // EF Core

    internal NhanSuThiCong(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null, int? nhanVienId = null)
        : base(LoaiNhanSuYeuCau.ThiCong, hoTen, soCCCD, soDienThoai, vaiTro, ghiChu, nhanVienId)
    {
    }

    internal static NhanSuThiCong Create(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null, int? nhanVienId = null)
    {
        return new NhanSuThiCong(hoTen, soCCCD, soDienThoai, vaiTro, ghiChu, nhanVienId);
    }
}
