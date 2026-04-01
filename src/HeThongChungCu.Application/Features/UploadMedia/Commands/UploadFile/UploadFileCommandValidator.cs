using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.UploadMedia.Commands.UploadFile;

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(x => x.TargetContainer)
            .NotEmpty().WithMessage("Mục đích tải lên (Target Container/Category) không được để trống.");

        RuleForEach(x => x.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.FileName).NotEmpty().WithMessage("Tên tệp tin không được để trống.");
            file.RuleFor(f => f.Content).NotNull().WithMessage("Nội dung tệp tin không được để trống.");
            file.RuleFor(f => f.Size).GreaterThan(0).WithMessage("Tệp tin không được rỗng.");
        });

        RuleFor(x => x.Files)
            .Must(files =>
            {
                var duplicateNames = files
                    .GroupBy(f => f.FileName)
                    .Where(g => g.Count() > 1)
                    .ToList();
                return duplicateNames.Count == 0;
            })
            .WithMessage(FileErrors.DuplicateFileName.Description);
    }
}
