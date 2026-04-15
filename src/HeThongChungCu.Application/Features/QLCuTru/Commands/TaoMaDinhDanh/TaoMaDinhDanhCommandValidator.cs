using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoMaDinhDanh;

public class TaoMaDinhDanhCommandValidator : AbstractValidator<TaoMaDinhDanhCommand>
{
    public TaoMaDinhDanhCommandValidator()
    {
        RuleFor(x => x.QuanHeCuTruId).GreaterThan(0);
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không đúng định dạng email.");
    }
}
