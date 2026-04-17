using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.BoSungNhanSuDoiTac;

public class BoSungNhanSuDoiTacCommandValidator : AbstractValidator<BoSungNhanSuDoiTacCommand>
{
    public BoSungNhanSuDoiTacCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.NhanSu)
            .NotEmpty().WithMessage("Danh sách nhân sự bổ sung không được để trống.");

        RuleForEach(x => x.NhanSu).ChildRules(ns =>
        {
            ns.RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage("Họ tên nhân sự không được để trống.");

            ns.RuleFor(x => x.SoCCCD)
                .NotEmpty().WithMessage("Số CCCD nhân sự không được để trống.");
        });
    }
}
