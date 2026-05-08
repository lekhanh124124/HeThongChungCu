using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.XacNhanHoanThanhPhanAnh;

public record XacNhanHoanThanhPhanAnhCommand : ICommand<PhanAnhResponse>
{
    public int PhanAnhId { get; init; }
    public string KetQua { get; init; } = "Đã hoàn thành xử lý phản ánh.";
}
