using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLSystem.Commands.DeleteBackup;

public record DeleteBackupCommand(int FileId) : ICommand<bool>;
