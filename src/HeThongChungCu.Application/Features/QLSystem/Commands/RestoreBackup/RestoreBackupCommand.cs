using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLSystem.Commands.RestoreBackup;

public record RestoreBackupCommand(int FileId) : ICommand<bool>;
