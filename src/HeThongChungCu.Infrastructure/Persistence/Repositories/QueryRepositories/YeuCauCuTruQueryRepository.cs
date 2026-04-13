using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSYeuCauCuTru;
using HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class YeuCauCuTruQueryRepository : IYeuCauCuTruQueryRepository
{
    private readonly AppDbContext _dbContext;

    public YeuCauCuTruQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<DSYeuCauCuTruResponse>> GetPagedListAsync(
        LayDSYeuCauCuTruQuerySpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "CanHoId", "y.CanHoId" },
            { "LoaiYeuCauId", "y.LoaiYeuCauId" },
            { "TrangThaiId", "y.TrangThaiId" },
            { "CreatedAt", "y.CreatedAt" },
            { "ToaNhaId", "tg.ToaNhaId" },
            { "TangId", "ch.TangId" },
            { "TenNguoiGui", "COALESCE(NULLIF(LTRIM(RTRIM(nd1.Ho + ' ' + nd1.Ten)), ''), tk1.TenDangNhap, 'User #' + CAST(y.CreatedBy AS NVARCHAR(10)))" },
            { "TenNguoiXuLy", "COALESCE(NULLIF(LTRIM(RTRIM(nd2.Ho + ' ' + nd2.Ten)), ''), tk2.TenDangNhap, 'User #' + CAST(y.NguoiXuLyId AS NVARCHAR(10)))" },
            { "IsDeleted", "y.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(
            spec,
            columnMapping,
            parameters,
            discriminators: [("y.LoaiYeuCauCuDan", "YeuCauCuTru")],
            addSoftDeleteFilter: true);

        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("CanHo", "ch", "y.CanHoId = ch.Id", JoinType.Inner, true),
            new JoinDefinition("Tang", "tg", "ch.TangId = tg.Id", JoinType.Inner, true),
            new JoinDefinition("ToaNha", "tn", "tg.ToaNhaId = tn.Id", JoinType.Inner, true),
            new JoinDefinition("NguoiDung", "nd1", "y.CreatedBy = nd1.Id", JoinType.Left, true),
            new JoinDefinition("TaiKhoan", "tk1", "nd1.Id = tk1.NguoiDungId AND tk1.IsActive = 1", JoinType.Left, false),
            new JoinDefinition("NguoiDung", "nd2", "y.NguoiXuLyId = nd2.Id", JoinType.Left, true),
            new JoinDefinition("TaiKhoan", "tk2", "nd2.Id = tk2.NguoiDungId AND tk2.IsActive = 1", JoinType.Left, false)
        ]);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "CreatedAt");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                y.Id,
                y.CanHoId,
                y.LoaiYeuCauId,
                y.TrangThaiId,
                y.LyDo,
                y.NoiDung,
                y.CreatedAt,
                y.NgayXuLy,
                y.NguoiXuLyId,
                y.CreatedBy,
                y.YeuCauTen,
                y.YeuCauHo,
                y.YeuCauNgaySinh,
                y.YeuCauGioiTinhId,
                y.YeuCauSoDienThoai,
                y.YeuCauCCCD,
                y.YeuCauDiaChi,
                y.YeuCauLoaiQuanHeId,
                y.YeuCauQuanHeCuTruId,
                ch.TenCanHo,
                tg.TenTang,
                tg.ToaNhaId,
                ch.TangId,
                tn.TenToaNha,
                {columnMapping["TenNguoiGui"]} AS TenNguoiGui,
                {columnMapping["TenNguoiXuLy"]} AS TenNguoiXuLy
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        // Sử dụng helper GetDbTransaction() có sẵn trong AppDbContext
        var transaction = _dbContext.GetDbTransaction();

        var rows = (await connection.QueryAsync<YeuCauCuTruReadModel>(sql, parameters, transaction: transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiYeuCauMap = LoaiYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();

        var items = rows.Select(r => new DSYeuCauCuTruResponse
        {
            Id = r.Id,
            CanHoId = r.CanHoId,
            TenCanHo = r.TenCanHo,
            TenTang = r.TenTang,
            TenToaNha = r.TenToaNha,
            LoaiYeuCauId = r.LoaiYeuCauId,
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(r.LoaiYeuCauId, string.Empty),
            TrangThaiId = r.TrangThaiId,
            TenTrangThai = trangThaiMap.GetValueOrDefault(r.TrangThaiId, string.Empty),
            LyDo = r.LyDo,
            NoiDung = r.NoiDung,
            CreatedAt = r.CreatedAt,
            NgayXuLy = r.NgayXuLy,
            NguoiXuLyId = r.NguoiXuLyId,
            CreatedBy = r.CreatedBy,
            TenNguoiGui = r.TenNguoiGui,
            TenNguoiXuLy = r.TenNguoiXuLy
        }).ToList();

        return new PagedResult<DSYeuCauCuTruResponse>
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

    public async Task<YeuCauCuTruResponse?> GetByIdAsync(GetYeuCauCuTruByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "TenNguoiGui", "COALESCE(NULLIF(LTRIM(RTRIM(nd1.Ho + ' ' + nd1.Ten)), ''), tk1.TenDangNhap, 'User #' + CAST(y.CreatedBy AS NVARCHAR(10)))" },
            { "TenNguoiXuLy", "COALESCE(NULLIF(LTRIM(RTRIM(nd2.Ho + ' ' + nd2.Ten)), ''), tk2.TenDangNhap, 'User #' + CAST(y.NguoiXuLyId AS NVARCHAR(10)))" },
            { "IsDeleted", "y.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(
            spec,
            columnMapping,
            parameters,
            discriminators: [("y.LoaiYeuCauCuDan", "YeuCauCuTru")],
            addSoftDeleteFilter: true);

        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("CanHo", "ch", "y.CanHoId = ch.Id", JoinType.Inner, true),
            new JoinDefinition("Tang", "tg", "ch.TangId = tg.Id", JoinType.Inner, true),
            new JoinDefinition("ToaNha", "tn", "tg.ToaNhaId = tn.Id", JoinType.Inner, true),
            new JoinDefinition("NguoiDung", "nd1", "y.CreatedBy = nd1.Id", JoinType.Left, true),
            new JoinDefinition("TaiKhoan", "tk1", "nd1.Id = tk1.NguoiDungId AND tk1.IsActive = 1", JoinType.Left, false),
            new JoinDefinition("NguoiDung", "nd2", "y.NguoiXuLyId = nd2.Id", JoinType.Left, true),
            new JoinDefinition("TaiKhoan", "tk2", "nd2.Id = tk2.NguoiDungId AND tk2.IsActive = 1", JoinType.Left, false)
        ]);

        var sqlJoinsTl = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("TaiLieu", "tl", "tl.YeuCauCuTruId = y.Id", JoinType.Inner, true, Discriminators: [("LoaiTaiLieu", "YeuCauTaiLieuCuTru")])
        ]);

        var sqlJoinsTtl = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("TaiLieu", "tl", "tl.YeuCauCuTruId = y.Id", JoinType.Inner, true, Discriminators: [("LoaiTaiLieu", "YeuCauTaiLieuCuTru")]),
            new JoinDefinition("TepTaiLieu", "ttl", "ttl.TaiLieuId = tl.Id", JoinType.Inner, true, Discriminators: [("LoaiTepTaiLieu", "TepYeuCauTaiLieuCuTru")])
        ]);

        var sql = $"""
            -- 1. Main Info
            SELECT
                y.Id, y.CanHoId, y.LoaiYeuCauId, y.TrangThaiId, y.LyDo, y.NoiDung, 
                y.CreatedAt, y.NgayXuLy, y.NguoiXuLyId, y.CreatedBy,
                y.YeuCauTen, y.YeuCauHo, y.YeuCauNgaySinh, y.YeuCauGioiTinhId,
                y.YeuCauSoDienThoai, y.YeuCauCCCD, y.YeuCauDiaChi,
                y.YeuCauLoaiQuanHeId, y.YeuCauQuanHeCuTruId AS TargetQuanHeCuTruId,
                ch.TenCanHo, tg.TenTang, tn.TenToaNha,
                {columnMapping["TenNguoiGui"]} AS TenNguoiGui,
                {columnMapping["TenNguoiXuLy"]} AS TenNguoiXuLy
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere};

            -- 2. Documents (1-N)
            SELECT 
                tl.Id, tl.LoaiGiayToId, tl.SoGiayTo, tl.NgayPhatHanh, 
                tl.TaiLieuCuTruId AS TargetTaiLieuCuTruId
            FROM YeuCau y
            {sqlJoinsTl}
            {sqlWhere};

            -- 3. Files (1-N)
            SELECT 
                ttl.Id, ttl.FileUrl, ttl.FileName, ttl.ContentType,
                ttl.TaiLieuId AS DocumentId
            FROM YeuCau y
            {sqlJoinsTtl}
            {sqlWhere};
            """;

        var transaction = _dbContext.GetDbTransaction();

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: transaction);

        var readModel = await multi.ReadFirstOrDefaultAsync<YeuCauCuTruReadModel>();
        if (readModel == null) return null;

        var loaiYeuCauMap = LoaiYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();
        var loaiGiayToMap = LoaiGiayTo.ToDictionary();
        var gioiTinhMap = GioiTinh.ToDictionary();
        var loaiQuanHeCuTruMap = LoaiQuanHeCuTru.ToDictionary();

        // Map to Response
        var response = new YeuCauCuTruResponse
        {
            Id = readModel.Id,
            CanHoId = readModel.CanHoId,
            LoaiYeuCauId = readModel.LoaiYeuCauId,
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(readModel.LoaiYeuCauId, string.Empty),
            TrangThaiId = readModel.TrangThaiId,
            TenTrangThai = trangThaiMap.GetValueOrDefault(readModel.TrangThaiId, string.Empty),
            LyDo = readModel.LyDo,
            NoiDung = readModel.NoiDung,
            CreatedAt = readModel.CreatedAt,
            NgayXuLy = readModel.NgayXuLy,
            NguoiXuLyId = readModel.NguoiXuLyId,
            CreatedBy = readModel.CreatedBy,
            TenNguoiGui = readModel.TenNguoiGui,
            TenNguoiXuLy = readModel.TenNguoiXuLy,
            TenCanHo = readModel.TenCanHo,
            TenTang = readModel.TenTang,
            TenToaNha = readModel.TenToaNha,
            YeuCauTen = readModel.YeuCauTen,
            YeuCauHo = readModel.YeuCauHo,
            YeuCauNgaySinh = readModel.YeuCauNgaySinh,
            YeuCauGioiTinhId = readModel.YeuCauGioiTinhId,
            YeuCauGioiTinhTen = readModel.YeuCauGioiTinhId.HasValue ? gioiTinhMap.GetValueOrDefault(readModel.YeuCauGioiTinhId.Value, string.Empty) : null,
            YeuCauSoDienThoai = readModel.YeuCauSoDienThoai,
            YeuCauCCCD = readModel.YeuCauCCCD,
            YeuCauDiaChi = readModel.YeuCauDiaChi,
            YeuCauLoaiQuanHeId = readModel.YeuCauLoaiQuanHeId,
            YeuCauLoaiQuanHeTen = readModel.YeuCauLoaiQuanHeId.HasValue ? loaiQuanHeCuTruMap.GetValueOrDefault(readModel.YeuCauLoaiQuanHeId.Value, string.Empty) : null,
            TargetQuanHeCuTruId = readModel.YeuCauQuanHeCuTruId
        };

        var documents = (await multi.ReadAsync<YeuCauCuTruDocumentReadModel>()).ToList();
        var fileRows = (await multi.ReadAsync<YeuCauCuTruFileReadModel>()).ToList();

        // 2. Map Documents to Response
        var docResponses = documents.Select(doc => new TaiLieuResponse
        {
            Id = doc.Id,
            LoaiGiayToId = doc.LoaiGiayToId,
            TenLoaiGiayTo = loaiGiayToMap.GetValueOrDefault(doc.LoaiGiayToId, string.Empty),
            SoGiayTo = doc.SoGiayTo,
            NgayPhatHanh = doc.NgayPhatHanh,
            Files = fileRows
                .Where(f => f.DocumentId == doc.Id)
                .Select(f => new TepTaiLieuResponse(f.Id, f.FileUrl, f.FileName, f.ContentType))
                .ToList()
        }).ToList();

        return response with { Documents = docResponses };
    }

    public async Task<DSYeuCauCuTruResponse?> GetListResponseByIdAsync(GetYeuCauCuTruByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "y.Id" },
            { "TenNguoiGui", "COALESCE(NULLIF(LTRIM(RTRIM(nd1.Ho + ' ' + nd1.Ten)), ''), tk1.TenDangNhap, 'User #' + CAST(y.CreatedBy AS NVARCHAR(10)))" },
            { "TenNguoiXuLy", "COALESCE(NULLIF(LTRIM(RTRIM(nd2.Ho + ' ' + nd2.Ten)), ''), tk2.TenDangNhap, 'User #' + CAST(y.NguoiXuLyId AS NVARCHAR(10)))" },
            { "IsDeleted", "y.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(
            spec,
            columnMapping,
            parameters,
            discriminators: [("y.LoaiYeuCauCuDan", "YeuCauCuTru")],
            addSoftDeleteFilter: true);

        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("CanHo", "ch", "y.CanHoId = ch.Id", JoinType.Inner, true),
            new JoinDefinition("Tang", "tg", "ch.TangId = tg.Id", JoinType.Inner, true),
            new JoinDefinition("ToaNha", "tn", "tg.ToaNhaId = tn.Id", JoinType.Inner, true),
            new JoinDefinition("NguoiDung", "nd1", "y.CreatedBy = nd1.Id", JoinType.Left, true),
            new JoinDefinition("TaiKhoan", "tk1", "nd1.Id = tk1.NguoiDungId AND tk1.IsActive = 1", JoinType.Left, false),
            new JoinDefinition("NguoiDung", "nd2", "y.NguoiXuLyId = nd2.Id", JoinType.Left, true),
            new JoinDefinition("TaiKhoan", "tk2", "nd2.Id = tk2.NguoiDungId AND tk2.IsActive = 1", JoinType.Left, false)
        ]);

        var sql = $"""
            SELECT
                y.Id,
                y.CanHoId,
                y.LoaiYeuCauId,
                y.TrangThaiId,
                y.LyDo,
                y.NoiDung,
                y.CreatedAt,
                y.NgayXuLy,
                y.NguoiXuLyId,
                y.CreatedBy,
                ch.TenCanHo,
                tg.TenTang,
                tg.ToaNhaId,
                ch.TangId,
                tn.TenToaNha,
                {columnMapping["TenNguoiGui"]} AS TenNguoiGui,
                {columnMapping["TenNguoiXuLy"]} AS TenNguoiXuLy
            FROM YeuCau y
            {sqlJoins}
            {sqlWhere}
            """;

        var transaction = _dbContext.GetDbTransaction();

        var row = await connection.QueryFirstOrDefaultAsync<YeuCauCuTruReadModel>(sql, parameters, transaction: transaction);
        if (row == null) return null;

        var loaiYeuCauMap = LoaiYeuCau.ToDictionary();
        var trangThaiMap = TrangThaiYeuCau.ToDictionary();

        return new DSYeuCauCuTruResponse
        {
            Id = row.Id,
            CanHoId = row.CanHoId,
            TenCanHo = row.TenCanHo,
            TenTang = row.TenTang,
            TenToaNha = row.TenToaNha,
            LoaiYeuCauId = row.LoaiYeuCauId,
            TenLoaiYeuCau = loaiYeuCauMap.GetValueOrDefault(row.LoaiYeuCauId, string.Empty),
            TrangThaiId = row.TrangThaiId,
            TenTrangThai = trangThaiMap.GetValueOrDefault(row.TrangThaiId, string.Empty),
            LyDo = row.LyDo,
            NoiDung = row.NoiDung,
            CreatedAt = row.CreatedAt,
            NgayXuLy = row.NgayXuLy,
            NguoiXuLyId = row.NguoiXuLyId,
            CreatedBy = row.CreatedBy,
            TenNguoiGui = row.TenNguoiGui,
            TenNguoiXuLy = row.TenNguoiXuLy
        };
    }
}
