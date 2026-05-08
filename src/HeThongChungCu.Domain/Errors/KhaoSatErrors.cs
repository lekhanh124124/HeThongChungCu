using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Errors;

public static class KhaoSatErrors
{
    public static readonly Error NotFound = new(
        "KhaoSat.NotFound",
        "Không tìm thấy đợt khảo sát ý kiến hoặc bầu cử.");

    public static readonly Error InvalidDateRange = new(
        "KhaoSat.InvalidDateRange",
        "Thời gian kết thúc đợt khảo sát phải diễn ra sau thời gian bắt đầu.");

    public static readonly Error NoQuestions = new(
        "KhaoSat.NoQuestions",
        "Đợt khảo sát cần phải có ít nhất một câu hỏi trước khi chính thức công khai.");

    public static readonly Error NotDraftStatus = new(
        "KhaoSat.NotDraftStatus",
        "Chỉ có thể sửa đổi thông tin hoặc câu hỏi khi đợt khảo sát đang ở trạng thái mới tạo (Draft).");

    public static readonly Error AlreadyVoted = new(
        "KhaoSat.AlreadyVoted",
        "Căn hộ này đã thực hiện bỏ phiếu biểu quyết cho đợt khảo sát này rồi.");

    public static readonly Error InvalidOTP = new(
        "KhaoSat.InvalidOTP",
        "Mã xác thực OTP không hợp lệ, sai hoặc đã hết hiệu lực.");

    public static readonly Error NotEnoughOptions = new(
        "KhaoSat.NotEnoughOptions",
        "Mỗi câu hỏi khảo sát cần phải có ít nhất 2 đáp án hoặc phương án lựa chọn.");

    public static readonly Error InvalidStatus = new(
        "KhaoSat.InvalidStatus",
        "Trạng thái đợt khảo sát không hợp lệ để thực hiện thao tác này.");

    public static Error NotFoundById(int id) => new(
        "KhaoSat.NotFound",
        $"Không tìm thấy đợt khảo sát với ID '{id}'.");
}
