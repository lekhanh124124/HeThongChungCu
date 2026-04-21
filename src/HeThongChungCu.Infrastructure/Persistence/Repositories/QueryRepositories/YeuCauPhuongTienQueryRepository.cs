using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetYeuCauPhuongTienById;
using HeThongChungCu.Application.Features.QLPhuongTien.Queries.LayDSYeuCauPhuongTien;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class YeuCauPhuongTienQueryRepository : IYeuCauPhuongTienQueryRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauPhuongTienQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<DSYeuCauPhuongTienResponse>> GetPagedListAsync(
        LayDSYeuCauPhuongTienQuerySpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "CanHoId", "y.CanHoId" },
            { "LoaiYeuCauId", "y.LoaiHanhDongYeuCauId" },
            { "TrangThaiId", "y.TrangThaiId" },
            { "CreatedAt", "y.CreatedAt" },
            { "ToaNhaId", "tg.ToaNhaId" },
            { "TangId", "ch.TangId" },
            { "TenNguoiGui", "ISNULL(nd1.Ho + ' ' + nd1.Ten, '')" },
            { "TenNguoiXuLy", "ISNULL(nd2.Ho + ' ' + nd2.Ten, '')" },
            { "YeuCauBienSo", "y.YeuCauBienSo" },
            { "IsDeleted", "y.IsDeleted" },
            { "LoaiYeuCauCuDan", "y.LoaiYeuCauCuDanId" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("CanHo", "ch", "ch.Id = y.CanHoId", JoinType.Inner,
                Mapping: new() {
                    { "CanHoIsDeleted", "ch.IsDeleted" } }),
            new JoinDefinition("Tang", "tg", "tg.Id = ch.TangId", JoinType.Inner,
                Mapping: new() {
                    { "TangIsDeleted", "tg.IsDeleted" } }),
            new JoinDefinition("ToaNha", "tn", "tn.Id = tg.ToaNhaId", JoinType.Inner,
                Mapping: new() {
                    { "ToaNhaIsDeleted", "tn.IsDeleted" } }),
            new JoinDefinition("NguoiDung", "nd1", "nd1.Id = y.CreatedBy", JoinType.Left),
            new JoinDefinition("TaiKhoan", "tk1", "tk1.NguoiDungId = nd1.Id", JoinType.Left,
                Mapping: new() {
                    { "TaiKhoanIsActive", "tk1.IsActive" },
                    { "TaiKhoanIsDeleted", "tk1.IsDeleted" } }),
            new JoinDefinition("NguoiDung", "nd2", "nd2.Id = y.NguoiXuLyId", JoinType.Left),
            new JoinDefinition("TaiKhoan", "tk2", "tk2.NguoiDungId = nd2.Id", JoinType.Left,
                Mapping: new() {
                    { "TaiKhoanIsActive", "tk2.IsActive" },
                    { "TaiKhoanIsDeleted", "tk2.IsDeleted" } })
        ], parameters);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "CreatedAt");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                y.Id,
                y.CanHoId,
                y.YeuCauPhuongTienId,
                y.LoaiHanhDongYeuCauId,
                y.TrangThaiId,
                y.LyDo,
                y.NoiDung,
                y.CreatedAt,
                y.NgayXuLy,
                y.NguoiXuLyId,
                y.CreatedBy,
                y.YeuCauTenPhuongTien,
                y.YeuCauLoaiPhuongTienId,
                y.YeuCauBienSo,
                y.YeuCauMauXe,
                ch.TenCanHo,
                tg.TenTang,
                tn.TenToaNha,
                {columnMapping["TenNguoiGui"]} AS TenNguoiGui,
                {columnMapping["TenNguoiXuLy"]} AS TenNguoiXuLy
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var transaction = _dbContext.GetDbTransaction();
        var rows = (await connection.QueryAsync<YeuCauPhuongTienReadModel>(sql, parameters, transaction: transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiYeuCauMap = LoaiHanhDongYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();

        var items = rows.Select(r => new DSYeuCauPhuongTienResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            TenCanHo = r.TenCanHo,
            TenTang = r.TenTang,
            TenToaNha = r.TenToaNha,
            LoaiYeuCauId = r.LoaiHanhDongYeuCauId,
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(r.LoaiHanhDongYeuCauId, string.Empty),
            TrangThaiId = r.TrangThaiId,
            TenTrangThai = trangThaiMap.GetValueOrDefault(r.TrangThaiId, string.Empty),
            LyDo = r.LyDo,
            NoiDung = r.NoiDung,
            CreatedAt = r.CreatedAt,
            NgayXuLy = r.NgayXuLy,
            NguoiXuLyId = r.NguoiXuLyId,
            CreatedBy = r.CreatedBy,
            TenNguoiGui = r.TenNguoiGui,
            TenNguoiXuLy = r.TenNguoiXuLy,
            YeuCauTenPhuongTien = r.YeuCauTenPhuongTien,
            YeuCauBienSo = r.YeuCauBienSo
        }).ToList();

        return new PagedResult<DSYeuCauPhuongTienResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize = spec.PageSize ?? items.Count,
                TotalItems = totalCount
            }
        };
    }

    public async Task<YeuCauPhuongTienResponse?> GetByIdAsync(GetYeuCauPhuongTienByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "IsDeleted", "y.IsDeleted" },
            { "LoaiYeuCauId", "y.LoaiHanhDongYeuCauId" },
            { "LoaiYeuCauCuDan", "y.LoaiYeuCauCuDanId" },
            { "TenNguoiGui", "ISNULL(nd1.Ho + ' ' + nd1.Ten, '')" },
            { "TenNguoiXuLy", "ISNULL(nd2.Ho + ' ' + nd2.Ten, '')" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("CanHo", "ch", "ch.Id = y.CanHoId", JoinType.Inner, Mapping: new() { { "CanHoIsDeleted", "ch.IsDeleted" } }),
            new JoinDefinition("Tang", "tg", "tg.Id = ch.TangId", JoinType.Inner, Mapping: new() { { "TangIsDeleted", "tg.IsDeleted" } }),
            new JoinDefinition("ToaNha", "tn", "tn.Id = tg.ToaNhaId", JoinType.Inner, Mapping: new() { { "ToaNhaIsDeleted", "tn.IsDeleted" } }),
            new JoinDefinition("NguoiDung", "nd1", "nd1.Id = y.CreatedBy", JoinType.Left),
            new JoinDefinition("TaiKhoan", "tk1", "tk1.NguoiDungId = nd1.Id", JoinType.Left, Mapping: new() { { "TaiKhoanIsActive", "tk1.IsActive" }, { "TaiKhoanIsDeleted", "tk1.IsDeleted" } }),
            new JoinDefinition("NguoiDung", "nd2", "nd2.Id = y.NguoiXuLyId", JoinType.Left),
            new JoinDefinition("TaiKhoan", "tk2", "tk2.NguoiDungId = nd2.Id", JoinType.Left, Mapping: new() { { "TaiKhoanIsActive", "tk2.IsActive" }, { "TaiKhoanIsDeleted", "tk2.IsDeleted" } })
        ], parameters);

        var sqlJoinsTtl = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("TepTaiLieu", "ttl", "ttl.YeuCauId = y.Id", Mapping: new()
            {
                { "TepIsDeleted", "ttl.IsDeleted" },
                { "LoaiTepYeuCauPhuongTien", "ttl.LoaiTepId" }
            })
        ], parameters);

        var tenNguoiGuiSql = columnMapping["TenNguoiGui"];
        var tenNguoiXuLySql = columnMapping["TenNguoiXuLy"];

        var sql = $"""
            -- 1. Main Info
            SELECT
                y.Id, y.CanHoId, y.YeuCauPhuongTienId, y.LoaiHanhDongYeuCauId, y.TrangThaiId, y.LyDo, y.NoiDung, 
                y.CreatedAt, y.NgayXuLy, y.NguoiXuLyId, y.CreatedBy,
                y.YeuCauTenPhuongTien, y.YeuCauLoaiPhuongTienId, y.YeuCauBienSo, y.YeuCauMauXe,
                ch.TenCanHo, tg.TenTang, tn.TenToaNha,
                {tenNguoiGuiSql} AS TenNguoiGui,
                {tenNguoiXuLySql} AS TenNguoiXuLy
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere};

            -- 2. Detail Info (1-N)
            SELECT 
                ttl.Id, ttl.FileUrl, ttl.FileName, ttl.ContentType
            FROM YeuCau y
            {sqlJoinsTtl}
            {sqlWhere};
            """;

        var transaction = _dbContext.GetDbTransaction();
        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: transaction);

        var readModel = await multi.ReadFirstOrDefaultAsync<YeuCauPhuongTienReadModel>();
        if (readModel == null) return null;

        var loaiYeuCauMap = LoaiHanhDongYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();
        var loaiPhuongTienMap = LoaiPhuongTien.ToDictionary();

        var images = (await multi.ReadAsync<YeuCauPhuongTienFileReadModel>()).ToList();

        return new YeuCauPhuongTienResponse
        {
            Id = readModel.Id,
            CreatedBy = readModel.CreatedBy,
            TenNguoiGui = readModel.TenNguoiGui,
            CreatedAt = readModel.CreatedAt,
            CanHoId = readModel.CanHoId,
            TenCanHo = readModel.TenCanHo,
            TenTang = readModel.TenTang,
            TenToaNha = readModel.TenToaNha,
            NguoiXuLyId = readModel.NguoiXuLyId,
            TenNguoiXuLy = readModel.TenNguoiXuLy,
            NgayXuLy = readModel.NgayXuLy,
            PhuongTienId = readModel.YeuCauPhuongTienId,
            LoaiYeuCauId = readModel.LoaiHanhDongYeuCauId,
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(readModel.LoaiHanhDongYeuCauId, string.Empty),
            TrangThaiId = readModel.TrangThaiId,
            TenTrangThai = trangThaiMap.GetValueOrDefault(readModel.TrangThaiId, string.Empty),
            NoiDung = readModel.NoiDung,
            LyDo = readModel.LyDo,
            YeuCauTenPhuongTien = readModel.YeuCauTenPhuongTien,
            YeuCauLoaiPhuongTienId = readModel.YeuCauLoaiPhuongTienId,
            TenYeuCauLoaiPhuongTien = loaiPhuongTienMap.GetValueOrDefault(readModel.YeuCauLoaiPhuongTienId, string.Empty),
            YeuCauBienSo = readModel.YeuCauBienSo,
            YeuCauMauXe = readModel.YeuCauMauXe,
            YeuCauHinhAnhPhuongTiens = images.Select(f => new TepTaiLieuResponse(f.Id, f.FileUrl, f.FileName, f.ContentType)).ToList()
        };
    }

    public async Task<DSYeuCauPhuongTienResponse?> GetListResponseByIdAsync(GetYeuCauPhuongTienByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "IsDeleted", "y.IsDeleted" },
            { "LoaiYeuCauId", "y.LoaiHanhDongYeuCauId" },
            { "LoaiYeuCauCuDan", "y.LoaiYeuCauCuDanId" },
            { "TenNguoiGui", "ISNULL(nd1.Ho + ' ' + nd1.Ten, '')" },
            { "TenNguoiXuLy", "ISNULL(nd2.Ho + ' ' + nd2.Ten, '')" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("CanHo", "ch", "ch.Id = y.CanHoId", JoinType.Inner, Mapping: new() { { "CanHoIsDeleted", "ch.IsDeleted" } }),
            new JoinDefinition("Tang", "tg", "tg.Id = ch.TangId", JoinType.Inner, Mapping: new() { { "TangIsDeleted", "tg.IsDeleted" } }),
            new JoinDefinition("ToaNha", "tn", "tn.Id = tg.ToaNhaId", JoinType.Inner, Mapping: new() { { "ToaNhaIsDeleted", "tn.IsDeleted" } }),
            new JoinDefinition("NguoiDung", "nd1", "nd1.Id = y.CreatedBy", JoinType.Left),
            new JoinDefinition("TaiKhoan", "tk1", "tk1.NguoiDungId = nd1.Id", JoinType.Left, Mapping: new() { { "TaiKhoanIsActive", "tk1.IsActive" }, { "TaiKhoanIsDeleted", "tk1.IsDeleted" } }),
            new JoinDefinition("NguoiDung", "nd2", "nd2.Id = y.NguoiXuLyId", JoinType.Left),
            new JoinDefinition("TaiKhoan", "tk2", "tk2.NguoiDungId = nd2.Id", JoinType.Left, Mapping: new() { { "TaiKhoanIsActive", "tk2.IsActive" }, { "TaiKhoanIsDeleted", "tk2.IsDeleted" } })
        ], parameters);

        var tenNguoiGuiSql = columnMapping["TenNguoiGui"];
        var tenNguoiXuLySql = columnMapping["TenNguoiXuLy"];

        var sql = $"""
            SELECT
                y.Id,
                y.CanHoId,
                y.YeuCauPhuongTienId,
                y.LoaiHanhDongYeuCauId,
                y.TrangThaiId,
                y.LyDo,
                y.NoiDung,
                y.CreatedAt,
                y.NgayXuLy,
                y.NguoiXuLyId,
                y.CreatedBy,
                y.YeuCauTenPhuongTien,
                y.YeuCauLoaiPhuongTienId,
                y.YeuCauBienSo,
                y.YeuCauMauXe,
                ch.TenCanHo,
                tg.TenTang,
                tn.TenToaNha,
                {tenNguoiGuiSql} AS TenNguoiGui,
                {tenNguoiXuLySql} AS TenNguoiXuLy
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere}
            """;

        var transaction = _dbContext.GetDbTransaction();
        var row = await connection.QueryFirstOrDefaultAsync<YeuCauPhuongTienReadModel>(sql, parameters, transaction: transaction);
        if (row == null) return null;

        var loaiYeuCauMap = LoaiHanhDongYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();

        return new DSYeuCauPhuongTienResponse
        {
            Id = row.Id,
            CanHoId = row.CanHoId,
            TenCanHo = row.TenCanHo,
            TenTang = row.TenTang,
            TenToaNha = row.TenToaNha,
            LoaiYeuCauId = row.LoaiHanhDongYeuCauId,
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(row.LoaiHanhDongYeuCauId, string.Empty),
            TrangThaiId = row.TrangThaiId,
            TenTrangThai = trangThaiMap.GetValueOrDefault(row.TrangThaiId, string.Empty),
            LyDo = row.LyDo,
            NoiDung = row.NoiDung,
            CreatedAt = row.CreatedAt,
            NgayXuLy = row.NgayXuLy,
            NguoiXuLyId = row.NguoiXuLyId,
            CreatedBy = row.CreatedBy,
            TenNguoiGui = row.TenNguoiGui,
            TenNguoiXuLy = row.TenNguoiXuLy,
            YeuCauTenPhuongTien = row.YeuCauTenPhuongTien,
            YeuCauBienSo = row.YeuCauBienSo
        };
    }
}
