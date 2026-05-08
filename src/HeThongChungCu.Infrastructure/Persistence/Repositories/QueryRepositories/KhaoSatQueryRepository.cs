using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatList;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatById;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKetQuaKhaoSat;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Infrastructure.Persistence.Helpers;
using HeThongChungCu.Infrastructure.Persistence.ReadModels;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class KhaoSatQueryRepository : IKhaoSatQueryRepository
{
    private readonly AppDbContext _dbContext;

    public KhaoSatQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<KhaoSatResponse>> GetAllAsync(
        GetKhaoSatListSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "k.Id" },
            { "TieuDe", "k.TieuDe" },
            { "MoTa", "k.MoTa" },
            { "LoaiKhaoSatId", "k.LoaiKhaoSatId" },
            { "CoCheTinhDiemId", "k.CoCheTinhDiemId" },
            { "TrangThaiId", "k.TrangThaiId" },
            { "NgayBatDau", "k.NgayBatDau" },
            { "NgayKetThuc", "k.NgayKetThuc" },
            { "CreatedAt", "k.CreatedAt" },
            { "KhaoSatIsDeleted", "k.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "CreatedAt");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                k.Id,
                k.TieuDe,
                k.MoTa,
                k.LoaiKhaoSatId,
                k.CoCheTinhDiemId,
                k.TrangThaiId,
                k.NgayBatDau,
                k.NgayKetThuc,
                k.TyleThamGiaToiThieu,
                k.TyLeDongYToiThieu,
                k.IsAnDanh,
                k.CreatedAt
            FROM KhaoSat k
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination}
            """;

        var transaction = _dbContext.GetDbTransaction();
        var rows = (await connection.QueryAsync<KhaoSatReadModel>(sql, parameters, transaction: transaction)).ToList();
        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var votedKhaoSatIds = new HashSet<int>();
        var currentUserId = spec.CurrentUserId;
        if (currentUserId.HasValue)
        {
            var sqlVoted = """
                SELECT DISTINCT b.KhaoSatId
                FROM BieuQuyetCuDan b
                INNER JOIN QuanHeCuTru q ON b.CanHoId = q.CanHoId
                WHERE q.NguoiDungId = @UserId 
                  AND q.TrangThaiCuTruId = 1 
                  AND q.IsDeleted = 0 
                  AND b.IsDeleted = 0
                """;
            var votedIds = await connection.QueryAsync<int>(sqlVoted, new { UserId = currentUserId.Value }, transaction: transaction);
            votedKhaoSatIds = new HashSet<int>(votedIds);
        }

        var loaiMap = LoaiKhaoSat.ToDictionary();
        var coCheMap = CoCheTinhDiemBauCu.ToDictionary();
        var trangThaiMap = TrangThaiKhaoSat.ToDictionary();

        var items = rows.Select(r => new KhaoSatResponse
        {
            Id = r.Id,
            TieuDe = r.TieuDe,
            MoTa = r.MoTa,
            LoaiKhaoSatId = r.LoaiKhaoSatId,
            LoaiKhaoSatTen = loaiMap.GetValueOrDefault(r.LoaiKhaoSatId, string.Empty),
            CoCheTinhDiemId = r.CoCheTinhDiemId,
            CoCheTinhDiemTen = coCheMap.GetValueOrDefault(r.CoCheTinhDiemId, string.Empty),
            TrangThaiId = r.TrangThaiId,
            TrangThaiTen = trangThaiMap.GetValueOrDefault(r.TrangThaiId, string.Empty),
            NgayBatDau = r.NgayBatDau,
            NgayKetThuc = r.NgayKetThuc,
            TyleThamGiaToiThieu = r.TyleThamGiaToiThieu,
            TyLeDongYToiThieu = r.TyLeDongYToiThieu,
            IsAnDanh = r.IsAnDanh,
            IsVoted = votedKhaoSatIds.Contains(r.Id),
            CreatedAt = r.CreatedAt
        }).ToList();

        return new PagedResult<KhaoSatResponse>
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



    public async Task<KhaoSatDetailResponse?> GetByIdAsync(
        GetKhaoSatByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "k.Id" },
            { "KhaoSatIsDeleted", "k.IsDeleted" }
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sqlJoinCauHoi = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("CauHoiKhaoSat", "q", "q.KhaoSatId = k.Id", JoinType.Inner, Mapping: new() { { "CauHoiIsDeleted", "q.IsDeleted" } })
        ], parameters);

        var sqlJoinLuaChon = DapperQueryBuilder.BuildJoin(spec, [
            new JoinDefinition("CauHoiKhaoSat", "q", "q.KhaoSatId = k.Id", JoinType.Inner, Mapping: new() { { "CauHoiIsDeleted", "q.IsDeleted" } }),
            new JoinDefinition("LuaChonKhaoSat", "o", "o.CauHoiKhaoSatId = q.Id", JoinType.Inner, Mapping: new() { { "LuaChonIsDeleted", "o.IsDeleted" } })
        ], parameters);

        var sql = $"""
            -- 1. KhaoSat Main Info
            SELECT
                k.Id, k.TieuDe, k.MoTa, k.LoaiKhaoSatId, k.CoCheTinhDiemId, k.TrangThaiId,
                k.NgayBatDau, k.NgayKetThuc, k.TyleThamGiaToiThieu, k.TyLeDongYToiThieu, k.IsAnDanh, k.CreatedAt
            FROM KhaoSat k
            {sqlWhere};

            -- 2. Questions
            SELECT DISTINCT
                q.Id, q.KhaoSatId, q.NoiDungCauHoi, q.IsBatBuoc, q.IsMultiSelect
            FROM KhaoSat k
            {sqlJoinCauHoi}
            {sqlWhere};

            -- 3. Options
            SELECT DISTINCT
                o.Id, o.CauHoiKhaoSatId, o.NoiDungLuaChon, o.IsUngVienBQT, o.TieuSuUngVien, o.UngVienId
            FROM KhaoSat k
            {sqlJoinLuaChon}
            {sqlWhere};
            """;

        var transaction = _dbContext.GetDbTransaction();
        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: transaction);

        var mainReadModel = await multi.ReadFirstOrDefaultAsync<KhaoSatReadModel>();
        if (mainReadModel == null) return null;

        var questions = (await multi.ReadAsync<CauHoiKhaoSatReadModel>()).ToList();
        var options = (await multi.ReadAsync<LuaChonKhaoSatReadModel>()).ToList();

        var loaiMap = LoaiKhaoSat.ToDictionary();
        var coCheMap = CoCheTinhDiemBauCu.ToDictionary();
        var trangThaiMap = TrangThaiKhaoSat.ToDictionary();

        var isVoted = false;
        var currentUserId = spec.CurrentUserId;
        if (currentUserId.HasValue)
        {
            var sqlVoted = """
                SELECT COUNT(1)
                FROM BieuQuyetCuDan b
                INNER JOIN QuanHeCuTru q ON b.CanHoId = q.CanHoId
                WHERE q.NguoiDungId = @UserId 
                  AND q.TrangThaiCuTruId = 1 
                  AND q.IsDeleted = 0 
                  AND b.KhaoSatId = @KhaoSatId
                  AND b.IsDeleted = 0
                """;
            var count = await connection.ExecuteScalarAsync<int>(sqlVoted, new { UserId = currentUserId.Value, KhaoSatId = mainReadModel.Id }, transaction: transaction);
            isVoted = count > 0;
        }

        var result = new KhaoSatDetailResponse
        {
            Id = mainReadModel.Id,
            TieuDe = mainReadModel.TieuDe,
            MoTa = mainReadModel.MoTa,
            LoaiKhaoSatId = mainReadModel.LoaiKhaoSatId,
            LoaiKhaoSatTen = loaiMap.GetValueOrDefault(mainReadModel.LoaiKhaoSatId, string.Empty),
            CoCheTinhDiemId = mainReadModel.CoCheTinhDiemId,
            CoCheTinhDiemTen = coCheMap.GetValueOrDefault(mainReadModel.CoCheTinhDiemId, string.Empty),
            TrangThaiId = mainReadModel.TrangThaiId,
            TrangThaiTen = trangThaiMap.GetValueOrDefault(mainReadModel.TrangThaiId, string.Empty),
            NgayBatDau = mainReadModel.NgayBatDau,
            NgayKetThuc = mainReadModel.NgayKetThuc,
            TyleThamGiaToiThieu = mainReadModel.TyleThamGiaToiThieu,
            TyLeDongYToiThieu = mainReadModel.TyLeDongYToiThieu,
            IsAnDanh = mainReadModel.IsAnDanh,
            IsVoted = isVoted,
            CreatedAt = mainReadModel.CreatedAt,
            CauHois = []
        };

        foreach (var q in questions)
            result.CauHois.Add(new CauHoiKhaoSatResponse
            {
                Id = q.Id,
                NoiDungCauHoi = q.NoiDungCauHoi,
                IsBatBuoc = q.IsBatBuoc,
                IsMultiSelect = q.IsMultiSelect,
                LuaChons = options.Where(o => o.CauHoiKhaoSatId == q.Id).Select(o => new LuaChonKhaoSatResponse
                {
                    Id = o.Id,
                    NoiDungLuaChon = o.NoiDungLuaChon,
                    IsUngVienBQT = o.IsUngVienBQT,
                    TieuSuUngVien = o.TieuSuUngVien,
                    UngVienId = o.UngVienId
                }).ToList()
            });

        return result;
    }

    public async Task<KetQuaKhaoSatResponse?> GetKetQuaKhaoSatAsync(
        GetKetQuaKhaoSatSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        // Get the Campaign Id from specification filters
        var parameters = new DynamicParameters();
        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "k.Id" },
            { "KhaoSatIsDeleted", "k.IsDeleted" }
        };
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        // 1. Fetch Main Campaign
        var sqlCampaign = $"""
            SELECT
                k.Id, k.TieuDe, k.MoTa, k.LoaiKhaoSatId, k.CoCheTinhDiemId, k.TrangThaiId,
                k.NgayBatDau, k.NgayKetThuc, k.TyleThamGiaToiThieu, k.TyLeDongYToiThieu, k.IsAnDanh, k.CreatedAt
            FROM KhaoSat k
            {sqlWhere}
            """;

        var transaction = _dbContext.GetDbTransaction();
        var campaign = await connection.QueryFirstOrDefaultAsync<KhaoSatReadModel>(sqlCampaign, parameters, transaction: transaction);
        if (campaign == null) return null;

        // 2. Compute Statistics:
        // A. Total Active Apartments
        var sqlTotalCanHo = "SELECT COUNT(*) FROM CanHo WHERE IsDeleted = 0";
        var totalCanHo = await connection.ExecuteScalarAsync<int>(sqlTotalCanHo, transaction: transaction);
        if (totalCanHo == 0) totalCanHo = 1; // Safeguard

        // B. Total Voted Apartments
        var sqlVotedCanHo = "SELECT COUNT(DISTINCT CanHoId) FROM BieuQuyetCuDan WHERE KhaoSatId = @KhaoSatId AND IsDeleted = 0";
        var votedCanHo = await connection.QueryFirstOrDefaultAsync<int>(sqlVotedCanHo, new { KhaoSatId = campaign.Id }, transaction: transaction);

        decimal rateParticipation = ((decimal)votedCanHo * 100.0m) / totalCanHo;
        bool isHieuLuc = rateParticipation >= campaign.TyleThamGiaToiThieu;

        // 3. Fetch Questions
        var sqlQuestions = $"""
            SELECT q.Id, q.KhaoSatId, q.NoiDungCauHoi, q.IsBatBuoc, q.IsMultiSelect
            FROM CauHoiKhaoSat q
            WHERE q.KhaoSatId = @KhaoSatId AND q.IsDeleted = 0
            """;
        var questions = (await connection.QueryAsync<CauHoiKhaoSatReadModel>(sqlQuestions, new { KhaoSatId = campaign.Id }, transaction: transaction)).ToList();

        // 4. Fetch Options and vote sum metrics
        // We calculate vote sum based on Weighted or Count based mechanism
        var sqlStats = $"""
            SELECT 
                o.Id AS LuaChonId,
                o.CauHoiKhaoSatId AS CauHoiId,
                o.NoiDungLuaChon,
                o.IsUngVienBQT,
                COALESCE(SUM(v.TrongSoBieuQuyet), 0) AS SoLuongPhieu
            FROM LuaChonKhaoSat o
            INNER JOIN CauHoiKhaoSat q ON o.CauHoiKhaoSatId = q.Id
            LEFT JOIN ChiTietBieuQuyet d ON d.LuaChonKhaoSatId = o.Id AND d.IsDeleted = 0
            LEFT JOIN BieuQuyetCuDan v ON d.BieuQuyetCuDanId = v.Id AND v.IsDeleted = 0 AND v.KhaoSatId = q.KhaoSatId
            WHERE q.KhaoSatId = @KhaoSatId AND o.IsDeleted = 0 AND q.IsDeleted = 0
            GROUP BY o.Id, o.CauHoiKhaoSatId, o.NoiDungLuaChon, o.IsUngVienBQT
            """;

        var statRows = (await connection.QueryAsync(sqlStats, new { KhaoSatId = campaign.Id }, transaction: transaction)).ToList();

        var coCheMap = CoCheTinhDiemBauCu.ToDictionary();

        var response = new KetQuaKhaoSatResponse
        {
            KhaoSatId = campaign.Id,
            TieuDeKhaoSat = campaign.TieuDe,
            TongSoCanHo = totalCanHo,
            SoCanHoDaThamGia = votedCanHo,
            TyLeThamGia = Math.Round(rateParticipation, 2),
            TyleThamGiaToiThieu = campaign.TyleThamGiaToiThieu,
            IsHieuLuc = isHieuLuc,
            CoCheTinhDiemId = campaign.CoCheTinhDiemId,
            CoCheTinhDiemTen = coCheMap.GetValueOrDefault(campaign.CoCheTinhDiemId, string.Empty),
            KetQuaCauHois = []
        };

        foreach (var q in questions)
        {
            var qRows = statRows.Where(r => (int)r.CauHoiId == q.Id).ToList();
            
            // Total vote weight for this question to calculate individual option percentages
            decimal totalWeightForQuestion = 0;
            foreach (var r in qRows)
                totalWeightForQuestion += (decimal)r.SoLuongPhieu;

            var qResult = new KetQuaCauHoiResponse
            {
                CauHoiId = q.Id,
                NoiDungCauHoi = q.NoiDungCauHoi,
                IsMultiSelect = q.IsMultiSelect,
                KetQuaLuaChons = []
            };

            foreach (var r in qRows)
            {
                decimal votes = (decimal)r.SoLuongPhieu;
                decimal pct = totalWeightForQuestion > 0 ? (votes * 100.0m) / totalWeightForQuestion : 0.0m;

                qResult.KetQuaLuaChons.Add(new KetQuaLuaChonResponse
                {
                    LuaChonId = (int)r.LuaChonId,
                    NoiDungLuaChon = (string)r.NoiDungLuaChon,
                    IsUngVienBQT = (bool)r.IsUngVienBQT,
                    SoLuongPhieuBau = Math.Round(votes, 2),
                    TyLePhanTram = Math.Round(pct, 2)
                });
            }

            response.KetQuaCauHois.Add(qResult);
        }

        return response;
    }
}
