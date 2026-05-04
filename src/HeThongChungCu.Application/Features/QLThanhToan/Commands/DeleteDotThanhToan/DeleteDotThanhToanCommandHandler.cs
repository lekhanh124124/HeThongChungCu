using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.DeleteDotThanhToan;

public class DeleteDotThanhToanCommandHandler : ICommandHandler<DeleteDotThanhToanCommand, bool>
{
    private readonly IDotThanhToanCommandRepository _dotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDotThanhToanCommandHandler(
        IDotThanhToanCommandRepository dotRepository,
        IUnitOfWork unitOfWork)
    {
        _dotRepository = dotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteDotThanhToanCommand request, CancellationToken cancellationToken)
    {

        var dots = await _dotRepository.GetByIdsAsync(request.Ids, cancellationToken);
        var foundIds = dots.Select(x => x.Id).ToList();
        var missingIds = request.Ids.Except(foundIds).ToList();

        if (missingIds.Count > 0)
            return DotThanhToanErrors.NotFoundByIds(missingIds);

        // Chỉ cho phép xóa các đợt ở trạng thái Tạo mới
        var invalidDots = dots.Where(x => x.TrangThaiDotThanhToanId != TrangThaiDotThanhToan.TaoMoi).ToList();
        if (invalidDots.Count != 0)
        {
            var invalidNames = string.Join(", ", invalidDots.Select(x => x.TenDot));
            throw new BusinessException($"Không thể xóa các đợt thanh toán đã phát hành hoặc đã duyệt: {invalidNames}");
        }

        _dotRepository.DeleteRange(dots);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
