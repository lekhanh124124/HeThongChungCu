using FluentValidation;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Commands.HuyPhanAnh;

public class HuyPhanAnhCommandValidator : AbstractValidator<HuyPhanAnhCommand>
{
    public HuyPhanAnhCommandValidator()
    {
        RuleFor(x => x.PhanAnhId)
            .NotEmpty().WithMessage("PhanAnhId không được để trống.");

        RuleFor(x => x.LyDoHuy)
            .NotEmpty().WithMessage("Lý do hủy không được để trống.")
            .MaximumLength(500).WithMessage("Lý do hủy không được dài quá 500 ký tự.");
    }
}
