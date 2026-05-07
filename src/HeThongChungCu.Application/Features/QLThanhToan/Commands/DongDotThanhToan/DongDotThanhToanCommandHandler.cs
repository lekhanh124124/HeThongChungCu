using System.Runtime.InteropServices;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.DongDotThanhToan;

public class DongDotThanhToanCommandHandler : ICommandHandler<DongDotThanhToanCommand, bool>
{
    private readonly IDotThanhToanCommandRepository _dotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DongDotThanhToanCommandHandler(IDotThanhToanCommandRepository dotRepository, IUnitOfWork unitOfWork)
    {
        _dotRepository = dotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DongDotThanhToanCommand request, CancellationToken cancellationToken)
    {
        var dot = await _dotRepository.GetByIdAsync(request.DotThanhToanId, cancellationToken);
        if (dot is null)
            return Result.Failure<bool>(DotThanhToanErrors.NotFound);

        var closeResult = dot.MarkAsClosed();
        if (closeResult.IsFailure)
            return Result.Failure<bool>(closeResult.Errors);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
