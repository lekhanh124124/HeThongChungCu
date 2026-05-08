using FluentValidation;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.PublishKhaoSat;

public class PublishKhaoSatCommandValidator : AbstractValidator<PublishKhaoSatCommand>
{
    public PublishKhaoSatCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID đợt khảo sát không hợp lệ.");
    }
}
