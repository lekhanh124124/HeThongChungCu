using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLSystem.Commands.ProcessAutoBackup;

public class ProcessAutoBackupCommandHandler : ICommandHandler<ProcessAutoBackupCommand, bool>
{
    private readonly IBackupService _backupService;
    private readonly IZipService _zipService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITepTaiLieuCommandRepository _tepRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessAutoBackupCommandHandler(
        IBackupService backupService,
        IZipService zipService,
        IFileStorageService fileStorageService,
        ITepTaiLieuCommandRepository tepRepository,
        IUnitOfWork unitOfWork)
    {
        _backupService = backupService;
        _zipService = zipService;
        _fileStorageService = fileStorageService;
        _tepRepository = tepRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ProcessAutoBackupCommand request, CancellationToken cancellationToken)
    {
        var timeStamp = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
        var fileName = $"AutoBackup_ChungCu_{timeStamp}.zip";

        // 1. Trích xuất dữ liệu thô từ Database
        var dbDataResult = await _backupService.ExportBusinessDataAsync("system@chungcu.com", cancellationToken);
        if (dbDataResult.IsFailure)
        {
            return Result.Failure<bool>(dbDataResult.Errors);
        }

        // 2. Nén Zip in-memory qua IZipService
        using var zipMemoryStream = await _zipService.CreateZipAsync(dbDataResult.Value, cancellationToken);
        var fileSize = zipMemoryStream.Length;

        // 3. Upload file lên Azure Blob Storage trực tiếp từ RAM
        var uploadResult = await _fileStorageService.UploadFileAsync(
            zipMemoryStream,
            fileName,
            FileCategory.Backup,
            "application/x-zip-compressed",
            cancellationToken);

        if (uploadResult.IsFailure)
        {
            return Result.Failure<bool>(uploadResult.Errors);
        }

        // 4. Lưu vết thực thể TepSaoLuuDb tự động
        var tepTaiLieu = new TepSaoLuuDb(
            fileName,
            uploadResult.Value,
            fileSize,
            "application/x-zip-compressed");
        
        // Gán IsUsed = true
        tepTaiLieu.MarkAsUsed();

        await _tepRepository.AddAsync(tepTaiLieu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Áp dụng chính sách dọn dẹp các tệp tự động cũ hơn 15 ngày (Retention Policy)
        var thresholdDate = DateTime.Now.AddDays(-15);
        var oldBackups = (await _tepRepository.GetExpiredAutoBackupsAsync(thresholdDate, cancellationToken)).ToList();

        foreach (var backup in oldBackups)
        {
            // Xóa file trên Azure Blob Storage
            await _fileStorageService.DeleteFileAsync(backup.FileUrl, FileCategory.Backup, cancellationToken);
        }

        // Xóa bản ghi trong database
        if (oldBackups.Any())
        {
            _tepRepository.DeleteRange(oldBackups);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
