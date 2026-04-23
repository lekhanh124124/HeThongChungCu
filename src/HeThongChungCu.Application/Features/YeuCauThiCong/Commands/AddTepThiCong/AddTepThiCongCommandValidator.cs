using FluentValidation;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.AddTepThiCong;

public class AddTepThiCongCommandValidator : AbstractValidator<AddTepThiCongCommand>
{
    public AddTepThiCongCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id không được để trống.");
        RuleFor(x => x.TepIds).NotEmpty().WithMessage("Danh sách ID tệp không được để trống.");
    }
}
