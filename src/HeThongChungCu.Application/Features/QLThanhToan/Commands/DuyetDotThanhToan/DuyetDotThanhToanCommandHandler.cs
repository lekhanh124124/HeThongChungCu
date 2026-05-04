using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.DuyetDotThanhToan;

public class DuyetDotThanhToanCommandHandler : ICommandHandler<DuyetDotThanhToanCommand, bool>
{
    private readonly IDotThanhToanCommandRepository _dotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DuyetDotThanhToanCommandHandler(
        IDotThanhToanCommandRepository dotRepository,
        IUnitOfWork unitOfWork)
    {
        _dotRepository = dotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DuyetDotThanhToanCommand request, CancellationToken cancellationToken)
    {
        var dots = await _dotRepository.GetByIdsAsync(request.Ids, cancellationToken);
        var foundIds = dots.Select(x => x.Id).ToList();
        var missingIds = request.Ids.Except(foundIds).ToList();

        if (missingIds.Count > 0)
            return DotThanhToanErrors.NotFoundByIds(missingIds);

        foreach (var dot in dots)
        {
            if (dot.TrangThaiDotThanhToanId != TrangThaiDotThanhToan.TaoMoi)
            {
                return DotThanhToanErrors.CannotApprove;
            }
            
            dot.MarkAsApproved();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
