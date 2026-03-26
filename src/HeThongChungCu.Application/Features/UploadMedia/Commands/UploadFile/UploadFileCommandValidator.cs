using FluentValidation;
using HeThongChungCu.Application.Common.Options;
using HeThongChungCu.Domain.Errors;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Application.Features.UploadMedia.Commands.UploadFile;

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator(IOptions<FileStorageOptions> options)
    {
        var settings = options.Value;

        // RuleFor(x => x.Files)
        //     .NotEmpty().WithMessage("Danh sách tệp tin không được để trống.");

        RuleForEach(x => x.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.FileName).NotEmpty().WithMessage("Tên tệp tin không được để trống.");
            file.RuleFor(f => f.Content).NotNull().WithMessage("Nội dung tệp tin không được để trống.");
            file.RuleFor(f => f.Size)
                .GreaterThan(0).WithMessage("Tệp tin không được rỗng.")
                .LessThanOrEqualTo(settings.MaxFileSizeInMB * 1024 * 1024)
                .WithMessage($"Dung lượng tệp tin không được vượt quá {settings.MaxFileSizeInMB}MB.");
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
