using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.HuyHoaDon;

public class HuyHoaDonCommandHandler : ICommandHandler<HuyHoaDonCommand, bool>
{
    private readonly IHoaDonCommandRepository _hoaDonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public HuyHoaDonCommandHandler(IHoaDonCommandRepository hoaDonRepository, IUnitOfWork unitOfWork)
    {
        _hoaDonRepository = hoaDonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(HuyHoaDonCommand request, CancellationToken cancellationToken)
    {
        var hoaDon = await _hoaDonRepository.GetByIdAsync(request.HoaDonId, cancellationToken);
        if (hoaDon is null)
            return Result.Failure<bool>(HoaDonErrors.NotFound);

        var cancelResult = hoaDon.Cancel(request.LyDo);
        if (cancelResult.IsFailure)
            return Result.Failure<bool>(cancelResult.Errors);

        _hoaDonRepository.Update(hoaDon);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
