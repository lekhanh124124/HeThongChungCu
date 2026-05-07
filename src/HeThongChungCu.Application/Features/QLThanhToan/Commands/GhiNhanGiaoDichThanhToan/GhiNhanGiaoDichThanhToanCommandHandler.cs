using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.GhiNhanGiaoDichThanhToan;

public class GhiNhanGiaoDichThanhToanCommandHandler : ICommandHandler<GhiNhanGiaoDichThanhToanCommand, List<int>>
{
    private readonly IHoaDonCommandRepository _hoaDonRepository;
    private readonly IGiaoDichThanhToanCommandRepository _giaoDichRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GhiNhanGiaoDichThanhToanCommandHandler(
        IHoaDonCommandRepository hoaDonRepository,
        IGiaoDichThanhToanCommandRepository giaoDichRepository,
        IUnitOfWork unitOfWork)
    {
        _hoaDonRepository = hoaDonRepository;
        _giaoDichRepository = giaoDichRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<int>>> Handle(GhiNhanGiaoDichThanhToanCommand request, CancellationToken cancellationToken)
    {
        var hoaDon = await _hoaDonRepository.GetByIdAsync(request.HoaDonId, cancellationToken);
        if (hoaDon == null)
            return HoaDonErrors.NotFound;

        if (hoaDon.TrangThaiHoaDonId == TrangThaiHoaDon.ChoDuyet ||
            hoaDon.TrangThaiHoaDonId == TrangThaiHoaDon.DaThanhToan ||
            hoaDon.TrangThaiHoaDonId == TrangThaiHoaDon.DaHuy)
        {
            return GiaoDichThanhToanErrors.HoaDonNotPayable;
        }

        var invoiceDetailIds = hoaDon.ChiTietHoaDons.Select(x => x.Id).ToHashSet();
        if (request.ChiTietHoaDonIds.Any(id => !invoiceDetailIds.Contains(id)))
            return GiaoDichThanhToanErrors.ChiTietHoaDonInvalid;

        var allocated = await _giaoDichRepository.GetAllocatedChiTietHoaDonIdsAsync(request.ChiTietHoaDonIds, cancellationToken);
        if (allocated.Count > 0)
            return GiaoDichThanhToanErrors.ChiTietHoaDonAlreadyPaid;

        var detailById = hoaDon.ChiTietHoaDons.ToDictionary(x => x.Id);
        var totalAmount = request.ChiTietHoaDonIds.Sum(id => detailById[id].ThanhTien);

        var paidBefore = await _giaoDichRepository.GetPaidAmountByHoaDonIdAsync(hoaDon.Id, cancellationToken);
        var paidAfter = paidBefore + totalAmount;

        if (paidAfter > hoaDon.TongTien)
            return GiaoDichThanhToanErrors.Overpaid;

        var phuongThuc = PhuongThucThanhToan.FromValue(request.PhuongThucThanhToanId, null);
        if (phuongThuc == null)
            return Result.Failure<List<int>>(new Error("GiaoDichThanhToan.InvalidMethod", "Phương thức thanh toán không hợp lệ."));

        var giaoDichs = new List<GiaoDichThanhToan>();
        foreach (var detailId in request.ChiTietHoaDonIds)
        {
            var detail = detailById[detailId];
            var giaoDichResult = GiaoDichThanhToan.RecordTransaction(
                chiTietHoaDonId: detailId,
                soTien: detail.ThanhTien,
                phuongThucThanhToanId: phuongThuc,
                maGiaoDich: request.MaGiaoDich,
                ghiChu: request.GhiChu);

            if (giaoDichResult.IsFailure)
                return giaoDichResult.Errors;

            giaoDichs.Add(giaoDichResult.Value);
        }

        await _giaoDichRepository.AddRangeAsync(giaoDichs, cancellationToken);

        if (paidAfter == 0)
            hoaDon.UpdateStatus(TrangThaiHoaDon.ChuaThanhToan);
        else if (paidAfter < hoaDon.TongTien)
            hoaDon.UpdateStatus(TrangThaiHoaDon.ThanhToanMotPhan);
        else
            hoaDon.UpdateStatus(TrangThaiHoaDon.DaThanhToan);

        _hoaDonRepository.Update(hoaDon);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(giaoDichs.Select(x => x.Id).ToList());
    }
}
