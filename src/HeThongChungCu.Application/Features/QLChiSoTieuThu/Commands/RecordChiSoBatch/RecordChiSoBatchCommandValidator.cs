using FluentValidation;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.RecordChiSoBatch;

public class RecordChiSoBatchCommandValidator : AbstractValidator<RecordChiSoBatchCommand>
{
    public RecordChiSoBatchCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Danh sách chỉ số không được để trống.");

        RuleFor(x => x.Thang)
            .InclusiveBetween(1, 12).WithMessage("Tháng phải từ 1 đến 12.");

        RuleFor(x => x.Nam)
            .GreaterThanOrEqualTo(2000).WithMessage("Năm không hợp lệ.");

        RuleFor(x => x.NgayGhiNhan)
            .NotEmpty().WithMessage("Ngày ghi nhận không được để trống.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.CanHoId)
                .GreaterThan(0).WithMessage("Căn hộ không hợp lệ.");

            item.RuleFor(x => x.DichVuId)
                .GreaterThan(0).WithMessage("Dịch vụ không hợp lệ.");

            item.RuleFor(x => x.ChiSoMoi)
                .GreaterThanOrEqualTo(x => x.ChiSoCu)
                .WithMessage("Chỉ số mới không được nhỏ hơn chỉ số cũ.");
        });
    }
}
