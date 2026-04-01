namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class ValidationErrors
{
    public static readonly Error NotEmpty = new(
        "Validation.NotEmpty",
        "Trường này không được để trống.");

    public static Error MaxLength(int maxLength) => new(
        "Validation.MaxLength",
        $"Không được vượt quá {maxLength} ký tự.");

    public static Error MinLength(int minLength) => new(
        "Validation.MinLength",
        $"Phải có ít nhất {minLength} ký tự.");

    public static readonly Error InvalidEmail = new(
        "Validation.InvalidEmail",
        "Email không đúng định dạng.");

    public static Error Range(double min, double max) => new(
        "Validation.Range",
        $"Giá trị phải nằm trong khoảng từ {min} đến {max}.");

    public static readonly Error DateInPast = new(
        "Validation.DateInPast",
        "Ngày chọn không được ở quá khứ.");

    public static readonly Error DateInFuture = new(
        "Validation.DateInFuture",
        "Ngày chọn không được ở tương lai.");

    public static readonly Error InvalidDateRange = new(
        "Validation.InvalidDateRange",
        "Khoảng thời gian không hợp lệ.");

    public static Error InvalidFormat(string formatName) => new(
        "Validation.InvalidFormat",
        $"Định dạng {formatName} không hợp lệ.");
    
    public static readonly Error MustMatch = new(
        "Validation.MustMatch",
        "Giá trị xác nhận không khớp.");
}
