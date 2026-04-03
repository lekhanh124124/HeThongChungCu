namespace HeThongChungCu.Domain.Common;

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error NullValue = new(
        "Error.NullValue",
        "Giá trị là null.");

    public static Error FromException(Exception exception) => new(
        "Error.Exception",
        exception.Message);

    public static Error NotEmpty(string fieldName) => new(
        "Validation.NotEmpty",
        $"{fieldName} không được để trống.");

    public static Error MaxLength(string fieldName, int maxLength) => new(
        "Validation.MaxLength",
        $"{fieldName} không được vượt quá {maxLength} ký tự.");

    public static Error MinLength(string fieldName, int minLength) => new(
        "Validation.MinLength",
        $"{fieldName} phải có ít nhất {minLength} ký tự.");

    public static Error InvalidEmail(string fieldName) => new(
        "Validation.InvalidEmail",
        $"{fieldName} không đúng định dạng email.");

    public static Error Range(string fieldName, double min, double max) => new(
        "Validation.Range",
        $"Giá trị {fieldName} phải nằm trong khoảng từ {min} đến {max}.");

    public static Error DateInPast(string fieldName) => new(
        "Validation.DateInPast",
        $"{fieldName} không được là ngày trong quá khứ.");

    public static Error DateInFuture(string fieldName) => new(
        "Validation.DateInFuture",
        $"{fieldName} không được là ngày trong tương lai.");

    public static Error InvalidFormat(string fieldName) => new(
        "Validation.InvalidFormat",
        $"Định dạng {fieldName} không hợp lệ.");

    public static Error MustMatch(string fieldName, string targetFieldName) => new(
        "Validation.MustMatch",
        $"{fieldName} không khớp với {targetFieldName}.");

    public static Error InvalidDateRange(string startDateField, string endDateField) => new(
        "Validation.InvalidDateRange",
        $"{endDateField} phải sau {startDateField}.");

    public static Error NotFound(string entity) => new(
        $"{entity}.NotFound",
        $"Không tìm thấy {entity.ToLower()}.");

    public static Error NotFound(string entity, object id) => new(
        $"{entity}.NotFound",
        $"Không tìm thấy {entity.ToLower()} với ID '{id}'.");

    public static Error NotFound(string entity, string field, object value) => new(
        $"{entity}.NotFound",
        $"Không tìm thấy {entity.ToLower()} với {field.ToLower()} '{value}'.");

    public static Error AlreadyExists(string entity) => new(
        $"{entity}.AlreadyExists",
        $"{char.ToUpper(entity[0]) + entity.Substring(1).ToLower()} đã tồn tại.");

    public static Error AlreadyExists(string entity, string field, object value) => new(
        $"{entity}.AlreadyExists",
        $"Đã tồn tại {entity.ToLower()} với {field.ToLower()} '{value}'.");

    public static Error InvalidType(string type, IEnumerable<string> allowedValues) => new(
        "Validation.InvalidType",
        $"{char.ToUpper(type[0]) + type.Substring(1).ToLower()} không hợp lệ. Các giá trị hợp lệ: {string.Join(", ", allowedValues)}.");

    public static Error Forbidden(string action = "hành động này") => new(
        "Auth.Forbidden",
        $"Bạn không có quyền thực hiện {action.ToLower()}.");

    public static Error Conflict(string entity = "dữ liệu") => new(
        "Error.Conflict",
        $"Dữ liệu {entity.ToLower()} đã bị thay đổi, vui lòng thử lại.");

    public static Error Failure(string action = "thao tác") => new(
        "Error.Failure",
        $"{char.ToUpper(action[0]) + action.Substring(1).ToLower()} thất bại.");

    public static implicit operator string(Error error) => error.Code;

    public override string ToString() => Code;
}
