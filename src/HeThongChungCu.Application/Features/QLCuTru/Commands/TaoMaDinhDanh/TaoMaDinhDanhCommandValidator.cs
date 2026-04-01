using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoMaDinhDanh;

public class TaoMaDinhDanhCommandValidator : AbstractValidator<TaoMaDinhDanhCommand>
{
    public TaoMaDinhDanhCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage(ValidationErrors.Range(1, int.MaxValue).Description);
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .EmailAddress().WithMessage(ValidationErrors.InvalidFormat("Email").Description);
    }
}
