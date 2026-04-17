using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetDoiTacById;
using HeThongChungCu.Application.Features.QLDoiTac.Queries.GetListDoiTac;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class DoiTacQueryRepository : IDoiTacQueryRepository
{
    private readonly AppDbContext _dbContext;

    public DoiTacQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<DoiTacResponse>> GetAllAsync(
        GetListDoiTacSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TenDoiTac", "dt.TenDoiTac" },
            { "TenCongTy", "dt.TenCongTy" },
            { "Email", "dt.Email" },
            { "SoDienThoai", "dt.SoDienThoai" },
            { "Id", "dt.Id" },
            { "IsDeleted", "dt.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, 
                   dt.Id, dt.TenDoiTac, dt.TenCongTy, dt.NguoiDaiDien, dt.SoDienThoai, dt.Email
            FROM DoiTac dt
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var rows = (await connection.QueryAsync<DoiTacReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();

        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var items = rows.Select(r => new DoiTacResponse
        {
            Id = r.Id,
            TenDoiTac = r.TenDoiTac,
            TenCongTy = r.TenCongTy,
            NguoiDaiDien = r.NguoiDaiDien,
            SoDienThoai = r.SoDienThoai,
            Email = r.Email
        }).ToList();

        return new PagedResult<DoiTacResponse>
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

    public async Task<DoiTacDetailResponse?> GetByIdAsync(
        GetDoiTacByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var doiTacMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "dt.Id" },
            { "IsDeleted", "dt.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, doiTacMapping, parameters);

        // Mappings for related entities
        var hopDongMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "HopDongIsDeleted", "hd.IsDeleted" }
        };

        var tepTaiLieuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "TepIsDeleted", "tp.IsDeleted" },
            { "LoaiTepTaiLieu", "tp.LoaiTepId" }
        };

        var dichVuMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "DichVuIsDeleted", "dv.IsDeleted" }
        };

        var sqlJoinHd = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("HopDongDoiTac", "hd", "hd.DoiTacId = dt.Id", Mapping: hopDongMapping),
            new JoinDefinition("DichVu", "dv", "dv.Id = hd.DichVuId", Mapping: dichVuMapping)
        ], parameters);

        var sqlJoinTp = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("HopDongDoiTac", "hd", "hd.DoiTacId = dt.Id", Mapping: hopDongMapping),
            new JoinDefinition("TepTaiLieu", "tp", "tp.HopDongDoiTacId = hd.Id", JoinType.Left, Mapping: tepTaiLieuMapping)
        ], parameters);

        var sql = $"""
            -- Query 1: DoiTac
            SELECT dt.Id, dt.TenDoiTac, dt.TenCongTy, dt.NguoiDaiDien, dt.SoGiayPhepKD, dt.MaSoThue, dt.DiaChi, dt.SoDienThoai, dt.Email
            FROM DoiTac dt
            {sqlWhere};

            -- Query 2: HopDong + DichVu
            SELECT hd.Id AS HopDongUid, hd.SoHopDong, hd.NgayKy, hd.NgayHetHan, hd.GiaTriHopDong, hd.NoiDung, hd.DichVuId AS HopDongDichVuId, hd.TrangThaiHopDongId,
                   dv.Id AS DichVuUid, dv.MaDichVu, dv.TenDichVu, dv.LoaiDichVuId, dv.DonViTinh, dv.IsBatBuoc, dv.TrangThaiId AS DichVuTrangThaiId
            FROM DoiTac dt
            {sqlJoinHd}
            {sqlWhere}
            ORDER BY hd.NgayKy DESC;

            -- Query 3: TepTaiLieu
            SELECT tp.Id AS FileUid, tp.FileUrl, tp.FileName, tp.ContentType, tp.HopDongDoiTacId
            FROM DoiTac dt
            {sqlJoinTp}
            {sqlWhere};
            """;

        var transaction = _dbContext.GetDbTransaction();
        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: transaction);

        var firstRow = await multi.ReadFirstOrDefaultAsync<DoiTacDetailReadModel>();
        if (firstRow == null) return null;

        var doiTac = new DoiTacDetailResponse
        {
            Id = firstRow.Id,
            TenDoiTac = firstRow.TenDoiTac,
            TenCongTy = firstRow.TenCongTy,
            NguoiDaiDien = firstRow.NguoiDaiDien,
            SoGiayPhepKD = firstRow.SoGiayPhepKD,
            MaSoThue = firstRow.MaSoThue,
            DiaChi = firstRow.DiaChi,
            SoDienThoai = firstRow.SoDienThoai,
            Email = firstRow.Email,
            HopDongs = []
        };

        var loaiDichVuMap = LoaiDichVu.ToDictionary();
        var trangThaiHopDongMap = TrangThaiHopDong.ToDictionary();
        var trangThaiDichVuMap = TrangThaiDichVu.ToDictionary();

        var hopDongs = (await multi.ReadAsync<HopDongReadModel>()).ToList();
        var teps = (await multi.ReadAsync<DoiTacContractFileReadModel>()).ToList();

        foreach (var hd in hopDongs)
        {
            var existingHopDong = new HopDongResponse
            {
                Id = hd.HopDongUid,
                SoHopDong = hd.SoHopDong,
                NgayKy = hd.NgayKy,
                NgayHetHan = hd.NgayHetHan,
                GiaTriHopDong = hd.GiaTriHopDong,
                LoaiDichVuId = hd.LoaiDichVuId,
                LoaiDichVuTen = loaiDichVuMap.GetValueOrDefault(hd.LoaiDichVuId, string.Empty),
                TrangThaiHopDongId = hd.TrangThaiHopDongId,
                TrangThaiHopDongTen = trangThaiHopDongMap.GetValueOrDefault(hd.TrangThaiHopDongId, string.Empty),
                NoiDung = hd.NoiDung,
                Teps = [],

                // Dich Vu Info
                MaDichVu = hd.MaDichVu,
                TenDichVu = hd.TenDichVu,
                DonViTinh = hd.DonViTinh,
                IsBatBuoc = hd.IsBatBuoc,
                TrangThaiDichVuId = hd.DichVuTrangThaiId,
                TrangThaiDichVuTen = trangThaiDichVuMap.GetValueOrDefault(hd.DichVuTrangThaiId, string.Empty)
            };

            var relatedTeps = teps.Where(t => t.HopDongDoiTacId == hd.HopDongUid);
            foreach (var tp in relatedTeps)
            {
                existingHopDong.Teps.Add(new UploadFileResponse(tp.FileUid, tp.FileName, tp.FileUrl, tp.ContentType));
            }

            doiTac.HopDongs.Add(existingHopDong);
        }

        return doiTac;
    }
}
