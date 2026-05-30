using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.UpdateTriThucChatbot;

public class UpdateTriThucChatbotCommandValidator : AbstractValidator<UpdateTriThucChatbotCommand>
{
    public UpdateTriThucChatbotCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID không hợp lệ.");

        RuleFor(x => x.TieuDe)
            .NotEmpty().WithMessage(TriThucChatbotErrors.TieuDeRequired.Description)
            .MaximumLength(500).WithMessage("Tiêu đề không được vượt quá 500 ký tự.");

        RuleFor(x => x.NoiDung)
            .NotEmpty().WithMessage(TriThucChatbotErrors.NoiDungRequired.Description);

        RuleFor(x => x.DanhMuc)
            .NotEmpty().WithMessage(TriThucChatbotErrors.DanhMucRequired.Description)
            .MaximumLength(100).WithMessage("Danh mục không được vượt quá 100 ký tự.");

        RuleFor(x => x.ThuTuHienThi)
            .GreaterThanOrEqualTo(0).WithMessage("Thứ tự hiển thị phải >= 0.");
    }
}
