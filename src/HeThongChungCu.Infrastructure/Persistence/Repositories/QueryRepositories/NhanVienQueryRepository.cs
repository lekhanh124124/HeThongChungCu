using HeThongChungCu.Application.Features.QLNhanVien.DTOs;
using HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienList;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class NhanVienQueryRepository : INhanVienQueryRepository
{
    private readonly AppDbContext _dbContext;

    public NhanVienQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NhanVienDetailResponse?> GetByIdAsync(GetNhanVienByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "nv.Id" },
            { "IsDeleted", "nv.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var nguoiDungMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "NguoiDungIsDeleted", "u.IsDeleted" }
        };

        var taiKhoanMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TaiKhoanIsActive", "a.IsActive" },
            { "TaiKhoanIsDeleted", "a.IsDeleted" }
        };

        var tepTaiLieuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TepIsDeleted", "atl.IsDeleted" },
            { "LoaiTepTaiLieu", "atl.LoaiTepId" }
        };

        var taiLieuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TaiLieuIsDeleted", "t.IsDeleted" },
            { "LoaiTaiLieu", "t.LoaiTaiLieuId" }
        };

        var tepTaiLieuNguoiDungMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TepIsDeleted", "f.IsDeleted" },
            { "LoaiTepTaiLieuNguoiDung", "f.LoaiTepId" }
        };

        var sqlMainJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("NguoiDung", "u", "u.Id = nv.NguoiDungId", Mapping: nguoiDungMapping),
            new JoinDefinition("TaiKhoan", "a", "a.NguoiDungId = u.Id", Mapping: taiKhoanMapping),
            new JoinDefinition("TepTaiLieu", "atl", "atl.Id = a.AnhDaiDienId", Mapping: tepTaiLieuMapping)
        ], parameters);

        var sqlPqJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("NguoiDung", "u", "u.Id = nv.NguoiDungId", Mapping: nguoiDungMapping),
            new JoinDefinition("TaiKhoan", "a", "a.NguoiDungId = u.Id", Mapping: taiKhoanMapping),
            new JoinDefinition("PhanQuyen", "pq", "pq.TaiKhoanId = a.Id", JoinType.Left)
        ], parameters);

        var sqlDocJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("TaiLieu", "t", "t.NguoiDungId = nv.NguoiDungId", Mapping: taiLieuMapping),
            new JoinDefinition("TepTaiLieu", "f", "f.TaiLieuId = t.Id", Mapping: tepTaiLieuNguoiDungMapping)
        ], parameters);

        var sql = $"""
            SELECT 
                nv.Id,
                nv.NguoiDungId,
                u.Ten AS FirstName,
                u.Ho AS LastName,
                u.Ho + ' ' + u.Ten AS HoTen,
                a.Email,
                u.SoDienThoai,
                u.CCCD,
                u.DiaChi,
                u.NgaySinh AS Dob,
                u.GioiTinhId,
                atl.FileUrl AS AnhDaiDienUrl,
                nv.LoaiNhanVienId,
                nv.TrangThaiNhanVienId,
                nv.MaNhanVien,
                nv.NgayVaoLam,
                nv.NgayNghiLam,
                nv.GhiChu
            FROM NhanVien nv
            {sqlMainJoins}
            {sqlWhere};

            SELECT pq.RoleId
            FROM NhanVien nv
            {sqlPqJoins}
            {sqlWhere};

            SELECT t.Id AS DocId, t.LoaiGiayToId, t.SoGiayTo, t.NgayPhatHanh,
                   f.Id AS FileId, f.FileUrl, f.FileName, f.ContentType
            FROM NhanVien nv
            {sqlDocJoins}
            {sqlWhere};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());
        var first = await multi.ReadFirstOrDefaultAsync<NhanVienDetailReadModel>();
        if (first == null) return null;

        var roleIds = (await multi.ReadAsync<int>()).ToList();
        var docRows = (await multi.ReadAsync<TaiLieuReadModel>()).ToList();

        var gioiTinhMap = GioiTinh.ToDictionary();
        var roleMap = Role.ToDictionary();
        var loaiNhanVienMap = LoaiNhanVien.ToDictionary();
        var trangThaiNhanVienMap = TrangThaiNhanVien.ToDictionary();
        var loaiGiayToMap = LoaiGiayTo.ToDictionary();

        var response = new NhanVienDetailResponse
        {
            Id = first.Id,
            NguoiDungId = first.NguoiDungId,
            FirstName = first.FirstName,
            LastName = first.LastName,
            HoTen = first.HoTen,
            Email = first.Email ?? string.Empty,
            SoDienThoai = first.SoDienThoai ?? string.Empty,
            CCCD = first.CCCD ?? string.Empty,
            DiaChi = first.DiaChi ?? string.Empty,
            Dob = first.Dob.GetValueOrDefault(),
            GioiTinhId = first.GioiTinhId ?? 0,
            GioiTinhName = first.GioiTinhId != null ? gioiTinhMap.GetValueOrDefault(first.GioiTinhId.Value, string.Empty) : string.Empty,
            AnhDaiDienUrl = first.AnhDaiDienUrl ?? string.Empty,
            LoaiNhanVienId = first.LoaiNhanVienId,
            LoaiNhanVienTen = loaiNhanVienMap.GetValueOrDefault(first.LoaiNhanVienId, string.Empty),
            TrangThaiNhanVienId = first.TrangThaiNhanVienId,
            TrangThaiNhanVienTen = trangThaiNhanVienMap.GetValueOrDefault(first.TrangThaiNhanVienId, string.Empty),
            MaNhanVien = first.MaNhanVien,
            NgayVaoLam = first.NgayVaoLam,
            NgayNghiLam = first.NgayNghiLam,
            GhiChu = first.GhiChu,
            Roles = roleIds.Select(rId => roleMap.GetValueOrDefault(rId, string.Empty)).ToList(),
            TaiLieuNguoiDungs = []
        };

        var docLookup = new Dictionary<int, TaiLieuNhanVienResponse>();
        foreach (var row in docRows)
        {
            if (!docLookup.TryGetValue(row.DocId, out var doc))
            {
                doc = new TaiLieuNhanVienResponse
                {
                    Id = row.DocId,
                    LoaiGiayToId = row.LoaiGiayToId,
                    TenLoaiGiayTo = loaiGiayToMap.GetValueOrDefault(row.LoaiGiayToId, string.Empty),
                    SoGiayTo = row.SoGiayTo,
                    NgayPhatHanh = row.NgayPhatHanh,
                    Files = []
                };
                docLookup.Add(doc.Id, doc);
                response.TaiLieuNguoiDungs.Add(doc);
            }

            if (row.FileId != 0)
            {
                if (!doc.Files.Any(f => f.Id == row.FileId))
                {
                    doc.Files.Add(new TepTaiLieuNhanVienResponse(
                        row.FileId,
                        row.FileUrl,
                        row.FileName,
                        row.ContentType));
                }
            }
        }

        return response;
    }

    public async Task<PagedResult<NhanVienResponse>> GetListAsync(GetNhanVienListSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "nv.Id" },
            { "MaNhanVien", "nv.MaNhanVien" },
            { "HoTen", "u.Ho + ' ' + u.Ten" },
            { "Email", "a.Email" },
            { "SoDienThoai", "u.SoDienThoai" },
            { "LoaiNhanVienId", "nv.LoaiNhanVienId" },
            { "TrangThaiNhanVienId", "nv.TrangThaiNhanVienId" },
            { "NgayVaoLam", "nv.NgayVaoLam" },
            { "NgayNghiLam", "nv.NgayNghiLam" },
            { "IsDeleted", "nv.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var nguoiDungMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "NguoiDungIsDeleted", "u.IsDeleted" }
        };

        var taiKhoanMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TaiKhoanIsActive", "a.IsActive" },
            { "TaiKhoanIsDeleted", "a.IsDeleted" }
        };

        var tepTaiLieuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TepIsDeleted", "atl.IsDeleted" },
            { "LoaiTepTaiLieu", "atl.LoaiTepId" }
        };

        var sqlJoins = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("NguoiDung", "u", "u.Id = nv.NguoiDungId", JoinType.Left, Mapping: nguoiDungMapping),
            new JoinDefinition("TaiKhoan", "a", "a.NguoiDungId = u.Id", Mapping: taiKhoanMapping),
            new JoinDefinition("TepTaiLieu", "atl", "atl.Id = a.AnhDaiDienId", Mapping: tepTaiLieuMapping)
        ], parameters);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT 
                COUNT(*) OVER() AS TotalCount,
                nv.Id,
                nv.NguoiDungId,
                u.Ho + ' ' + u.Ten AS HoTen,
                a.Email,
                u.SoDienThoai,
                atl.FileUrl AS AnhDaiDienUrl,
                nv.LoaiNhanVienId,
                nv.TrangThaiNhanVienId,
                nv.MaNhanVien,
                nv.NgayVaoLam,
                nv.NgayNghiLam,
                nv.GhiChu
            FROM NhanVien nv
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var rows = (await connection.QueryAsync<NhanVienReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;
        var loaiNhanVienMap = LoaiNhanVien.ToDictionary();
        var trangThaiNhanVienMap = TrangThaiNhanVien.ToDictionary();

        var items = rows.Select(x => new NhanVienResponse
        {
            Id = x.Id,
            AnhDaiDienUrl = x.AnhDaiDienUrl ?? string.Empty,
            MaNhanVien = x.MaNhanVien,
            HoTen = x.HoTen,
            Email = x.Email ?? string.Empty,
            SoDienThoai = x.SoDienThoai,
            LoaiNhanVienId = x.LoaiNhanVienId,
            LoaiNhanVienTen = loaiNhanVienMap.GetValueOrDefault(x.LoaiNhanVienId, string.Empty),
            TrangThaiNhanVienId = x.TrangThaiNhanVienId,
            TrangThaiNhanVienTen = trangThaiNhanVienMap.GetValueOrDefault(x.TrangThaiNhanVienId, string.Empty),
            NgayVaoLam = x.NgayVaoLam,
            NgayNghiLam = x.NgayNghiLam
        }).ToList();

        return new PagedResult<NhanVienResponse>
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
}
