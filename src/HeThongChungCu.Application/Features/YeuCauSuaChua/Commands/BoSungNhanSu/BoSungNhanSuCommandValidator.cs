using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.BoSungNhanSu;

public class BoSungNhanSuCommandValidator : AbstractValidator<BoSungNhanSuCommand>
{
    public BoSungNhanSuCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.NhanSu)
            .NotEmpty().WithMessage("Danh sách nhân sự bổ sung không được để trống.");

        RuleForEach(x => x.NhanSu).ChildRules(ns =>
        {
            ns.RuleFor(x => x)
                .Must(dto => dto.NhanVienId.HasValue || (!string.IsNullOrEmpty(dto.HoTen) && !string.IsNullOrEmpty(dto.SoCCCD)))
                .WithMessage("Nhân sự phải có ID nhân viên (nội bộ) hoặc đầy đủ Họ tên và CCCD (đối tác).");

            ns.When(dto => dto.NhanVienId.HasValue, () =>
            {
                ns.RuleFor(dto => dto.NhanVienId)
                    .GreaterThan(0).WithMessage("ID nhân viên phải là số dương.");
            });
        });
    }
}
