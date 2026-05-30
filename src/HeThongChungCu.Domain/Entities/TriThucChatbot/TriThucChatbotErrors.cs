using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Entities;

public static class TriThucChatbotErrors
{
    public static readonly Error NotFound = new(
        "TriThucChatbot.NotFound",
        "Không tìm thấy mục tri thức chatbot với ID đã cung cấp.");

    public static readonly Error TieuDeRequired = new(
        "TriThucChatbot.TieuDeRequired",
        "Tiêu đề mục tri thức không được để trống.");

    public static readonly Error NoiDungRequired = new(
        "TriThucChatbot.NoiDungRequired",
        "Nội dung mục tri thức không được để trống.");

    public static readonly Error DanhMucRequired = new(
        "TriThucChatbot.DanhMucRequired",
        "Danh mục mục tri thức không được để trống.");

    public static readonly Error InvalidFileFormat = new(
        "TriThucChatbot.InvalidFileFormat",
        "File không hợp lệ. Chỉ hỗ trợ định dạng Markdown (.md).");

    public static readonly Error NoSectionsFound = new(
        "TriThucChatbot.NoSectionsFound",
        "Không tìm thấy section nào trong file. Đảm bảo file có ít nhất một heading H2 (##).");

    public static readonly Error EmptyFile = new(
        "TriThucChatbot.EmptyFile",
        "File rỗng hoặc không có nội dung hợp lệ.");

    public static readonly Error CannotDeleteActive = new(
        "TriThucChatbot.CannotDeleteActive",
        "Không thể xóa mục tri thức đang active. Vui lòng deactivate trước khi xóa.");
}
