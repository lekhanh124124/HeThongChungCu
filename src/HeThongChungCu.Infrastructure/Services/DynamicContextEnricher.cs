using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace HeThongChungCu.Infrastructure.Services;

/// <summary>
/// Bổ sung ngữ cảnh thời gian thực vào pipeline chatbot bằng cách phát hiện intent
/// từ câu hỏi và truy vấn trực tiếp DB (không qua Qdrant).
/// Dữ liệu luôn mới nhất, không cần sync/embed.
/// </summary>
public class DynamicContextEnricher : IChatbotContextEnricher
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DynamicContextEnricher> _logger;

    // ═══════════════════════════════════════════════════════════════
    // KEYWORD DICTIONARIES — Intent Detection
    // ═══════════════════════════════════════════════════════════════

    private static readonly string[] _dichVuKeywords =
    [
        "dịch vụ", "đăng ký dịch vụ", "dịch vụ nào", "có dịch vụ",
        "giữ xe", "vệ sinh", "dọn dẹp", "nước", "điện", "internet", "wifi",
        "thang máy", "bể bơi", "gym", "gym",
        "dịch vụ bắt buộc", "dịch vụ miễn phí"
    ];

    private static readonly string[] _bangGiaKeywords =
    [
        "bảng giá", "giá bao nhiêu", "giá tiền", "giá dịch vụ",
        "phí", "chi phí", "cước phí", "đơn giá",
        "giá giữ xe", "giá vệ sinh", "giá điện", "giá nước",
        "bao nhiêu tiền", "tốn bao nhiêu"
    ];

    private static readonly string[] _thongBaoKeywords =
    [
        "thông báo", "thông tin mới", "tin tức", "cập nhật",
        "thông báo mới nhất", "có gì mới", "gần đây",
        "ban quản lý thông báo", "tòa nhà thông báo"
    ];

    public DynamicContextEnricher(AppDbContext dbContext, ILogger<DynamicContextEnricher> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<string> EnrichAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            return string.Empty;

        var lowerPrompt = prompt.ToLowerInvariant();
        var contextParts = new List<string>();

        var hasDichVuIntent = _dichVuKeywords.Any(k => lowerPrompt.Contains(k));
        var hasBangGiaIntent = _bangGiaKeywords.Any(k => lowerPrompt.Contains(k));
        var hasThongBaoIntent = _thongBaoKeywords.Any(k => lowerPrompt.Contains(k));

        _logger.LogDebug(
            "[DynamicContext] Intent detected — DichVu: {D}, BangGia: {B}, ThongBao: {T}",
            hasDichVuIntent, hasBangGiaIntent, hasThongBaoIntent);

        // Không có intent nào → trả về rỗng
        if (!hasDichVuIntent && !hasBangGiaIntent && !hasThongBaoIntent)
            return string.Empty;

        var connection = _dbContext.GetDbConnection();
        var now = DateTimeOffset.UtcNow;

        // ─── Dịch vụ đang hoạt động ─────────────────────────────────
        if (hasDichVuIntent || hasBangGiaIntent)
        {
            var dichVuContext = await GetDichVuContextAsync(connection, now, hasBangGiaIntent, cancellationToken);
            if (!string.IsNullOrEmpty(dichVuContext))
                contextParts.Add(dichVuContext);
        }

        // ─── Thông báo gần nhất (7 ngày) ────────────────────────────
        if (hasThongBaoIntent)
        {
            var thongBaoContext = await GetThongBaoContextAsync(connection, now, cancellationToken);
            if (!string.IsNullOrEmpty(thongBaoContext))
                contextParts.Add(thongBaoContext);
        }

        if (!contextParts.Any())
            return string.Empty;

        var header = $"\n[DỮ LIỆU THỜI GIAN THỰC — Cập nhật lúc {now:HH:mm dd/MM/yyyy}]\n";
        return header + string.Join("\n\n", contextParts) + "\n[HẾT DỮ LIỆU THỜI GIAN THỰC]\n";
    }

    // ─── Private helpers ─────────────────────────────────────────────────────

    private static async Task<string> GetDichVuContextAsync(
        System.Data.IDbConnection connection,
        DateTimeOffset now,
        bool includeBangGia,
        CancellationToken cancellationToken)
    {
        // Lấy danh sách dịch vụ đang hoạt động
        const string dichVuSql = """
            SELECT dv.TenDichVu, dv.MoTa, dv.DonViTinh, dv.IsBatBuoc
            FROM DichVu dv
            WHERE dv.TrangThaiId = 1  -- TrangThaiDichVu.HoatDong
              AND dv.IsDeleted = 0
            ORDER BY dv.IsBatBuoc DESC, dv.TenDichVu
            """;

        var dichVus = await connection.QueryAsync<DichVuRow>(
            new CommandDefinition(dichVuSql, cancellationToken: cancellationToken));

        if (!dichVus.Any())
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("## Danh sách dịch vụ đang cung cấp:");

        foreach (var dv in dichVus)
        {
            var loai = dv.IsBatBuoc ? "(bắt buộc)" : "(tùy chọn)";
            sb.AppendLine($"- **{dv.TenDichVu}** {loai} — Đơn vị: {dv.DonViTinh}");
            if (!string.IsNullOrEmpty(dv.MoTa))
                sb.AppendLine($"  _{dv.MoTa}_");
        }

        // Lấy bảng giá hiện hành nếu có intent
        if (includeBangGia)
        {
            const string bangGiaSql = """
                SELECT dv.TenDichVu, bg.TenBangGia, bg.LoaiDinhGiaId AS Discriminator,
                       bg.DonGia
                FROM DichVu dv
                INNER JOIN BangGia bg ON bg.DichVuId = dv.Id
                    AND bg.IsActive = 1
                    AND bg.NgayApDung <= @Now
                    AND (bg.NgayKetThuc IS NULL OR bg.NgayKetThuc >= @Now)
                WHERE dv.TrangThaiId = 1  -- TrangThaiDichVu.HoatDong
                  AND dv.IsDeleted = 0
                ORDER BY dv.TenDichVu
                """;

            var bangGias = await connection.QueryAsync<BangGiaRow>(
                new CommandDefinition(bangGiaSql, new { Now = now }, cancellationToken: cancellationToken));

            if (bangGias.Any())
            {
                sb.AppendLine();
                sb.AppendLine("## Bảng giá hiện hành:");
                foreach (var bg in bangGias)
                {
                    if (bg.DonGia.HasValue)
                        sb.AppendLine($"- **{bg.TenDichVu}**: {bg.DonGia.Value:N0} VNĐ ({bg.TenBangGia})");
                    else
                        sb.AppendLine($"- **{bg.TenDichVu}**: Xem chi tiết bảng giá '{bg.TenBangGia}'");
                }
            }
        }

        return sb.ToString();
    }

    private static async Task<string> GetThongBaoContextAsync(
        System.Data.IDbConnection connection,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var sevenDaysAgo = now.AddDays(-7);

        const string sql = """
            SELECT TOP 5 tb.TieuDe, tb.NoiDung, tb.CreatedAt
            FROM ThongBao tb
            WHERE tb.IsDeleted = 0
              AND tb.LoaiThongBao = 5  -- LoaiThongBao.HeThong
              AND tb.CreatedAt >= @From
            ORDER BY tb.CreatedAt DESC
            """;

        var thongBaos = await connection.QueryAsync<ThongBaoRow>(
            new CommandDefinition(sql, new { From = sevenDaysAgo }, cancellationToken: cancellationToken));

        if (!thongBaos.Any())
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("## Thông báo gần đây (7 ngày qua):");
        foreach (var tb in thongBaos)
        {
            sb.AppendLine($"- **{tb.TieuDe}** ({tb.CreatedAt:dd/MM/yyyy})");
            if (!string.IsNullOrEmpty(tb.NoiDung) && tb.NoiDung.Length <= 200)
                sb.AppendLine($"  {tb.NoiDung}");
        }

        return sb.ToString();
    }

    // ─── Private DTOs for Dapper ─────────────────────────────────────────────

    private sealed record DichVuRow(string TenDichVu, string? MoTa, string DonViTinh, bool IsBatBuoc);
    private sealed record BangGiaRow(string TenDichVu, string TenBangGia, int Discriminator, decimal? DonGia);
    private sealed record ThongBaoRow(string TieuDe, string? NoiDung, DateTimeOffset CreatedAt);
}
