using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.XacNhanThanhToanDoiTac;

public class XacNhanThanhToanDoiTacCommandHandler : ICommandHandler<XacNhanThanhToanDoiTacCommand, bool>
{
    private readonly IHoaDonDoiTacCommandRepository _hoaDonDoiTacCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public XacNhanThanhToanDoiTacCommandHandler(
        IHoaDonDoiTacCommandRepository hoaDonDoiTacCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _hoaDonDoiTacCommandRepository = hoaDonDoiTacCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        XacNhanThanhToanDoiTacCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Tìm hóa đơn đối tác
        var hoaDon = await _hoaDonDoiTacCommandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (hoaDon == null)
        {
            return Result.Failure<bool>(DoiTacErrors.HoaDonNotFound);
        }

        // 2. Kiểm tra nếu đã thanh toán trước đó (Idempotent)
        if (hoaDon.TrangThaiThanhToanId == TrangThaiThanhToanDoiTac.DaThanhToan)
        {
            return Result.Success(true);
        }

        // 3. Xác nhận thanh toán hóa đơn
        hoaDon.UpdateStatus(TrangThaiThanhToanDoiTac.DaThanhToan);

        _hoaDonDoiTacCommandRepository.Update(hoaDon);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
