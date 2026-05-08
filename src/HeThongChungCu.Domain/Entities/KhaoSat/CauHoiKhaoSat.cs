using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Domain.Entities;

public class CauHoiKhaoSat : AuditableEntity
{
    public int KhaoSatId { get; private set; }
    public KhaoSat KhaoSat { get; private set; } = null!;
    public string NoiDungCauHoi { get; private set; } = null!;
    public bool IsBatBuoc { get; private set; }
    public bool IsMultiSelect { get; private set; }

    private readonly List<LuaChonKhaoSat> _luaChons = [];
    public IReadOnlyCollection<LuaChonKhaoSat> LuaChons => _luaChons.AsReadOnly();

    private CauHoiKhaoSat() : base() { } // EF Core

    private CauHoiKhaoSat(string noiDung, bool isBatBuoc, bool isMultiSelect)
    {
        NoiDungCauHoi = noiDung;
        IsBatBuoc = isBatBuoc;
        IsMultiSelect = isMultiSelect;
    }

    public static Result<CauHoiKhaoSat> Create(string noiDung, bool isBatBuoc, bool isMultiSelect, List<string> danhSachLuaChon)
    {
        if (danhSachLuaChon == null || danhSachLuaChon.Count < 2)
            return Result.Failure<CauHoiKhaoSat>(KhaoSatErrors.NotEnoughOptions);

        var ch = new CauHoiKhaoSat(noiDung, isBatBuoc, isMultiSelect);

        foreach (var item in danhSachLuaChon)
        {
            ch._luaChons.Add(LuaChonKhaoSat.Create(item));
        }

        return Result.Success(ch);
    }

    public static Result<CauHoiKhaoSat> Create(
        string noiDung,
        bool isBatBuoc,
        bool isMultiSelect,
        List<(string NoiDung, bool IsUngVien, string? TieuSu, int? UngVienId)> danhSachLuaChon)
    {
        if (danhSachLuaChon == null || danhSachLuaChon.Count < 2)
            return Result.Failure<CauHoiKhaoSat>(KhaoSatErrors.NotEnoughOptions);

        var ch = new CauHoiKhaoSat(noiDung, isBatBuoc, isMultiSelect);

        foreach (var item in danhSachLuaChon)
        {
            ch._luaChons.Add(LuaChonKhaoSat.Create(item.NoiDung, item.IsUngVien, item.TieuSu, item.UngVienId));
        }

        return Result.Success(ch);
    }
}
