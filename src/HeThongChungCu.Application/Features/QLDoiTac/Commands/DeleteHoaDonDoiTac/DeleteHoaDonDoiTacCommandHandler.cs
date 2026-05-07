using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.DeleteHoaDonDoiTac;

public class DeleteHoaDonDoiTacCommandHandler : ICommandHandler<DeleteHoaDonDoiTacCommand, bool>
{
    private readonly IHoaDonDoiTacCommandRepository _hoaDonDoiTacCommandRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHoaDonDoiTacCommandHandler(
        IHoaDonDoiTacCommandRepository hoaDonDoiTacCommandRepository,
        ITepTaiLieuCommandRepository tepTaiLieuCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _hoaDonDoiTacCommandRepository = hoaDonDoiTacCommandRepository;
        _tepTaiLieuCommandRepository = tepTaiLieuCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteHoaDonDoiTacCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Tìm hóa đơn đối tác
        var hoaDon = await _hoaDonDoiTacCommandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (hoaDon == null)
        {
            return Result.Failure<bool>(DoiTacErrors.HoaDonNotFound);
        }

        // 2. Chặn xóa khi đã thanh toán
        if (hoaDon.TrangThaiThanhToanId == TrangThaiThanhToanDoiTac.DaThanhToan)
        {
            return Result.Failure<bool>(DoiTacErrors.HoaDonAlreadyPaid);
        }

        // 3. Giải phóng tệp chứng từ đính kèm
        if (hoaDon.FileHoaDonId.HasValue)
        {
            var file = await _tepTaiLieuCommandRepository.GetByIdAsync(hoaDon.FileHoaDonId.Value, cancellationToken);
            if (file != null)
            {
                file.MarkAsUnused();
                _tepTaiLieuCommandRepository.Update(file);
            }
        }

        // 4. Xóa mềm hóa đơn đối tác
        _hoaDonDoiTacCommandRepository.Remove(hoaDon);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
