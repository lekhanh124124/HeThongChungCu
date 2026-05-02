using FluentValidation;
using HeThongChungCu.Application.Common.Interfaces.Services;
using System.IO;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.UploadChiSoImagesBatch;

public class UploadChiSoImagesBatchCommandValidator : AbstractValidator<UploadChiSoImagesBatchCommand>
{
    public UploadChiSoImagesBatchCommandValidator(IZipService zipService)
    {
        RuleFor(x => x.ZipStream)
            .NotNull().WithMessage("File zip trống.")
            .Must(stream => stream != null && stream.Length > 0).WithMessage("File zip trống.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("Tên file không được để trống.")
            .Must(fileName => Path.GetExtension(fileName).ToLowerInvariant() == ".zip")
            .WithMessage("Vui lòng upload file định dạng .zip.");

        RuleFor(x => x.ZipStream)
            .Must(stream => stream != null && stream.Length > 0 && zipService.IsValidZip(stream))
            .When(x => x.ZipStream != null && x.ZipStream.Length > 0 && Path.GetExtension(x.FileName).ToLowerInvariant() == ".zip")
            .WithMessage("File zip không hợp lệ hoặc bị hỏng.");
    }
}
