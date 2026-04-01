using FluentValidation;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

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
            .NotEmpty()
            .Must(HaveValidExtension)
            .WithMessage(x => FileErrors.InvalidType(
                Path.GetExtension(x.FileName), 
                FileCategory.Avatar.AllowedExtensions).Description);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(x => AllowedContentTypes.Contains(x))
            .WithMessage("Chỉ cho phép định dạng ảnh (image/jpeg, image/png, image/jpg, image/webp).");

        RuleFor(x => x.AvatarStream)
            .NotNull()
            .WithMessage("File không được để trống.");
    }

    private bool HaveValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return FileCategory.Avatar.AllowedExtensions.Contains(extension);
    }
}
