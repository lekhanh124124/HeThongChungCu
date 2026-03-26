using FluentValidation;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoMaDinhDanh;

public class TaoMaDinhDanhCommandValidator : AbstractValidator<TaoMaDinhDanhCommand>
{
    public TaoMaDinhDanhCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
