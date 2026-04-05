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

    /// <summary>
    /// Đồng bộ hóa bộ sưu tập hình ảnh của phương tiện.
    /// </summary>
    void ReconcilePhuongTienImages(
        PhuongTien phuongTien,
        IEnumerable<TepTaiLieu> hinhAnhs);
}
