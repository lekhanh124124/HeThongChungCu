using FluentValidation;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.RevokeHopDong;

public class RevokeHopDongCommandValidator : AbstractValidator<RevokeHopDongCommand>
{
    public RevokeHopDongCommandValidator()
    {
        RuleFor(x => x.DoiTacId)
            .GreaterThan(0).WithErrorCode("Validation.NotEmpty").WithMessage("ID không được để trống.");

        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID hợp đồng cần thu hồi không được để trống.")
            .Must(x => x.All(id => id > 0)).WithMessage("Id không hợp lệ.");
    }
}
