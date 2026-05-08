using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public class BieuQuyetCuDan : AggregateRoot
{
    public int KhaoSatId { get; private set; }
    public KhaoSat KhaoSat { get; private set; } = null!;
    public int CanHoId { get; private set; }

    public decimal TrongSoBieuQuyet { get; private set; }
    public bool IsOtpVerified { get; private set; }

    private readonly List<ChiTietBieuQuyet> _chiTiets = [];
    public IReadOnlyCollection<ChiTietBieuQuyet> ChiTiets => _chiTiets.AsReadOnly();

    private BieuQuyetCuDan() : base() { } // EF Core

    private BieuQuyetCuDan(
        int khaoSatId,
        int canHoId,
        decimal trongSo,
        bool isOtpVerified)
    {
        KhaoSatId = khaoSatId;
        CanHoId = canHoId;
        TrongSoBieuQuyet = trongSo;
        IsOtpVerified = isOtpVerified;
    }

    public static Result<BieuQuyetCuDan> Create(
        int khaoSatId,
        int canHoId,
        decimal dienTichCanHo,
        CoCheTinhDiemBauCu coChe,
        List<(int luaChonId, string? noiDungTuDo)> traLois,
        bool isOtpVerified = false)
    {
        var trongSo = coChe == CoCheTinhDiemBauCu.TheoDienTichSoHuu ? dienTichCanHo : 1.0m;
        var bieuQuyet = new BieuQuyetCuDan(khaoSatId, canHoId, trongSo, isOtpVerified);

        foreach (var (luaChonId, noiDungTuDo) in traLois)
        {
            bieuQuyet._chiTiets.Add(ChiTietBieuQuyet.Create(luaChonId, noiDungTuDo));
        }

        return Result.Success(bieuQuyet);
    }
}
