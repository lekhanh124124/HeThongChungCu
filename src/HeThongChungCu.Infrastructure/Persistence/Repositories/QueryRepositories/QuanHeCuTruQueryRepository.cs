using HeThongChungCu.Application.Features.CuDan.DTOs;
using HeThongChungCu.Application.Features.CuDan.Queries.LayDSCuTruCuaNguoiDung;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru;
using HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Features.QLCuTru.Queries.LayDSCuDanTrongChungCu;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class QuanHeCuTruQueryRepository : IQuanHeCuTruQueryRepository
{
    private readonly AppDbContext _dbContext;
    public QuanHeCuTruQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CuDanResponse>> LayDSCuDanTrongChungCu(
        LayDSCuDanTrongChungCuQuerySpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ToaNhaId", "tn.Id" },
            { "MaToaNha", "tn.MaToaNha" },
            { "TangId", "t.Id" },
            { "MaTang", "t.MaTang" },
            { "CanHoId", "q.CanHoId" },
            { "MaCanHo", "c.MaCanHo" },

            { "NguoiDungId", "q.NguoiDungId" },
            { "TrangThaiCuTruId", "q.TrangThaiCuTruId" },
            { "NgayBatDau", "q.NgayBatDau" },
            { "NgayKetThuc", "q.NgayKetThuc" },
            { "LoaiQuanHeCuTruId", "q.LoaiQuanHeCuTruId" },
            { "HoTen", "u.Ho + N' ' + u.Ten" },
            { "SoDienThoai", "u.SoDienThoai" },
            { "IsDeleted", "q.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("NguoiDung", "u", "u.Id = q.NguoiDungId", JoinType.Left, Mapping: new() { { "NguoiDungIsDeleted", "u.IsDeleted" } }),
            new JoinDefinition("CanHo", "c", "c.Id = q.CanHoId", JoinType.Left, Mapping: new() { { "CanHoIsDeleted", "c.IsDeleted" } }),
            new JoinDefinition("Tang", "t", "t.Id = c.TangId", JoinType.Left, Mapping: new() { { "TangIsDeleted", "t.IsDeleted" } }),
            new JoinDefinition("ToaNha", "tn", "tn.Id = t.ToaNhaId", JoinType.Left, Mapping: new() { { "ToaNhaIsDeleted", "tn.IsDeleted" } })
        ], parameters);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "NgayBatDau");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                tn.MaToaNha       AS MaToaNha,
                t.MaTang          AS MaTang,
                c.MaCanHo         AS MaCanHo,
                q.Id         AS QuanHeCuTruId,
                q.NguoiDungId,
                u.Ho + N' ' + u.Ten AS HoTen,
                u.SoDienThoai as PhoneNumber,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                q.NgayKetThuc,
                q.TrangThaiCuTruId
            FROM QuanHeCuTru q
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<CuDanReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var trangThaiMap = TrangThaiCuTru.ToDictionary();
        var items = rows.Select(r => new CuDanResponse
        {
            MaToaNha = r.MaToaNha,
            MaTang = r.MaTang,
            MaCanHo = r.MaCanHo,
            QuanHeCuTruId = r.QuanHeCuTruId,
            UserId = r.NguoiDungId,
            HoTen = r.HoTen,
            PhoneNumber = r.PhoneNumber,
            LoaiQuanHeCuTruId = r.LoaiQuanHeCuTruId,
            TenLoaiQuanHeCuTru = loaiQuanHeMap.GetValueOrDefault(r.LoaiQuanHeCuTruId, string.Empty),
            NgayBatDau = r.NgayBatDau,
            NgayKetThuc = r.NgayKetThuc,
            TrangThaiCuTruId = r.TrangThaiCuTruId,
            TenTrangThaiCuTru = trangThaiMap.GetValueOrDefault(r.TrangThaiCuTruId, string.Empty)
        }).ToList();

        return new PagedResult<CuDanResponse>
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


    public async Task<IReadOnlyList<QuanHeCuTruResponse>> LayDSCuTruByUserId(
        LayDSCuTruCuaNguoiDungSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "NguoiDungId", "q.NguoiDungId" },
            { "TrangThaiCuTruId", "q.TrangThaiCuTruId" },
            { "IsDeleted", "q.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("CanHo", "c", "c.Id = q.CanHoId", JoinType.Left, Mapping: new() { { "CanHoIsDeleted", "c.IsDeleted" } }),
            new JoinDefinition("Tang", "t", "t.Id = c.TangId", JoinType.Left, Mapping: new() { { "TangIsDeleted", "t.IsDeleted" } }),
            new JoinDefinition("ToaNha", "tn", "tn.Id = t.ToaNhaId", JoinType.Left, Mapping: new() { { "ToaNhaIsDeleted", "tn.IsDeleted" } })
        ], parameters);

        var sql = $"""
            SELECT
                q.Id,
                tn.Id AS ToaNhaId,
                tn.MaToaNha,
                tn.TenToaNha,
                t.Id AS TangId,
                t.MaTang,
                t.TenTang,
                c.Id AS CanHoId,
                c.MaCanHo,
                c.TenCanHo,
                q.LoaiQuanHeCuTruId,
                (SELECT COUNT(*) FROM QuanHeCuTru qr 
                    WHERE qr.CanHoId = q.CanHoId 
                    AND qr.TrangThaiCuTruId = 1
                    AND qr.IsDeleted = 0) AS TongCuDan
            FROM QuanHeCuTru q
            {sqlJoins}
            {sqlWhere}
            """;

        var rows = await connection.QueryAsync<CuTruReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction());

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var items = rows.Select(r => new QuanHeCuTruResponse
        {
            QuanHeCuTruId = r.Id,
            ToaNhaId = r.ToaNhaId,
            MaToaNha = r.MaToaNha,
            TenToaNha = r.TenToaNha,
            TangId = r.TangId,
            MaTang = r.MaTang,
            TenTang = r.TenTang,
            CanHoId = r.CanHoId,
            MaCanHo = r.MaCanHo,
            TenCanHo = r.TenCanHo,
            LoaiQuanHeCuTruId = r.LoaiQuanHeCuTruId,
            TongCuDan = r.TongCuDan,
            LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault(r.LoaiQuanHeCuTruId, string.Empty)
        }).ToList();

        return items;
    }

    public async Task<LayThongTinCuDanResponse?> GetByIdAsync(
        LayThongTinCuDanSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "q.Id" },
            { "NguoiDungId", "q.NguoiDungId" },
            { "IsDeleted", "q.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sqlJoinsRoot = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("NguoiDung", "u", "u.Id = q.NguoiDungId", JoinType.Left, Mapping: new() { { "NguoiDungIsDeleted", "u.IsDeleted" } }),
            new JoinDefinition("TaiKhoan", "a", "a.NguoiDungId = u.Id", Mapping: new() { { "TaiKhoanIsActive", "a.IsActive" }, { "TaiKhoanIsDeleted", "a.IsDeleted" } }),
            new JoinDefinition("TepTaiLieu", "atl", "atl.Id = a.AnhDaiDienId", JoinType.Left, Mapping: new() { { "TepIsDeleted", "atl.IsDeleted" }, { "LoaiTepNguoiDung", "atl.LoaiTepTaiLieu" } })
        ], parameters);

        var sqlJoinsDoc = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("TaiLieu", "t", "t.NguoiDungId = q.NguoiDungId", JoinType.Left, Mapping: new() { { "TaiLieuIsDeleted", "t.IsDeleted" }, { "LoaiTaiLieuNguoiDung", "t.LoaiTaiLieu" } }),
            new JoinDefinition("TepTaiLieu", "f", "f.TaiLieuId = t.Id", JoinType.Left, Mapping: new() { { "TepIsDeleted", "f.IsDeleted" }, { "LoaiTepNguoiDung", "f.LoaiTepTaiLieu" } })
        ], parameters);

        var sql = $"""
            -- Query 1: QuanHeCuTru + NguoiDung + Icon (N-1)
            SELECT
                q.NguoiDungId,
                u.Ho + N' ' + u.Ten AS FullName,
                u.Ho as LastName,
                u.Ten as FirstName,
                u.SoDienThoai as PhoneNumber,
                u.NgaySinh as Dob,
                u.GioiTinhId,
                u.CCCD as IdCard,
                atl.FileUrl as AnhDaiDienUrl,
                q.Id             AS QuanHeCuTruId,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                q.NgayKetThuc,
                q.TrangThaiCuTruId,
                u.DiaChi
            FROM QuanHeCuTru q
            {sqlJoinsRoot}
            {sqlWhere};

            -- Query 2: TaiLieu + TepTaiLieu (1-N)
            SELECT t.Id AS DocId, t.LoaiGiayToId, t.SoGiayTo, t.NgayPhatHanh,
                   f.Id AS FileId, f.FileUrl, f.FileName, f.ContentType
            FROM QuanHeCuTru q
            {sqlJoinsDoc}
            {sqlWhere};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());
        var firstRow = await multi.ReadFirstOrDefaultAsync<CuDanDetailReadModel>();

        if (firstRow == null) return null;

        var docRows = (await multi.ReadAsync<TaiLieuReadModel>()).ToList();
        var docLookup = new Dictionary<int, TaiLieuResponse>();

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var gioiTinhMap = GioiTinh.ToDictionary();
        var trangThaiCuTruMap = TrangThaiCuTru.ToDictionary();

        var result = new LayThongTinCuDanResponse
        {
            UserId = firstRow.NguoiDungId,
            FullName = firstRow.FullName,
            FirstName = firstRow.FirstName,
            LastName = firstRow.LastName,
            PhoneNumber = firstRow.PhoneNumber,
            Dob = firstRow.Dob.GetValueOrDefault(),
            GioiTinhId = firstRow.GioiTinhId,
            GioiTinhName = gioiTinhMap.GetValueOrDefault(firstRow.GioiTinhId, string.Empty),
            AnhDaiDienUrl = firstRow.AnhDaiDienUrl ?? string.Empty,
            QuanHeCuTruId = firstRow.QuanHeCuTruId,
            LoaiQuanHeCuTruId = firstRow.LoaiQuanHeCuTruId,
            LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault(firstRow.LoaiQuanHeCuTruId, string.Empty),
            NgayBatDau = firstRow.NgayBatDau,
            NgayKetThuc = firstRow.NgayKetThuc,
            TrangThaiCuTruId = firstRow.TrangThaiCuTruId,
            TrangThaiCuTruTen = trangThaiCuTruMap.GetValueOrDefault(firstRow.TrangThaiCuTruId, string.Empty),
            DiaChi = firstRow.DiaChi,
            IdCard = firstRow.IdCard,
            TaiLieuCuTrus = []
        };

        foreach (var row in docRows)
        {
            if (!docLookup.TryGetValue(row.DocId, out var doc))
            {
                doc = new TaiLieuResponse
                {
                    Id = row.DocId,
                    LoaiGiayToId = row.LoaiGiayToId,
                    TenLoaiGiayTo = LoaiGiayTo.FromValue(row.LoaiGiayToId)?.Name ?? string.Empty,
                    SoGiayTo = row.SoGiayTo,
                    NgayPhatHanh = row.NgayPhatHanh,
                    Files = []
                };
                docLookup.Add(doc.Id, doc);
                result.TaiLieuCuTrus.Add(doc);
            }

            if (row.FileId != 0)
            {
                if (!doc.Files.Any(f => f.Id == row.FileId))
                {
                    doc.Files.Add(new TepTaiLieuResponse(
                        row.FileId,
                        row.FileUrl,
                        row.FileName,
                        row.ContentType));
                }
            }
        }

        return result;
    }

    public async Task<IReadOnlyList<ThanhVienCuTruResponse>> LayThanhVienCuTru(
        LayThanhVienCuTruSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "CanHoId", "q.CanHoId" },
            { "TrangThaiCuTruId", "q.TrangThaiCuTruId" },
            { "IsDeleted", "q.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("NguoiDung", "u", "u.Id = q.NguoiDungId", JoinType.Left, Mapping: new() { { "NguoiDungIsDeleted", "u.IsDeleted" } }),
            new JoinDefinition("TaiKhoan", "a", "a.NguoiDungId = u.Id", Mapping: new() { { "TaiKhoanIsActive", "a.IsActive" }, { "TaiKhoanIsDeleted", "a.IsDeleted" } }),
            new JoinDefinition("TepTaiLieu", "atl", "atl.Id = a.AnhDaiDienId", JoinType.Left, Mapping: new() { { "TepIsDeleted", "atl.IsDeleted" }, { "LoaiTepNguoiDung", "atl.LoaiTepTaiLieu" } })
        ], parameters);

        var sql = $"""
            SELECT
                q.Id,
                u.Id as UserId,
                q.LoaiQuanHeCuTruId,
                q.NgayBatDau,
                u.Ten as FirstName,
                u.Ho as LastName,
                atl.FileUrl as AnhDaiDienUrl
            FROM QuanHeCuTru q
            {sqlJoins}
            {sqlWhere}
            """;

        var rows = await connection.QueryAsync<ThanhVienCuTruReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction());

        var loaiQuanHeMap = LoaiQuanHeCuTru.ToDictionary();
        var items = rows.Select(r => new ThanhVienCuTruResponse
        {
            QuanHeCuTruId = r.Id,
            UserId = r.UserId,
            LoaiQuanHeCuTruId = r.LoaiQuanHeCuTruId,
            LoaiQuanHeTen = loaiQuanHeMap.GetValueOrDefault(r.LoaiQuanHeCuTruId, string.Empty),
            NgayBatDau = r.NgayBatDau,
            FullName = $"{r.LastName} {r.FirstName}",
            AnhDaiDienUrl = r.AnhDaiDienUrl ?? string.Empty
        }).ToList();

        return items;
    }
}
