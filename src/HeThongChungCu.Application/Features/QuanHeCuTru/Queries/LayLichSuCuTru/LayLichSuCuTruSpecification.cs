namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayLichSuCuTru;

public class LayLichSuCuTruSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "NgayBatDau",
        "NgayKetThuc",
        "MaCanHo",
        "LoaiQuanHeCuTruId"
    };
    public LayLichSuCuTruSpecification(
        int userId,
        int? loaiQuanHeCuTruId,
        DateOnly? ngayBatDauFrom,
        DateOnly? ngayBatDauTo,
        DateOnly? ngayKetThucFrom,
        DateOnly? ngayKetThucTo,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) 
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("UserId", FilterOperator.Equal, userId);
        AddFilter("LoaiQuanHeCuTruId", FilterOperator.Equal, loaiQuanHeCuTruId);
        AddFilter("IsKetThuc", FilterOperator.Equal, true);
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        // Date range filters
        if (ngayBatDauFrom.HasValue) 
            AddFilter("NgayBatDau", FilterOperator.GreaterThanOrEqual, ngayBatDauFrom);
        if (ngayBatDauTo.HasValue) 
            AddFilter("NgayBatDau", FilterOperator.LessThanOrEqual, ngayBatDauTo);
        if (ngayKetThucFrom.HasValue) 
            AddFilter("NgayKetThuc", FilterOperator.GreaterThanOrEqual, ngayKetThucFrom);
        if (ngayKetThucTo.HasValue) 
            AddFilter("NgayKetThuc", FilterOperator.LessThanOrEqual, ngayKetThucTo);
    }
}
