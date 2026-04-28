using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class GiaoDichErrors
{
    public static readonly Error InvalidAmount = new(
        "GiaoDich.InvalidAmount",
        "Số tiền giao dịch phải lớn hơn 0.");
}
