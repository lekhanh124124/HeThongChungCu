using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;

namespace HeThongChungCu.Application.Features.QLTaiChinh.EventHandlers;

/// <summary>
/// Handles HoaDonDoiTacPaidEvent to automatically log expenses (Chi) into the Operating Fund.
/// </summary>
public class AutoRecordExpenseOnPartnerPaymentHandler : INotificationHandler<HoaDonDoiTacPaidEvent>
{
    private readonly IQuyThuChiCommandRepository _thuChiRepository;
    private readonly IHoaDonDoiTacQueryRepository _hoaDonDoiTacQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AutoRecordExpenseOnPartnerPaymentHandler(
        IQuyThuChiCommandRepository thuChiRepository,
        IHoaDonDoiTacQueryRepository hoaDonDoiTacQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _thuChiRepository = thuChiRepository;
        _hoaDonDoiTacQueryRepository = hoaDonDoiTacQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(HoaDonDoiTacPaidEvent notification, CancellationToken cancellationToken)
    {
        var hd = notification.HoaDonDoiTac;

        var (soHopDong, tenDoiTac) = await _hoaDonDoiTacQueryRepository.GetHoaDonDoiTacInfoAsync(hd.Id, cancellationToken);

        string maGiaoDich = $"CHI-AUTO-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var cashLogResult = QuyThuChi.CreateChi(
            maGiaoDich: maGiaoDich,
            ngayGiaoDich: DateTimeOffset.UtcNow,
            phuongThucThanhToan: PhuongThucThanhToan.ChuyenKhoan,
            nguoiGiaoDich: tenDoiTac,
            chungTuGoc: $"HĐ Đối tác: #{hd.Id}, Số hợp đồng: {soHopDong}"
        );

        if (cashLogResult.IsSuccess)
        {
            var quyChi = cashLogResult.Value;
            quyChi.AddChiTiet(
                soTien: hd.SoTien.SoTien,
                nhomThongKe: "Chi trả nhà cung cấp/đối tác",
                ghiChu: $"Tự động thanh toán hóa đơn đối tác {tenDoiTac} tháng {hd.Thang}/{hd.Nam} theo hợp đồng {soHopDong}.",
                dichVuId: null
            );

            await _thuChiRepository.AddAsync(quyChi, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
