using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.UpdatePhanAnh;

public class UpdatePhanAnhCommandValidator : AbstractValidator<UpdatePhanAnhCommand>
{
    public UpdatePhanAnhCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID phản ánh không được để trống.");

        When(x => !x.IsWithdraw, () =>
        {
            RuleFor(x => x.TieuDe)
                .MaximumLength(200).WithMessage("Tiêu đề không được dài quá 200 ký tự.")
                .Must(x => x == null || !string.IsNullOrWhiteSpace(x)).WithMessage("Tiêu đề không được chỉ chứa khoảng trắng nếu được cung cấp.");

            RuleFor(x => x.NoiDung)
                .MaximumLength(1000).WithMessage("Nội dung không được dài quá 1000 ký tự.")
                .Must(x => x == null || !string.IsNullOrWhiteSpace(x)).WithMessage("Nội dung không được chỉ chứa khoảng trắng nếu được cung cấp.");
        });
    }
}
