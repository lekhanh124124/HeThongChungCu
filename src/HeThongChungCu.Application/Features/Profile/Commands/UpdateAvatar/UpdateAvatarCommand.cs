namespace HeThongChungCu.Application.Features.Profile.Commands.UpdateAvatar;

public record UpdateAvatarCommand(
    Stream AvatarStream,
    string FileName,
    string ContentType) : ICommand<string>;
