using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.Interfaces;

public interface IDocumentReconciliationService
{
    /// <summary>
    /// Đồng bộ hóa danh sách tài liệu của người dùng, thực hiện thêm/sửa/xóa 
    /// và tái sử dụng TepTaiLieu để tránh sinh lỗi duplicate records.
    /// </summary>
    void ReconcileNguoiDungDocuments(
        NguoiDung user,
        IEnumerable<DocumentSyncItem> proposedDocs,
        IEnumerable<TepTaiLieu> fetchedFiles);
}
