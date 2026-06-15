using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Domain.Entities;

public class KhaoSat : AggregateRoot
{
    public string TieuDe { get; private set; } = null!;
    public string MoTa { get; private set; } = null!;
    public LoaiKhaoSat LoaiKhaoSatId { get; private set; } = null!;
    public CoCheTinhDiemBauCu CoCheTinhDiemId { get; private set; } = null!;
    public TrangThaiKhaoSat TrangThaiId { get; private set; } = null!;
    
    public DateTimeOffset NgayBatDau { get; private set; }
    public DateTimeOffset NgayKetThuc { get; private set; }
    
    public decimal TyleThamGiaToiThieu { get; private set; } = 50.0m; // % tối thiểu căn hộ tham gia để biểu quyết hợp chuẩn
    public decimal TyLeDongYToiThieu { get; private set; } = 50.0m;  // % tối thiểu phiếu đồng ý để thông qua nghị quyết
    public bool IsAnDanh { get; private set; } // Ẩn danh căn hộ khi biểu diễn kết quả biểu quyết

    private readonly List<CauHoiKhaoSat> _cauHois = [];
    public IReadOnlyCollection<CauHoiKhaoSat> CauHois => _cauHois.AsReadOnly();

    private readonly List<BieuQuyetCuDan> _bieuQuyets = [];
    public IReadOnlyCollection<BieuQuyetCuDan> BieuQuyets => _bieuQuyets.AsReadOnly();

    private KhaoSat() : base() { } // EF Core

    private KhaoSat(
        string tieuDe,
        string moTa,
        LoaiKhaoSat loaiKhaoSat,
        CoCheTinhDiemBauCu coChe,
        DateTimeOffset tuNgay,
        DateTimeOffset denNgay,
        bool isAnDanh = false)
    {
        TieuDe = tieuDe;
        MoTa = moTa;
        LoaiKhaoSatId = loaiKhaoSat;
        CoCheTinhDiemId = coChe;
        TrangThaiId = TrangThaiKhaoSat.MoiTao;
        NgayBatDau = tuNgay;
        NgayKetThuc = denNgay;
        IsAnDanh = isAnDanh;
    }

    public static Result<KhaoSat> Create(
        string tieuDe,
        string moTa,
        LoaiKhaoSat loaiKhaoSat,
        CoCheTinhDiemBauCu coChe,
        DateTimeOffset tuNgay,
        DateTimeOffset denNgay,
        bool isAnDanh = false)
    {
        if (denNgay <= tuNgay)
            return Result.Failure<KhaoSat>(KhaoSatErrors.InvalidDateRange);

        var campaign = new KhaoSat(tieuDe, moTa, loaiKhaoSat, coChe, tuNgay, denNgay, isAnDanh);
        return Result.Success(campaign);
    }

    public Result Update(
        string tieuDe,
        string moTa,
        LoaiKhaoSat loaiKhaoSat,
        CoCheTinhDiemBauCu coChe,
        DateTimeOffset tuNgay,
        DateTimeOffset denNgay,
        bool isAnDanh)
    {
        if (TrangThaiId != TrangThaiKhaoSat.MoiTao)
            return Result.Failure(KhaoSatErrors.NotDraftStatus);

        if (denNgay <= tuNgay)
            return Result.Failure(KhaoSatErrors.InvalidDateRange);

        TieuDe = tieuDe;
        MoTa = moTa;
        LoaiKhaoSatId = loaiKhaoSat;
        CoCheTinhDiemId = coChe;
        NgayBatDau = tuNgay;
        NgayKetThuc = denNgay;
        IsAnDanh = isAnDanh;

        return Result.Success();
    }

    public Result ClearQuestions()
    {
        if (TrangThaiId != TrangThaiKhaoSat.MoiTao)
            return Result.Failure(KhaoSatErrors.NotDraftStatus);

        _cauHois.Clear();
        return Result.Success();
    }

    public Result PublicCampaign()
    {
        if (TrangThaiId != TrangThaiKhaoSat.MoiTao)
            return Result.Failure(KhaoSatErrors.NotDraftStatus);
        
        if (_cauHois.Count == 0)
            return Result.Failure(KhaoSatErrors.NoQuestions);

        TrangThaiId = TrangThaiKhaoSat.DangDienRa;
        AddDomainEvent(new KhaoSatPublishedEvent(Id, TieuDe));
        return Result.Success();
    }

    public Result EndCampaign()
    {
        if (TrangThaiId != TrangThaiKhaoSat.DangDienRa)
            return Result.Failure(KhaoSatErrors.InvalidStatus);

        TrangThaiId = TrangThaiKhaoSat.DaKetThuc;
        return Result.Success();
    }

    public Result ThemCauHoi(string noiDung, bool isBatBuoc, bool isMultiSelect, List<string> luaChons)
    {
        if (TrangThaiId != TrangThaiKhaoSat.MoiTao)
            return Result.Failure(KhaoSatErrors.NotDraftStatus);

        var creationResult = CauHoiKhaoSat.Create(noiDung, isBatBuoc, isMultiSelect, luaChons);
        if (creationResult.IsFailure)
            return Result.Failure(creationResult.Errors);

        _cauHois.Add(creationResult.Value);
        return Result.Success();
    }

    public Result ThemCauHoi(
        string noiDung,
        bool isBatBuoc,
        bool isMultiSelect,
        List<(string NoiDung, bool IsUngVien, string? TieuSu, int? UngVienId)> luaChons)
    {
        if (TrangThaiId != TrangThaiKhaoSat.MoiTao)
            return Result.Failure(KhaoSatErrors.NotDraftStatus);

        var creationResult = CauHoiKhaoSat.Create(noiDung, isBatBuoc, isMultiSelect, luaChons);
        if (creationResult.IsFailure)
            return Result.Failure(creationResult.Errors);

        _cauHois.Add(creationResult.Value);
        return Result.Success();
    }
}
