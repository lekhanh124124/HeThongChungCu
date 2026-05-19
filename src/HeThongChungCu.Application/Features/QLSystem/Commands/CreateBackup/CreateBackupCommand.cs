using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLSystem.DTOs;

namespace HeThongChungCu.Application.Features.QLSystem.Commands.CreateBackup;

public record CreateBackupCommand : ICommand<BackupHistoryResponse>;
