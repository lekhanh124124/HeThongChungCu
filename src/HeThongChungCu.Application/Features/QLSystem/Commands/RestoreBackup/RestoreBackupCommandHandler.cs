using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLSystem.Commands.RestoreBackup;

public class RestoreBackupCommandHandler : ICommandHandler<RestoreBackupCommand, bool>
{
    private readonly ITepTaiLieuCommandRepository _tepRepository;
    private readonly IBackupService _backupService;
    private readonly IZipService _zipService;
    private readonly HttpClient _httpClient;
    private readonly IMaintenanceService _maintenanceService;

    public RestoreBackupCommandHandler(
        ITepTaiLieuCommandRepository tepRepository,
        IBackupService backupService,
        IZipService zipService,
        IMaintenanceService maintenanceService)
    {
        _tepRepository = tepRepository;
        _backupService = backupService;
        _zipService = zipService;
        _maintenanceService = maintenanceService;
        _httpClient = new HttpClient();
    }

    public async Task<Result<bool>> Handle(RestoreBackupCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm tệp sao lưu nghiệp vụ hợp lệ
        var tepBackup = await _tepRepository.GetByIdAsync(request.FileId, cancellationToken);
        if (tepBackup == null || tepBackup.LoaiTepId != LoaiTepTaiLieu.SaoLuuDb)
        {
            return Result.Failure<bool>(new Error("Restore.NotFound", "Không tìm thấy tệp tin sao lưu nghiệp vụ hợp lệ."));
        }

        try
        {
            // Kích hoạt chế độ bảo trì trước khi thực thi
            _maintenanceService.SetMaintenanceMode(true);

            // Giả lập delay 30 giây để Admin kiểm thử tác động của chế độ bảo trì từ các API khác
            // await Task.Delay(30000, cancellationToken);

            // 2. Tải tệp zip từ Azure Blob Storage về MemoryStream trong RAM (Không bọc try-catch)
            using var zipMemoryStream = new MemoryStream();
            using (var response = await _httpClient.GetAsync(tepBackup.FileUrl, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return Result.Failure<bool>(new Error("Restore.DownloadFailed", "Không thể tải tệp sao lưu từ Cloud về."));
                }
                await response.Content.CopyToAsync(zipMemoryStream, cancellationToken);
            }
            zipMemoryStream.Position = 0;

            // 3. Giải nén các file JSON trực tiếp trên RAM qua IZipService
            var extractedFiles = await _zipService.ExtractFilesAsync(zipMemoryStream, cancellationToken);

            try
            {
                // 4. Thực thi nạp dữ liệu động cấp thấp trong database sử dụng BackupService
                var restoreResult = await _backupService.ImportBusinessDataAsync(
                    extractedFiles.Select(f => (f.FileName, (Stream)f.Content)),
                    cancellationToken);

                if (restoreResult.IsFailure)
                {
                    return Result.Failure<bool>(restoreResult.Errors);
                }
            }
            finally
            {
                // Giải phóng tài nguyên các stream giải nén
                foreach (var file in extractedFiles)
                {
                    file.Content.Dispose();
                }
            }

            return true;
        }
        finally
        {
            // Tắt chế độ bảo trì sau khi hoàn tất hoặc thất bại
            _maintenanceService.SetMaintenanceMode(false);
        }
    }
}
