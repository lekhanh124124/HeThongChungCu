using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;

public interface ITriThucChatbotCommandRepository
{
    /// <summary>Lấy mục tri thức theo ID (bao gồm soft-deleted nếu dùng IgnoreQueryFilters).</summary>
    Task<TriThucChatbot?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Lấy tất cả mục tri thức đang active, sắp xếp theo DanhMuc và ThuTuHienThi.</summary>
    Task<List<TriThucChatbot>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>Lấy danh sách mục tri thức theo danh mục.</summary>
    Task<List<TriThucChatbot>> GetByDanhMucAsync(string danhMuc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách bản ghi đang inactive nhưng đã từng được sync lên Qdrant (IsSynced = true).
    /// Dùng cho Sync command để biết vector nào cần xóa khỏi Qdrant.
    /// </summary>
    Task<List<TriThucChatbot>> GetSyncedInactiveAsync(CancellationToken cancellationToken = default);

    void Add(TriThucChatbot triThuc);
    void Update(TriThucChatbot triThuc);
    void Remove(TriThucChatbot triThuc);
}
