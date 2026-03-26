namespace HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSCuDanTrongChungCu;

public class LayDSCuDanTrongChungCuQuerySpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "MaToaNha", "MaTang", "MaCanHo", "HoTen", "SoDienThoai", "LoaiQuanHeCuTruId", "NgayBatDau", "NgayKetThuc", "TrangThaiCuTruId"
    };

    public LayDSCuDanTrongChungCuQuerySpecification(
        int? toaNhaId, 
        int? tangId, 
        int? canHoId,
        string? keyword,
        string? maToaNha,
        string? maTang,
        string? maCanHo,
        int? loaiQuanHeCuTruId,
        int? trangThaiCuTruId,
        DateTime? ngayBatDauFrom,
        DateTime? ngayBatDauTo,
        DateTime? ngayKetThucFrom,
        DateTime? ngayKetThucTo,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) : base(sortCol, isAsc, pageNumber, pageSize)
    {
        if (toaNhaId == 0)
            toaNhaId = null;

        if (tangId == 0)
            tangId = null;

        if (canHoId == 0)
            canHoId = null;

        AddFilter("ToaNhaId", FilterOperator.Equal, toaNhaId);
        AddFilter("TangId", FilterOperator.Equal, tangId);
        AddFilter("CanHoId", FilterOperator.Equal, canHoId);
        AddFilter("LoaiQuanHeCuTruId", FilterOperator.Equal, loaiQuanHeCuTruId);
        AddFilter("TrangThaiCuTruId", FilterOperator.Equal, trangThaiCuTruId);
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (!string.IsNullOrWhiteSpace(maToaNha))
            AddFilter("MaToaNha", FilterOperator.Equal, maToaNha);
        if (!string.IsNullOrWhiteSpace(maTang))
            AddFilter("MaTang", FilterOperator.Equal, maTang);
        if (!string.IsNullOrWhiteSpace(maCanHo))
            AddFilter("MaCanHo", FilterOperator.Equal, maCanHo);

        // Date range filters
        if (ngayBatDauFrom.HasValue) 
            AddFilter("NgayBatDau", FilterOperator.GreaterThanOrEqual, ngayBatDauFrom);
        if (ngayBatDauTo.HasValue) 
            AddFilter("NgayBatDau", FilterOperator.LessThanOrEqual, ngayBatDauTo);
        if (ngayKetThucFrom.HasValue) 
            AddFilter("NgayKetThuc", FilterOperator.GreaterThanOrEqual, ngayKetThucFrom);
        if (ngayKetThucTo.HasValue) 
            AddFilter("NgayKetThuc", FilterOperator.LessThanOrEqual, ngayKetThucTo);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("HoTen", FilterOperator.Contains, keyword);
            AddKeyword("MaToaNha", FilterOperator.Contains, keyword);
            AddKeyword("MaTang", FilterOperator.Contains, keyword);
            AddKeyword("MaCanHo", FilterOperator.Contains, keyword);
        }
    }
}
