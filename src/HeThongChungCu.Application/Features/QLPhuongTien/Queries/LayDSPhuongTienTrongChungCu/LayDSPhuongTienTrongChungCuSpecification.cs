namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSPhuongTienTrongChungCu
{
    public class LayDSPhuongTienTrongChungCuSpecification : BaseSpecification
    {
        public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
        {
            "MaToaNha", "MaTang", "MaCanHo", "TenPhuongTien", "LoaiPhuongTienId", "BienSo", "MauXe", "TrangThaiPhuongTienId"
        };

        public LayDSPhuongTienTrongChungCuSpecification(
            int? toaNhaId,
            int? tangId,
            int? canHoId,
            string? keyword,
            string? maToaNha,
            string? maTang,
            string? maCanHo,
            int? loaiPhuongTienId,
            string? mauXe,
            int? trangThaiPhuongTienId,
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

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                AddKeyword("TenPhuongTien", FilterOperator.Contains, keyword);
                AddKeyword("BienSo", FilterOperator.Contains, keyword);
                AddKeyword("MauXe", FilterOperator.Contains, keyword);
            }

            if (!string.IsNullOrWhiteSpace(maToaNha))
                AddFilter("MaToaNha", FilterOperator.Equal, maToaNha);
            if (!string.IsNullOrWhiteSpace(maTang))
                AddFilter("MaTang", FilterOperator.Equal, maTang);
            if (!string.IsNullOrWhiteSpace(maCanHo))
                AddFilter("MaCanHo", FilterOperator.Equal, maCanHo);

            AddFilter("ToaNhaId", FilterOperator.Equal, toaNhaId);
            AddFilter("TangId", FilterOperator.Equal, tangId);
            AddFilter("CanHoId", FilterOperator.Equal, canHoId);
            AddFilter("LoaiPhuongTienId", FilterOperator.Equal, loaiPhuongTienId);
            AddFilter("MauXe", FilterOperator.Equal, mauXe);
            AddFilter("TrangThaiPhuongTienId", FilterOperator.Equal, trangThaiPhuongTienId);
            AddFilter("IsDeleted", FilterOperator.Equal, false);
        }
    }
}
