using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class FileErrors
{
    public static readonly Error DuplicateFileName = new(
        "File.DuplicateFileName",
        "Trong một lượt tải lên không được có các tệp tin trùng tên nhau để đảm bảo việc ánh xạ dữ liệu chính xác.");
}
