using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLSystem.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLSystem.Commands.CreateBackup;

public class CreateBackupCommandHandler : ICommandHandler<CreateBackupCommand, BackupHistoryResponse>
{
    private readonly IBackupService _backupService;
    private readonly IZipService _zipService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITepTaiLieuCommandRepository _tepRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateBackupCommandHandler(
        IBackupService backupService,
        IZipService zipService,
        IFileStorageService fileStorageService,
        ITepTaiLieuCommandRepository tepRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _backupService = backupService;
        _zipService = zipService;
        _fileStorageService = fileStorageService;
        _tepRepository = tepRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BackupHistoryResponse>> Handle(CreateBackupCommand request, CancellationToken cancellationToken)
    {
        var timeStamp = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
        var zipFileName = $"Backup_ChungCu_{timeStamp}.zip";
        var adminEmail = _currentUserService.UserEmail ?? "system@chungcu.com";

        // 1. Lấy dữ liệu nghiệp vụ thô dạng byte array từ Infrastructure BackupService
        var dbDataResult = await _backupService.ExportBusinessDataAsync(adminEmail, cancellationToken);
        if (dbDataResult.IsFailure)
        {
            return Result.Failure<BackupHistoryResponse>(dbDataResult.Errors);
        }

        // 2. Nén các file JSON thành MemoryStream file Zip thông qua IZipService
        using var zipMemoryStream = await _zipService.CreateZipAsync(dbDataResult.Value, cancellationToken);
        var fileSize = zipMemoryStream.Length;

        // 3. Upload tệp nén Zip từ bộ nhớ RAM trực tiếp lên Azure Blob
        var uploadResult = await _fileStorageService.UploadFileAsync(
            zipMemoryStream,
            zipFileName,
            FileCategory.Backup,
            "application/x-zip-compressed",
            cancellationToken);

        if (uploadResult.IsFailure)
        {
            return Result.Failure<BackupHistoryResponse>(uploadResult.Errors);
        }

        var azureUrl = uploadResult.Value;

        // 4. Ghi nhận lịch sử sao lưu bằng thực thể TepSaoLuuDb
        var tepTaiLieu = new TepSaoLuuDb(
            zipFileName,
            azureUrl,
            fileSize,
            "application/x-zip-compressed");
        
        tepTaiLieu.MarkAsUsed(); 

        await _tepRepository.AddAsync(tepTaiLieu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BackupHistoryResponse
        {
            FileId = tepTaiLieu.Id,
            FileName = tepTaiLieu.FileName,
            FileUrl = tepTaiLieu.FileUrl,
            Size = tepTaiLieu.Size,
            CreatedAt = tepTaiLieu.CreatedAt,
            ContentType = tepTaiLieu.ContentType
        };
    }
}
