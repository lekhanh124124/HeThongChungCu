using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.CreatePhanAnh;

public class CreatePhanAnhCommandValidator : AbstractValidator<CreatePhanAnhCommand>
{
    public CreatePhanAnhCommandValidator()
    {
        RuleFor(x => x.CanHoId)
            .NotEmpty().WithMessage("CanHoId không được để trống.");

        RuleFor(x => x.TieuDe)
            .NotEmpty().WithMessage("Tiêu đề phản ánh không được để trống.")
            .MaximumLength(200).WithMessage("Tiêu đề không được dài quá 200 ký tự.");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage("Nội dung phản ánh không được để trống.")
            .MaximumLength(1000).WithMessage("Nội dung không được dài quá 1000 ký tự.");

        RuleFor(x => x.LoaiPhanAnhId)
            .NotEmpty().WithMessage("Loại phản ánh không được để trống.");
    }
}
