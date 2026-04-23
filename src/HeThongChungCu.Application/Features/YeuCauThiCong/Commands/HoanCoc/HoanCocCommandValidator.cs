using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.HoanCoc;

public class HoanCocCommandValidator : AbstractValidator<HoanCocCommand>
{
    public HoanCocCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
        RuleFor(x => x.TienKhauTru).GreaterThanOrEqualTo(0).WithMessage("Tiền khấu trừ phải lớn hơn hoặc bằng 0.");
        RuleFor(x => x.LyDo).MaximumLength(1000).WithMessage("Lý do tối đa 1000 ký tự.");
    }
}
