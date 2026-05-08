using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public class ChiTietBieuQuyet : AuditableEntity
{
    public int BieuQuyetCuDanId { get; private set; }
    public BieuQuyetCuDan BieuQuyetCuDan { get; private set; } = null!;
    public int LuaChonKhaoSatId { get; private set; }
    public LuaChonKhaoSat LuaChonKhaoSat { get; private set; } = null!;
    public string? NoiDungTraLoiTuDo { get; private set; }

    private ChiTietBieuQuyet() : base() { } // EF Core

    private ChiTietBieuQuyet(int luaChonId, string? noiDungTuDo)
    {
        LuaChonKhaoSatId = luaChonId;
        NoiDungTraLoiTuDo = noiDungTuDo;
    }

    public static ChiTietBieuQuyet Create(int luaChonId, string? noiDungTuDo)
    {
        return new ChiTietBieuQuyet(luaChonId, noiDungTuDo);
    }
}
