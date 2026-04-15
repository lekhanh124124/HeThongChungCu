using FluentValidation;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.Profile.Commands.UpdateAvatar;

public class UpdateAvatarCommandValidator : AbstractValidator<UpdateAvatarCommand>
{
    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg",
        "image/png",
        "image/jpg",
        "image/webp"
    };

    public UpdateAvatarCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("Tên tệp tin không được để trống.")
            .Must(HaveValidExtension)
            .WithMessage(x => $"Tệp tin '{Path.GetExtension(x.FileName)}' không hợp lệ. Chỉ chấp nhận: {string.Join(", ", FileCategory.Avatar.AllowedExtensions)}.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Loại nội dung không được để trống.")
            .Must(x => AllowedContentTypes.Contains(x))
            .WithMessage($"Chỉ cho phép định dạng: {string.Join(", ", AllowedContentTypes)}.");

        RuleFor(x => x.AvatarStream)
            .NotNull()
            .WithMessage("Nội dung tệp tin không được để trống.");
    }

    private bool HaveValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return FileCategory.Avatar.AllowedExtensions.Contains(extension);
    }
}

