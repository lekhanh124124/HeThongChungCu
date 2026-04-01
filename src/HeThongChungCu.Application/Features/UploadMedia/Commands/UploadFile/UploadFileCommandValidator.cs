using FluentValidation;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.UploadMedia.Commands.UploadFile;

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(x => x.TargetContainer)
            .NotEmpty().WithMessage(FileErrors.EmptyTargetContainer.Description);

        RuleForEach(x => x.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.FileName).NotEmpty().WithMessage(FileErrors.EmptyFileName.Description);
            file.RuleFor(f => f.Content).NotNull().WithMessage(FileErrors.EmptyContent.Description);
            file.RuleFor(f => f.Size).GreaterThan(0).WithMessage(FileErrors.InvalidSize.Description);
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
