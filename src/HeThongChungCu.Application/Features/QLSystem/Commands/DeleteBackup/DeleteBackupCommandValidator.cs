using FluentValidation;

namespace HeThongChungCu.Application.Features.QLSystem.Commands.DeleteBackup;

public class DeleteBackupCommandValidator : AbstractValidator<DeleteBackupCommand>
{
    public DeleteBackupCommandValidator()
    {
        RuleFor(x => x.FileId)
            .GreaterThan(0).WithMessage("ID tệp sao lưu không hợp lệ.");
    }
}
