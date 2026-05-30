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

    private static readonly string[] _baoTriKeywords =
    [
        "bảo trì", "bảo dưỡng", "sửa chữa thiết bị", "lịch bảo trì",
        "thang máy bảo trì", "bảo trì thang máy", "sửa thang máy", "bảo trì điện",
        "bảo trì nước", "bảo trì pccc", "hạ tầng bảo trì"
    ];

    private static readonly string[] _khaoSatKeywords =
    [
        "khảo sát", "biểu quyết", "lấy ý kiến", "bình chọn", "bầu cử",
        "phiếu khảo sát", "tham gia khảo sát", "khảo sát cư dân"
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
        var hasBaoTriIntent = _baoTriKeywords.Any(k => lowerPrompt.Contains(k));
        var hasKhaoSatIntent = _khaoSatKeywords.Any(k => lowerPrompt.Contains(k));

        _logger.LogDebug(
            "[DynamicContext] Intent detected — DichVu: {D}, BangGia: {B}, ThongBao: {T}, BaoTri: {M}, KhaoSat: {S}",
            hasDichVuIntent, hasBangGiaIntent, hasThongBaoIntent, hasBaoTriIntent, hasKhaoSatIntent);

        // Không có intent nào → trả về rỗng
        if (!hasDichVuIntent && !hasBangGiaIntent && !hasThongBaoIntent && !hasBaoTriIntent && !hasKhaoSatIntent)
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

        // ─── Lịch bảo trì hạ tầng/thiết bị (30 ngày tới) ──────────────
        if (hasBaoTriIntent)
        {
            var baoTriContext = await GetBaoTriContextAsync(connection, now, cancellationToken);
            if (!string.IsNullOrEmpty(baoTriContext))
                contextParts.Add(baoTriContext);
        }

        // ─── Các chương trình khảo sát/biểu quyết cư dân ───────────────
        if (hasKhaoSatIntent)
        {
            var khaoSatContext = await GetKhaoSatContextAsync(connection, now, cancellationToken);
            if (!string.IsNullOrEmpty(khaoSatContext))
                contextParts.Add(khaoSatContext);
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
                SELECT bg.Id, dv.TenDichVu, bg.TenBangGia, bg.LoaiDinhGiaId AS Discriminator,
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

            var bangGias = (await connection.QueryAsync<BangGiaRow>(
                new CommandDefinition(bangGiaSql, new { Now = now }, cancellationToken: cancellationToken))).ToList();

            if (bangGias.Any())
            {
                sb.AppendLine();
                sb.AppendLine("## Bảng giá hiện hành:");

                var luyTienIds = bangGias.Where(b => b.Discriminator == 2).Select(b => b.Id).ToList();
                var loaiCanHoIds = bangGias.Where(b => b.Discriminator == 6).Select(b => b.Id).ToList();
                var khungGioIds = bangGias.Where(b => b.Discriminator == 7).Select(b => b.Id).ToList();

                var luyTienDetails = new List<ChiTietLuyTienRow>();
                if (luyTienIds.Any())
                {
                    const string sql = """
                        SELECT BangGiaId, TuMuc, DenMuc, DonGia AS SoTien
                        FROM ChiTietGiaLuyTien
                        WHERE BangGiaId IN @Ids
                        ORDER BY BangGiaId, TuMuc
                        """;
                    luyTienDetails = (await connection.QueryAsync<ChiTietLuyTienRow>(
                        new CommandDefinition(sql, new { Ids = luyTienIds }, cancellationToken: cancellationToken))).ToList();
                }

                var loaiCanHoDetails = new List<ChiTietLoaiCanHoRow>();
                if (loaiCanHoIds.Any())
                {
                    const string sql = """
                        SELECT ct.BangGiaId, ct.LoaiCanHoId, ct.DonGia AS SoTien
                        FROM ChiTietGiaLoaiCanHo ct
                        WHERE ct.BangGiaId IN @Ids
                        ORDER BY ct.BangGiaId, ct.LoaiCanHoId
                        """;
                    loaiCanHoDetails = (await connection.QueryAsync<ChiTietLoaiCanHoRow>(
                        new CommandDefinition(sql, new { Ids = loaiCanHoIds }, cancellationToken: cancellationToken))).ToList();
                }

                var khungGioDetails = new List<ChiTietKhungGioRow>();
                if (khungGioIds.Any())
                {
                    const string sql = """
                        SELECT ct.BangGiaId, ct.DonGia AS SoTien, kg.TenKhungGio, kg.GioBatDau, kg.GioKetThuc, kg.NgayTrongTuan
                        FROM ChiTietGiaKhungGio ct
                        INNER JOIN KhungGioDichVu kg ON ct.KhungGioId = kg.Id
                        WHERE ct.BangGiaId IN @Ids
                        ORDER BY ct.BangGiaId, kg.GioBatDau
                        """;
                    khungGioDetails = (await connection.QueryAsync<ChiTietKhungGioRow>(
                        new CommandDefinition(sql, new { Ids = khungGioIds }, cancellationToken: cancellationToken))).ToList();
                }

                foreach (var bg in bangGias)
                {
                    if (bg.Discriminator == 1 && bg.DonGia.HasValue) // Cố định
                    {
                        sb.AppendLine($"- **{bg.TenDichVu}**: {bg.DonGia.Value:N0} VNĐ ({bg.TenBangGia})");
                    }
                    else if (bg.Discriminator == 2) // Lũy tiến
                    {
                        sb.AppendLine($"- **{bg.TenDichVu}** ({bg.TenBangGia}) — Tính giá lũy tiến theo mức tiêu thụ:");
                        var details = luyTienDetails.Where(d => d.BangGiaId == bg.Id).ToList();
                        if (details.Any())
                        {
                            foreach (var d in details)
                            {
                                var limit = d.DenMuc.HasValue ? $"đến {d.DenMuc.Value:N0}" : "trở lên";
                                sb.AppendLine($"  * Từ {d.TuMuc:N0} {limit}: {d.SoTien:N0} VNĐ/đơn vị");
                            }
                        }
                        else
                        {
                            sb.AppendLine("  * (Chưa cấu hình chi tiết bậc thang lũy tiến)");
                        }
                    }
                    else if (bg.Discriminator == 6) // Theo diện tích / Loại căn hộ
                    {
                        sb.AppendLine($"- **{bg.TenDichVu}** ({bg.TenBangGia}) — Tính giá theo loại căn hộ:");
                        var details = loaiCanHoDetails.Where(d => d.BangGiaId == bg.Id).ToList();
                        if (details.Any())
                        {
                            foreach (var d in details)
                            {
                                sb.AppendLine($"  * Loại {GetLoaiCanHoName(d.LoaiCanHoId)}: {d.SoTien:N0} VNĐ/m²");
                            }
                        }
                        else
                        {
                            sb.AppendLine("  * (Chưa cấu hình chi tiết giá theo loại căn hộ)");
                        }
                    }
                    else if (bg.Discriminator == 7) // Theo khung giờ
                    {
                        sb.AppendLine($"- **{bg.TenDichVu}** ({bg.TenBangGia}) — Tính giá theo khung giờ:");
                        var details = khungGioDetails.Where(d => d.BangGiaId == bg.Id).ToList();
                        if (details.Any())
                        {
                            foreach (var d in details)
                            {
                                var dayInfo = d.NgayTrongTuan.HasValue ? $" ({GetNgayTrongTuanName(d.NgayTrongTuan.Value)})" : "";
                                sb.AppendLine($"  * Khung giờ '{d.TenKhungGio}' ({d.GioBatDau:hh\\:mm} - {d.GioKetThuc:hh\\:mm}){dayInfo}: {d.SoTien:N0} VNĐ/đơn vị");
                            }
                        }
                        else
                        {
                            sb.AppendLine("  * (Chưa cấu hình chi tiết giá theo khung giờ)");
                        }
                    }
                    else
                    {
                        sb.AppendLine($"- **{bg.TenDichVu}**: Xem chi tiết bảng giá '{bg.TenBangGia}'");
                    }
                }
            }
        }

        return sb.ToString();
    }

    private static string GetLoaiCanHoName(int? id) => id switch
    {
        1 => "Standard",
        2 => "Studio",
        3 => "Penthouse",
        4 => "Shophouse",
        _ => "Khác"
    };

    private static string GetNgayTrongTuanName(int value) => value switch
    {
        0 => "Chủ Nhật",
        1 => "Thứ Hai",
        2 => "Thứ Ba",
        3 => "Thứ Tư",
        4 => "Thứ Năm",
        5 => "Thứ Sáu",
        6 => "Thứ Bảy",
        _ => "Mọi ngày"
    };

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

    private static async Task<string> GetBaoTriContextAsync(
        System.Data.IDbConnection connection,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var thirtyDaysLater = now.AddDays(30);

        const string sql = """
            SELECT tb.TenThietBi, tb.ViTri, hm.TenHangMuc, lbt.NgayBaoTriTiepTheo
            FROM LichBaoTri lbt
            INNER JOIN ThietBi tb ON lbt.ThietBiId = tb.Id AND tb.IsDeleted = 0
            INNER JOIN HangMucBaoTri hm ON lbt.HangMucBaoTriId = hm.Id AND hm.IsDeleted = 0
            WHERE lbt.IsActive = 1
              AND lbt.IsDeleted = 0
              AND lbt.NgayBaoTriTiepTheo >= @Now
              AND lbt.NgayBaoTriTiepTheo <= @ToDate
            ORDER BY lbt.NgayBaoTriTiepTheo
            """;

        var list = (await connection.QueryAsync<BaoTriRow>(
            new CommandDefinition(sql, new { Now = now, ToDate = thirtyDaysLater }, cancellationToken: cancellationToken))).ToList();

        if (!list.Any())
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("## Lịch bảo trì thiết bị và hạ tầng sắp tới (30 ngày tới):");
        foreach (var item in list)
        {
            var location = string.IsNullOrEmpty(item.ViTri) ? "" : $" tại {item.ViTri}";
            sb.AppendLine($"- **{item.TenThietBi}**{location} — Hạng mục: {item.TenHangMuc} ({item.NgayBaoTriTiepTheo:dd/MM/yyyy HH:mm})");
        }

        return sb.ToString();
    }

    private static async Task<string> GetKhaoSatContextAsync(
        System.Data.IDbConnection connection,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TieuDe, MoTa, NgayBatDau, NgayKetThuc
            FROM KhaoSat
            WHERE TrangThaiId = 2  -- TrangThaiKhaoSat.DangDienRa
              AND IsDeleted = 0
              AND NgayBatDau <= @Now
              AND NgayKetThuc >= @Now
            ORDER BY NgayKetThuc
            """;

        var list = (await connection.QueryAsync<KhaoSatRow>(
            new CommandDefinition(sql, new { Now = now }, cancellationToken: cancellationToken))).ToList();

        if (!list.Any())
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("## Các chương trình khảo sát & biểu quyết đang diễn ra:");
        foreach (var item in list)
        {
            sb.AppendLine($"- **{item.TieuDe}** (Hạn chót: {item.NgayKetThuc:dd/MM/yyyy HH:mm})");
            if (!string.IsNullOrEmpty(item.MoTa))
                sb.AppendLine($"  _{item.MoTa}_");
        }

        return sb.ToString();
    }

    // ─── Private DTOs for Dapper ─────────────────────────────────────────────

    private sealed record DichVuRow(string TenDichVu, string? MoTa, string DonViTinh, bool IsBatBuoc);
    private sealed record BangGiaRow(int Id, string TenDichVu, string TenBangGia, int Discriminator, decimal? DonGia);
    private sealed record ThongBaoRow(string TieuDe, string? NoiDung, DateTimeOffset CreatedAt);
    private sealed record ChiTietLuyTienRow(int BangGiaId, decimal TuMuc, decimal? DenMuc, decimal SoTien);
    private sealed record ChiTietLoaiCanHoRow(int BangGiaId, int? LoaiCanHoId, decimal SoTien);
    private sealed record ChiTietKhungGioRow(int BangGiaId, decimal SoTien, string TenKhungGio, TimeSpan GioBatDau, TimeSpan GioKetThuc, int? NgayTrongTuan);
    private sealed record BaoTriRow(string TenThietBi, string ViTri, string TenHangMuc, DateTimeOffset NgayBaoTriTiepTheo);
    private sealed record KhaoSatRow(string TieuDe, string MoTa, DateTimeOffset NgayBatDau, DateTimeOffset NgayKetThuc);
}
