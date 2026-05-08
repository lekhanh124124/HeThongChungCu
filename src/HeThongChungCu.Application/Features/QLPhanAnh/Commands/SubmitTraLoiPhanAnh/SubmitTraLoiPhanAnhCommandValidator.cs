using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.SubmitTraLoiPhanAnh;

public class SubmitTraLoiPhanAnhCommandValidator : AbstractValidator<SubmitTraLoiPhanAnhCommand>
{
    public SubmitTraLoiPhanAnhCommandValidator()
    {
        RuleFor(x => x.PhanAnhId)
            .NotEmpty().WithMessage("PhanAnhId không được để trống.");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung phản hồi chat không được để trống.")
            .MaximumLength(1000).WithMessage("Nội dung phản hồi không được vượt quá 1000 ký tự.");
    }
}
