namespace HeThongChungCu.Application.Features.Profile.Commands.UpdateAvatar;

public class UpdateAvatarCommandValidator : AbstractValidator<UpdateAvatarCommand>
{
    private static readonly string[] AllowedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

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
            .WithMessage("Chỉ cho phép các định dạng file .jpg, .jpeg, .png, .webp.");

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
        return AllowedExtensions.Contains(extension);
    }
}
