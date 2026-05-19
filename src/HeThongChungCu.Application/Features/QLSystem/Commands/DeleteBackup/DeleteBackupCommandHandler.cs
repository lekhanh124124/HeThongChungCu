using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLSystem.Commands.DeleteBackup;

public class DeleteBackupCommandHandler : ICommandHandler<DeleteBackupCommand, bool>
{
    private readonly ITepTaiLieuCommandRepository _tepRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBackupCommandHandler(
        ITepTaiLieuCommandRepository tepRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _tepRepository = tepRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteBackupCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra sự tồn tại của tệp sao lưu
        var tepBackup = await _tepRepository.GetByIdAsync(request.FileId, cancellationToken);
        if (tepBackup == null || tepBackup.LoaiTepId != LoaiTepTaiLieu.SaoLuuDb)
        {
            return Result.Failure<bool>(new Error("Delete.NotFound", "Không tìm thấy tệp tin sao lưu nghiệp vụ."));
        }

        // 2. Xóa file vật lý khỏi Azure Blob Storage
        await _fileStorageService.DeleteFileAsync(tepBackup.FileUrl, FileCategory.Backup, cancellationToken);

        // 3. Xóa bản ghi lịch sử trong Database
        _tepRepository.Delete(tepBackup);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
