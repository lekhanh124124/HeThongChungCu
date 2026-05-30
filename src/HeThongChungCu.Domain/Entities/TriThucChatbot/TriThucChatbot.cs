using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

/// <summary>
/// Đại diện cho một mục tri thức tĩnh có thể chỉnh sửa của Chatbot.
/// Admin có thể CRUD nội dung này qua API, sau đó sync lên Qdrant vector store.
/// </summary>
public sealed class TriThucChatbot : AuditableEntity
{
    // ═══════════════════════════════════════════════════════════════
    // PROPERTIES
    // ═══════════════════════════════════════════════════════════════

    /// <summary>Tiêu đề / tên tài liệu tri thức.</summary>
    public string TieuDe { get; private set; } = string.Empty;

    /// <summary>Nội dung Markdown sẽ được chunk và embed vào Qdrant.</summary>
    public string NoiDung { get; private set; } = string.Empty;

    /// <summary>
    /// Phân loại tài liệu (ví dụ: faq, noi-quy, quy-trinh, dich-vu...).
    /// Tương ứng với <c>document_type</c> trong Qdrant payload.
    /// </summary>
    public string DanhMuc { get; private set; } = string.Empty;

    /// <summary>Thứ tự ưu tiên hiển thị (càng nhỏ càng ưu tiên cao).</summary>
    public int ThuTuHienThi { get; private set; }

    /// <summary>Bật/tắt mục tri thức này. Khi false sẽ bị xóa khỏi Qdrant ở lần sync tiếp theo.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Đánh dấu mục tri thức đã được đồng bộ lên Qdrant chưa.
    /// False = có thay đổi chưa sync (tạo mới, cập nhật, bật/tắt).
    /// True = trạng thái DB và Qdrant đang nhất quán.
    /// </summary>
    public bool IsSynced { get; private set; }

    /// <summary>Thời điểm đồng bộ lên Qdrant gần nhất.</summary>
    public DateTimeOffset? LastSyncedAt { get; private set; }

    // ═══════════════════════════════════════════════════════════════
    // CONSTRUCTORS
    // ═══════════════════════════════════════════════════════════════

    private TriThucChatbot() { } // EF Core

    private TriThucChatbot(
        string tieuDe,
        string noiDung,
        string danhMuc,
        int thuTuHienThi)
    {
        TieuDe = tieuDe;
        NoiDung = noiDung;
        DanhMuc = danhMuc;
        ThuTuHienThi = thuTuHienThi;
        IsActive = false; // Mặc định deactive — admin phải review và activate thủ công trước khi sync
        IsSynced = false;
    }

    // ═══════════════════════════════════════════════════════════════
    // FACTORY METHOD
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Tạo một mục tri thức mới cho chatbot.
    /// </summary>
    public static Result<TriThucChatbot> CreateTriThucChatbot(
        string tieuDe,
        string noiDung,
        string danhMuc,
        int thuTuHienThi = 0)
    {
        if (string.IsNullOrWhiteSpace(tieuDe))
            return Result.Failure<TriThucChatbot>(TriThucChatbotErrors.TieuDeRequired);

        if (string.IsNullOrWhiteSpace(noiDung))
            return Result.Failure<TriThucChatbot>(TriThucChatbotErrors.NoiDungRequired);

        if (string.IsNullOrWhiteSpace(danhMuc))
            return Result.Failure<TriThucChatbot>(TriThucChatbotErrors.DanhMucRequired);

        var triThuc = new TriThucChatbot(tieuDe, noiDung, danhMuc, thuTuHienThi);

        return Result.Success(triThuc);
    }

    // ═══════════════════════════════════════════════════════════════
    // DOMAIN METHODS
    // ═══════════════════════════════════════════════════════════════

    /// <summary>Cập nhật nội dung mục tri thức. Reset cờ sync vì nội dung thay đổi.</summary>
    public Result Update(
        string tieuDe,
        string noiDung,
        string danhMuc,
        int thuTuHienThi)
    {
        if (string.IsNullOrWhiteSpace(tieuDe))
            return Result.Failure(TriThucChatbotErrors.TieuDeRequired);

        if (string.IsNullOrWhiteSpace(noiDung))
            return Result.Failure(TriThucChatbotErrors.NoiDungRequired);

        if (string.IsNullOrWhiteSpace(danhMuc))
            return Result.Failure(TriThucChatbotErrors.DanhMucRequired);

        TieuDe = tieuDe;
        NoiDung = noiDung;
        DanhMuc = danhMuc;
        ThuTuHienThi = thuTuHienThi;

        ResetSync(); // nội dung thay đổi → Qdrant lỗi thời

        return Result.Success();
    }

    /// <summary>Kích hoạt mục tri thức. Reset cờ sync để lần sync tiếp sẽ upsert lại vào Qdrant.</summary>
    public void Activate()
    {
        IsActive = true;
        ResetSync();
    }

    /// <summary>Vô hiệu hóa mục tri thức. Reset cờ sync để lần sync tiếp sẽ xóa khỏi Qdrant.</summary>
    public void Deactivate()
    {
        IsActive = false;
        ResetSync();
    }

    /// <summary>
    /// Đánh dấu đã đồng bộ thành công lên Qdrant.
    /// Chỉ được gọi bởi SyncTriThucChatbotCommandHandler sau khi upsert/delete hoàn tất.
    /// </summary>
    public void MarkAsSynced(DateTimeOffset syncedAt)
    {
        IsSynced = true;
        LastSyncedAt = syncedAt;
    }

    /// <summary>
    /// Đánh dấu vector đã bị xóa khỏi Qdrant.
    /// Chỉ được gọi bởi SyncTriThucChatbotCommandHandler sau khi DeleteBySourceAsync hoàn tất.
    /// </summary>
    public void MarkAsUnsynced()
    {
        IsSynced = false;
        LastSyncedAt = null;
    }

    // ─── Private Helpers ────────────────────────────────────────────

    /// <summary>Reset cờ sync — gọi mỗi khi nội dung hoặc trạng thái thay đổi.</summary>
    private void ResetSync()
    {
        IsSynced = false;
        LastSyncedAt = null;
    }
}
