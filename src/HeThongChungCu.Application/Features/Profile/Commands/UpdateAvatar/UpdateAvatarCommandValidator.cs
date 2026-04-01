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
            .NotEmpty().WithMessage(FileErrors.EmptyFileName.Description)
            .Must(HaveValidExtension)
            .WithMessage(x => FileErrors.InvalidType(
                Path.GetExtension(x.FileName), 
                FileCategory.Avatar.AllowedExtensions).Description);

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage(ValidationErrors.NotEmpty.Description)
            .Must(x => AllowedContentTypes.Contains(x))
            .WithMessage(FileErrors.InvalidContentType(AllowedContentTypes).Description);

        RuleFor(x => x.AvatarStream)
            .NotNull()
            .WithMessage(FileErrors.EmptyContent.Description);
    }

    private bool HaveValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return FileCategory.Avatar.AllowedExtensions.Contains(extension);
    }
}

