using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Application.Features.Catalog.DTOs;
using HeThongChungCu.Application.Features.Catalog.Queries.LayCauTrucChungCu;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Application.Features.Tang.Queries.GetListTang;
using HeThongChungCu.Application.Features.Tang.Queries.GetTangById;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Application.Features.ToaNha.Queries.GetListToaNha;
using HeThongChungCu.Application.Features.ToaNha.Queries.GetToaNhaById;

namespace HeThongChungCu.Infrastructure.Persistence.Repositories.QueryRepositories;

public class ToaNhaQueryRepository : IToaNhaQueryRepository
{
    private readonly AppDbContext _dbContext;
    public ToaNhaQueryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ToaNhaDetailResponse>> GetAllAsync(
        GetListToaNhaSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "Id" },
            { "MaToaNha", "MaToaNha" },
            { "TenToaNha", "TenToaNha" },
            { "IsDeleted", "IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT COUNT(*) OVER() AS TotalCount, Id, MaToaNha, TenToaNha, DiaChi, MoTa, TrangThaiToaNhaId,
                   (SELECT COUNT(*) FROM CanHo c JOIN Tang t ON c.TangId = t.Id WHERE t.ToaNhaId = ToaNha.Id AND c.IsDeleted = 0 AND t.IsDeleted = 0) AS SoCanHo
            FROM ToaNha
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;

        var transaction = _dbContext.GetDbTransaction();
        var rows = (await connection.QueryAsync<ToaNhaReadModel>(sql, parameters, transaction: transaction)).ToList();

        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var trangThaiToaNhaMap = TrangThaiToaNha.ToDictionary();

        var items = rows.Select(r => new ToaNhaDetailResponse
        {
            Id = r.Id,
            MaToaNha = r.MaToaNha,
            TenToaNha = r.TenToaNha,
            DiaChi = r.DiaChi,
            MoTa = r.MoTa,
            TrangThaiToaNhaId = r.TrangThaiToaNhaId,
            SoCanHo = r.SoCanHo,
            TenTrangThaiToaNha = trangThaiToaNhaMap.GetValueOrDefault(r.TrangThaiToaNhaId, string.Empty)
        }).ToList();

        return new PagedResult<ToaNhaDetailResponse>
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

    public async Task<ToaNhaResponse?> GetByIdAsync(GetToaNhaByIdSpecification spec, CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "tn.Id" },
            { "IsDeleted", "tn.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);

        var sqlJoinsTang = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("Tang", "t", "t.ToaNhaId = tn.Id", JoinType.Inner)
        ]);

