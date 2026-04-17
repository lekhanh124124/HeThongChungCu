using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Entities;

public class NhanSuSuaChua : NhanSuYeuCau
{
    private NhanSuSuaChua() : base() { } // EF Core

    internal NhanSuSuaChua(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null, int? nhanVienId = null)
        : base(LoaiNhanSuYeuCau.SuaChua, hoTen, soCCCD, soDienThoai, vaiTro, ghiChu, nhanVienId)
    {
    }

    internal static NhanSuSuaChua Create(string hoTen, string soCCCD, string? soDienThoai, string? vaiTro, string? ghiChu = null, int? nhanVienId = null)
    {
        return new NhanSuSuaChua(hoTen, soCCCD, soDienThoai, vaiTro, ghiChu, nhanVienId);
    }
}
