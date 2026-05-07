using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;

using HeThongChungCu.Application.Features.QLThanhToan.Queries.GetHoaDonById;

namespace HeThongChungCu.Application.Features.QLThanhToan.Commands.TaoPhienThanhToanOnline;

public class TaoPhienThanhToanOnlineCommandHandler : ICommandHandler<TaoPhienThanhToanOnlineCommand, TaoPhienThanhToanOnlineResponse>
{
    private readonly IHoaDonQueryRepository _hoaDonQueryRepository;
    private readonly IGiaoDichThanhToanCommandRepository _giaoDichCommandRepository;
    private readonly IPhienThanhToanCommandRepository _phienCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaoPhienThanhToanOnlineCommandHandler(
        IHoaDonQueryRepository hoaDonQueryRepository,
        IGiaoDichThanhToanCommandRepository giaoDichCommandRepository,
        IPhienThanhToanCommandRepository phienCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _hoaDonQueryRepository = hoaDonQueryRepository;
        _giaoDichCommandRepository = giaoDichCommandRepository;
        _phienCommandRepository = phienCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TaoPhienThanhToanOnlineResponse>> Handle(TaoPhienThanhToanOnlineCommand request, CancellationToken cancellationToken)
    {
        var hoaDon = await _hoaDonQueryRepository.GetByIdAsync(new GetHoaDonByIdSpecification(request.HoaDonId), cancellationToken);
        if (hoaDon is null)
            return Result.Failure<TaoPhienThanhToanOnlineResponse>(HoaDonErrors.NotFound);

        // Lấy danh sách chi tiết được chọn
        var detailById = hoaDon.ChiTietHoaDons.ToDictionary(x => x.Id);
        if (request.ChiTietHoaDonIds.Any(id => !detailById.ContainsKey(id)))
            return Result.Failure<TaoPhienThanhToanOnlineResponse>(HoaDonErrors.ChiTietNotFound);

        // Kiểm tra xem có mục nào đã thanh toán chưa
        var paidDetailIds = await _giaoDichCommandRepository.GetAllocatedChiTietHoaDonIdsAsync(request.ChiTietHoaDonIds, cancellationToken);
        if (request.ChiTietHoaDonIds.Intersect(paidDetailIds).Any())
            return Result.Failure<TaoPhienThanhToanOnlineResponse>(GiaoDichErrors.DetailAlreadyPaid);

        // Tính tổng tiền
        var totalAmount = request.ChiTietHoaDonIds.Sum(id => detailById[id].ThanhTien);
        
        // Tạo mã thanh toán duy nhất
        var maThanhToan = $"PAY{DateTime.Now.Ticks}";

        var phien = new PhienThanhToan(
            maThanhToan,
            request.HoaDonId,
            string.Join(",", request.ChiTietHoaDonIds),
            totalAmount,
            $"Thanh toán online cho hóa đơn {hoaDon.MaHoaDon}");

        await _phienCommandRepository.AddAsync(phien, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Sinh link VietQR (Mock Bank Info)
        // Format: https://img.vietqr.io/image/<BANK_ID>-<ACCOUNT_NO>-compact.png?amount=<AMOUNT>&addInfo=<DESCRIPTION>
        var vietQrUrl = $"https://img.vietqr.io/image/TCB-19039590589017-compact.png?amount={totalAmount:0}&addInfo={maThanhToan}&accountName=LE%20MINH%20KHANH";

        return new TaoPhienThanhToanOnlineResponse(
            maThanhToan,
            totalAmount,
            vietQrUrl);
    }
}
