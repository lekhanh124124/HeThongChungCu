using FluentValidation;

namespace HeThongChungCu.Application.Features.QLSystem.Commands.RestoreBackup;

public class RestoreBackupCommandValidator : AbstractValidator<RestoreBackupCommand>
{
    public RestoreBackupCommandValidator()
    {
        RuleFor(x => x.FileId)
            .GreaterThan(0).WithMessage("ID tệp sao lưu không hợp lệ.");
    }
}
