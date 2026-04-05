using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoMaDinhDanh;

public class TaoMaDinhDanhCommandValidator : AbstractValidator<TaoMaDinhDanhCommand>
{
    public TaoMaDinhDanhCommandValidator()
    {
        RuleFor(x => x.QuanHeCuTruId).GreaterThan(0);
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(AuthErrors.EmailNotEmpty.Description)
            .EmailAddress().WithMessage(AuthErrors.EmailInvalid.Description);
    }
}