        var sqlJoinsCanHo = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("Tang", "t", "t.ToaNhaId = tn.Id", JoinType.Inner),
            new JoinDefinition("CanHo", "c", "c.TangId = t.Id", JoinType.Inner)
        ]);

        var sql = $"""
            SELECT tn.Id, tn.MaToaNha, tn.TenToaNha, tn.DiaChi, tn.MoTa, tn.TrangThaiToaNhaId,
                   (SELECT COUNT(*) FROM CanHo c JOIN Tang t ON c.TangId = t.Id WHERE t.ToaNhaId = tn.Id AND c.IsDeleted = 0 AND t.IsDeleted = 0) AS SoCanHo
            FROM ToaNha tn
            {sqlWhere};

            SELECT t.Id AS TangUid, t.MaTang, t.TenTang, t.LoaiTangId
            FROM ToaNha tn
            {sqlJoinsTang}
            {sqlWhere};

            SELECT c.Id AS CanHoId, c.MaCanHo, c.TenCanHo, c.DienTich, c.SoPhongNgu, c.SoPhongTam, c.LoaiCanHoId, c.TinhTrangCanHoId, t.Id AS TangUid
            FROM ToaNha tn
            {sqlJoinsCanHo}
            {sqlWhere};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());
        var firstRow = await multi.ReadFirstOrDefaultAsync<ToaNhaDetailReadModel>();

        if (firstRow == null)
            return null;

        var toaNha = new ToaNhaResponse
        {
            Id = firstRow.Id,
            MaToaNha = firstRow.MaToaNha,
            TenToaNha = firstRow.TenToaNha,
            SoCanHo = firstRow.SoCanHo,
            DiaChi = firstRow.DiaChi,
            MoTa = firstRow.MoTa,
            TrangThaiToaNhaId = firstRow.TrangThaiToaNhaId,
            TenTrangThaiToaNha = TrangThaiToaNha.ToDictionary().GetValueOrDefault(firstRow.TrangThaiToaNhaId, string.Empty)
        };

        var loaiTangMap = LoaiTang.ToDictionary();
        var loaiCanHoMap = LoaiCanHo.ToDictionary();
        var tinhTrangCanHoMap = TrangThaiCanHo.ToDictionary();

        var tangRows = (await multi.ReadAsync<TangInToaNhaReadModel>()).ToList();
        var canHoRows = (await multi.ReadAsync<CanHoInToaNhaReadModel>()).ToList();

        var tangMap = new Dictionary<int, TangResponse>();

        foreach (var r in tangRows)
        {
            var tangId = r.TangUid;
            var tang = new TangResponse
            {
                Id = tangId,
                MaTang = r.MaTang,
                TenTang = r.TenTang,
                LoaiTangId = r.LoaiTangId,
                TenLoaiTang = loaiTangMap.GetValueOrDefault(r.LoaiTangId, string.Empty),
                ToaNhaId = toaNha.Id,
                TenToaNha = toaNha.TenToaNha,
                CanHos = new List<CanHoDetailResponse>()
            };
            tangMap.Add(tangId, tang);
        }

        foreach (var r in canHoRows)
        {
            var tangId = r.TangUid;
            if (tangMap.TryGetValue(tangId, out var tang))
            {
                ((List<CanHoDetailResponse>)tang.CanHos).Add(new CanHoDetailResponse
                {
                    Id = r.CanHoId,
                    MaCanHo = r.MaCanHo,
                    TenCanHo = r.TenCanHo,
                    TangId = tangId,
                    TenTang = tang.TenTang,
                    DienTich = r.DienTich,
                    SoPhongNgu = r.SoPhongNgu,
                    SoPhongTam = r.SoPhongTam,
                    LoaiCanHoId = r.LoaiCanHoId,
                    TinhTrangCanHoId = r.TinhTrangCanHoId,
                    TenLoaiCanHo = loaiCanHoMap.GetValueOrDefault(r.LoaiCanHoId, string.Empty),
                    TenTinhTrangCanHo = tinhTrangCanHoMap.GetValueOrDefault(r.TinhTrangCanHoId, string.Empty)
                });
            }
        }

        toaNha.Tangs = tangMap.Values.ToList();

        return toaNha;
    }

    public async Task<PagedResult<TangDetailResponse>> GetTangsAllAsync(
        GetListTangSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "t.Id" },
            { "MaTang", "t.MaTang" },
            { "TenTang", "t.TenTang" },
            { "ToaNhaId", "t.ToaNhaId" },
            { "LoaiTangId", "t.LoaiTangId" },
            { "IsDeleted", "t.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("ToaNha", "tn", "tn.Id = t.ToaNhaId", Type: JoinType.Inner)
        ]);

        var sqlOrderBy = DapperQueryBuilder.BuildOrderBy(spec, columnMapping, "Id");
        var sqlPagination = DapperQueryBuilder.BuildPagination(spec, parameters);

        var sql = $"""
            SELECT
                COUNT(*) OVER() AS TotalCount,
                t.Id,
                t.MaTang,
                t.TenTang,
                t.LoaiTangId,
                t.ToaNhaId,
                tn.TenToaNha
            FROM Tang t
            {sqlJoins}
            {sqlWhere}
            {sqlOrderBy}
            {sqlPagination};
            """;


        var rows = (await connection.QueryAsync<TangReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction())).ToList();

        var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

        var loaiTangMap = LoaiTang.ToDictionary();

        var items = rows.Select(r => new TangDetailResponse
        {
            Id = r.Id,
            MaTang = r.MaTang,
            TenTang = r.TenTang,
            LoaiTangId = r.LoaiTangId,
            ToaNhaId = r.ToaNhaId,
            TenToaNha = r.TenToaNha,
            TenLoaiTang = loaiTangMap.GetValueOrDefault(r.LoaiTangId, string.Empty)
        }).ToList();

        return new PagedResult<TangDetailResponse>
        {
            Items = items,
            PagingInfo = new PagingInfo
            {
                PageNumber = spec.PageNumber ?? 1,
                PageSize = spec.PageSize ?? (items.Count == 0 ? 10 : items.Count),
                TotalItems = totalCount
            }
        };
    }

    public async Task<TangResponse?> GetTangDetailByIdAsync(
        GetTangByIdSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "t.Id" },
            { "IsDeleted", "t.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("ToaNha", "tn", "tn.Id = t.ToaNhaId", Type: JoinType.Inner)
        ]);

        var sqlJoinsCanHo = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("CanHo", "c", "c.TangId = t.Id", JoinType.Inner)
        ]);

        var sql = $"""
            SELECT t.Id, t.MaTang, t.TenTang, t.LoaiTangId, t.ToaNhaId, tn.TenToaNha
            FROM Tang t
            {sqlJoins}
            {sqlWhere};

            SELECT c.Id AS CanHoId, c.MaCanHo, c.TenCanHo, c.DienTich, c.SoPhongNgu, c.SoPhongTam, c.LoaiCanHoId, c.TinhTrangCanHoId
            FROM Tang t
            {sqlJoinsCanHo}
            {sqlWhere};
            """;

        using var multi = await connection.QueryMultipleAsync(sql, parameters, transaction: _dbContext.GetDbTransaction());
        var firstRow = await multi.ReadFirstOrDefaultAsync<TangDetailReadModel>();

        if (firstRow == null)
            return null;

        var tang = new TangResponse
        {
            Id = firstRow.Id,
            MaTang = firstRow.MaTang ?? string.Empty,
            TenTang = firstRow.TenTang ?? string.Empty,
            LoaiTangId = firstRow.LoaiTangId,
            ToaNhaId = firstRow.ToaNhaId,
            TenToaNha = firstRow.TenToaNha ?? string.Empty,
            TenLoaiTang = LoaiTang.ToDictionary().GetValueOrDefault(firstRow.LoaiTangId, string.Empty)
        };

        var loaiCanHoDict = LoaiCanHo.ToDictionary();
        var tinhTrangCanHoDict = TrangThaiCanHo.ToDictionary();

        var canHoRows = await multi.ReadAsync<CanHoInTangReadModel>();

        var canHos = canHoRows
            .Select(r => new CanHoDetailResponse
            {
                Id = r.CanHoId,
                TangId = tang.Id,
                TenTang = tang.TenTang,
                MaCanHo = r.MaCanHo,
                TenCanHo = r.TenCanHo,
                DienTich = r.DienTich,
                SoPhongNgu = r.SoPhongNgu,
                SoPhongTam = r.SoPhongTam,
                LoaiCanHoId = r.LoaiCanHoId,
                TinhTrangCanHoId = r.TinhTrangCanHoId,
                TenLoaiCanHo = loaiCanHoDict.GetValueOrDefault(r.LoaiCanHoId, string.Empty),
                TenTinhTrangCanHo = tinhTrangCanHoDict.GetValueOrDefault(r.TinhTrangCanHoId, string.Empty)
            })
            .ToList();

        tang.CanHos = canHos;

        return tang;
    }

    public async Task<IReadOnlyList<CauTrucToaNhaResponse>> GetCauTrucChungCuAsync(
        LayCauTrucChungCuSpecification spec,
        CancellationToken cancellationToken = default)
    {
        var connection = _dbContext.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        var columnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "MaToaNha", "t.MaToaNha" },
            { "TenToaNha", "t.TenToaNha" },
            { "MaTang", "f.MaTang" },
            { "TenTang", "f.TenTang" },
            { "LoaiTangId", "f.LoaiTangId" },
            { "MaCanHo", "c.MaCanHo" },
            { "TenCanHo", "c.TenCanHo" },
            { "IsDeleted", "t.IsDeleted" },
        };

        var parameters = new DynamicParameters();
        var sqlWhere = DapperQueryBuilder.BuildWhere(spec, columnMapping, parameters);
        var sqlJoins = DapperQueryBuilder.BuildJoin([
            new JoinDefinition("Tang", "f", "f.ToaNhaId = t.Id"),
            new JoinDefinition("CanHo", "c", "c.TangId = f.Id")
        ]);

        var sql = $"""
            SELECT 
                t.Id AS ToaNhaId, t.MaToaNha, t.TenToaNha, t.TrangThaiToaNhaId AS ToaNhaTrangThaiId,
                f.Id AS TangId, f.MaTang, f.TenTang,
                c.Id AS CanHoId, c.MaCanHo, c.TenCanHo, c.TinhTrangCanHoId AS CanHoTrangThaiId
            FROM ToaNha t
            {sqlJoins}
            {sqlWhere}
            ORDER BY t.TenToaNha, f.Id, c.MaCanHo
            """;

        var rows = await connection.QueryAsync<CauTrucChungCuReadModel>(sql, parameters, transaction: _dbContext.GetDbTransaction());

        var toaNhaMap = new Dictionary<int, CauTrucToaNhaResponse>();
        var tangMap = new Dictionary<int, CauTrucTangResponse>();

        var trangThaiToaNhaMap = TrangThaiToaNha.ToDictionary();
        var tinhTrangCanHoMap = TrangThaiCanHo.ToDictionary();

        foreach (var r in rows)
        {
            if (!toaNhaMap.TryGetValue(r.ToaNhaId, out var toaNha))
            {
                toaNha = new CauTrucToaNhaResponse
                {
                    Id = r.ToaNhaId,
                    MaToaNha = r.MaToaNha,
                    TenToaNha = r.TenToaNha,
                    TrangThaiId = r.ToaNhaTrangThaiId,
                    TenTrangThai = trangThaiToaNhaMap.GetValueOrDefault(r.ToaNhaTrangThaiId, string.Empty),
                    CauTrucTangs = new List<CauTrucTangResponse>()
                };
                toaNhaMap.Add(toaNha.Id, toaNha);
            }

            if (r.TangId.HasValue)
            {
                if (!tangMap.TryGetValue(r.TangId.Value, out var tang))
                {
                    tang = new CauTrucTangResponse
                    {
                        Id = r.TangId.Value,
                        MaTang = r.MaTang ?? string.Empty,
                        TenTang = r.TenTang ?? string.Empty,
                        CauTrucCanHos = new List<CauTrucCanHoResponse>()
                    };
                    tangMap.Add(tang.Id, tang);
                    toaNha.CauTrucTangs.Add(tang);
                }

                if (r.CanHoId.HasValue)
                {
                    tang.CauTrucCanHos.Add(new CauTrucCanHoResponse
                    {
                        Id = r.CanHoId.Value,
                        MaCanHo = r.MaCanHo ?? string.Empty,
                        TenCanHo = r.TenCanHo ?? string.Empty,
                        TrangThaiId = r.CanHoTrangThaiId ?? 0,
                        TenTrangThai = tinhTrangCanHoMap.GetValueOrDefault(r.CanHoTrangThaiId ?? 0, string.Empty)
                    });
                }
            }
        }

        return toaNhaMap.Values.ToList();
    }
}
