using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.GiaoViecDoiTac;

public class GiaoViecDoiTacCommandValidator : AbstractValidator<GiaoViecDoiTacCommand>
{
    public GiaoViecDoiTacCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID yêu cầu không được để trống.");

        RuleFor(x => x.HopDongDoiTacId)
            .NotEmpty().WithMessage("ID hợp đồng đối tác không được để trống.");

        RuleFor(x => x.NhanSu)
            .NotEmpty().WithMessage("Cần cung cấp ít nhất một nhân sự thực hiện.");

        RuleForEach(x => x.NhanSu).ChildRules(ns =>
        {
            ns.RuleFor(x => x.HoTen)
                .NotEmpty().WithMessage("Họ tên nhân sự không được để trống.");

            ns.RuleFor(x => x.SoCCCD)
                .NotEmpty().WithMessage("Số CCCD nhân sự không được để trống.");
        });
    }
}
