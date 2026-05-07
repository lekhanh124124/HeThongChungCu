using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.XacNhanThanhToanOnline;

public class XacNhanThanhToanOnlineCommandHandler : ICommandHandler<XacNhanThanhToanOnlineCommand, bool>
{
    private readonly IPhienThanhToanCommandRepository _phienCommandRepository;
    private readonly IHoaDonQueryRepository _hoaDonQueryRepository;
    private readonly IHoaDonCommandRepository _hoaDonCommandRepository;
    private readonly IGiaoDichThanhToanCommandRepository _giaoDichCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public XacNhanThanhToanOnlineCommandHandler(
        IPhienThanhToanCommandRepository phienCommandRepository,
        IHoaDonQueryRepository hoaDonQueryRepository,
        IHoaDonCommandRepository hoaDonCommandRepository,
        IGiaoDichThanhToanCommandRepository giaoDichCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _phienCommandRepository = phienCommandRepository;
        _hoaDonQueryRepository = hoaDonQueryRepository;
        _hoaDonCommandRepository = hoaDonCommandRepository;
        _giaoDichCommandRepository = giaoDichCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(XacNhanThanhToanOnlineCommand request, CancellationToken cancellationToken)
    {
        var phien = await _phienCommandRepository.GetByMaThanhToanAsync(request.MaThanhToan, cancellationToken);
        if (phien is null)
            return Result.Failure<bool>(GiaoDichErrors.PhienThanhToanNotFound);

        if (phien.TrangThaiThanhToanId != TrangThaiThanhToan.ChoThanhToan.Value)
            return Result.Failure<bool>(GiaoDichErrors.PhienThanhToanInvalidStatus);

        var hoaDon = await _hoaDonQueryRepository.GetByIdAsync(new GetHoaDonByIdSpecification(phien.HoaDonId), cancellationToken);
        if (hoaDon is null)
            return Result.Failure<bool>(HoaDonErrors.NotFound);

        var detailIds = phien.ChiTietHoaDonIds.Split(',').Select(int.Parse).ToList();
        var detailById = hoaDon.ChiTietHoaDons.ToDictionary(x => x.Id);

        var transactions = new List<GiaoDichThanhToan>();
        foreach (var detailId in detailIds)
        {
            var detail = detailById[detailId];
            var giaoDichResult = GiaoDichThanhToan.RecordTransaction(
                chiTietHoaDonId: detailId,
                soTien: detail.ThanhTien,
                phuongThucThanhToanId: PhuongThucThanhToan.ChuyenKhoan,
                maGiaoDich: request.GiaoDichNganHangId ?? phien.MaThanhToan,
                ghiChu: $"Thanh toán online thành công qua phiên {phien.MaThanhToan}");

            if (giaoDichResult.IsFailure)
                return Result.Failure<bool>(giaoDichResult.Errors);

            transactions.Add(giaoDichResult.Value);
        }

        await _giaoDichCommandRepository.AddRangeAsync(transactions, cancellationToken);

        // Cập nhật trạng thái phiên
        phien.UpdateStatus(TrangThaiThanhToan.ThanhCong);
        _phienCommandRepository.Update(phien);

        // Cập nhật trạng thái hóa đơn
        var paidBefore = await _giaoDichCommandRepository.GetPaidAmountByHoaDonIdAsync(hoaDon.Id, cancellationToken);
        var currentPaid = transactions.Sum(x => x.SoTien);
        var totalPaid = paidBefore + currentPaid;

        var invoiceEntity = await _hoaDonCommandRepository.GetByIdAsync(hoaDon.Id, cancellationToken);
        if (invoiceEntity != null)
        {
            invoiceEntity.UpdateStatusByPaidAmount(totalPaid);
            _hoaDonCommandRepository.Update(invoiceEntity);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
