using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class FileCategory : BaseEnum<FileCategory, int>
{
    public static readonly FileCategory Avatar = new(1, "Ảnh đại diện", 2, [".jpg", ".jpeg", ".png"]);
    public static readonly FileCategory Document = new(2, "Tài liệu", 10, [".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg"]);
    public static readonly FileCategory Building = new(3, "Ảnh tòa nhà", 5, [".jpg", ".jpeg", ".png"]);
    public static readonly FileCategory Apartment = new(4, "Ảnh căn hộ", 5, [".jpg", ".jpeg", ".png"]);
    public static readonly FileCategory Vehicle = new(5, "Ảnh phương tiện", 5, [".jpg", ".jpeg", ".png"]);
    public static readonly FileCategory StaffDocument = new(6, "Tài liệu nhân viên", 10, [".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg"]);

    public int MaxSizeMB { get; }
    public string[] AllowedExtensions { get; }

    private FileCategory(int value, string name, int maxSizeMB, string[] allowedExtensions)
        : base(value, name)
    {
        MaxSizeMB = maxSizeMB;
        AllowedExtensions = allowedExtensions;
    }

    public static FileCategory? FromTargetContainer(string targetContainer)
    {
        return targetContainer.ToLower() switch
        {
            "anh-dai-dien-nguoi-dung" => Avatar,
            "hinh-anh-toa-nha" => Building,
            "hinh-anh-can-ho" => Apartment,
            "tai-lieu-cu-tru" => Document,
            "tai-lieu-phuong-tien" => Vehicle,
            "tai-lieu-nhan-vien" => StaffDocument,
            _ => Document
        };
    }
}
