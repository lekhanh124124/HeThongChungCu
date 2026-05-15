using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;

namespace HeThongChungCu.Application.Features.QLTaiChinh.EventHandlers;

/// <summary>
/// Handles YeuCauThiCongDaCapPhepEvent and YeuCauThiCongHoanCocEvent to automatically manage deposits.
/// </summary>
public class AutoRecordDepositOnConstructionHandler :
    INotificationHandler<YeuCauThiCongDaCapPhepEvent>,
    INotificationHandler<YeuCauThiCongHoanCocEvent>
{
    private readonly IQuyThuChiCommandRepository _thuChiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AutoRecordDepositOnConstructionHandler(
        IQuyThuChiCommandRepository thuChiRepository,
        IUnitOfWork unitOfWork)
    {
        _thuChiRepository = thuChiRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(YeuCauThiCongDaCapPhepEvent notification, CancellationToken cancellationToken)
    {
        var tc = notification.YeuCauThiCong;

        // Collect deposit if any
        if (tc.TienDatCoc.HasValue && tc.TienDatCoc.Value > 0)
        {
            string maGiaoDich = $"THU-AUTO-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
            string nguoiGiaoDich = string.IsNullOrWhiteSpace(tc.NguoiDaiDien) ? "Cư dân" : tc.NguoiDaiDien;

            var cashLogResult = QuyThuChi.CreateThu(
                maGiaoDich: maGiaoDich,
                ngayGiaoDich: DateTimeOffset.UtcNow,
                phuongThucThanhToan: PhuongThucThanhToan.ChuyenKhoan,
                nguoiGiaoDich: nguoiGiaoDich,
                chungTuGoc: $"Yêu cầu thi công: #{tc.Id}, Hạng mục: {tc.HangMucThiCong}"
            );

            if (cashLogResult.IsSuccess)
            {
                var quyThu = cashLogResult.Value;
                quyThu.AddChiTiet(
                    soTien: tc.TienDatCoc.Value,
                    nhomThongKe: "Thu đặt cọc thi công",
                    ghiChu: $"Tự động thu tiền ký quỹ thi công hạng mục '{tc.HangMucThiCong}' của căn hộ #{tc.CanHoId}. {tc.GhiChuThuCoc}",
                    dichVuId: null
                );

                await _thuChiRepository.AddAsync(quyThu, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public async Task Handle(YeuCauThiCongHoanCocEvent notification, CancellationToken cancellationToken)
    {
        var tc = notification.YeuCauThiCong;

        // Refund deposit if any
        if (tc.TienThucHoan > 0)
        {
            string maGiaoDich = $"CHI-AUTO-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
            string nguoiGiaoDich = string.IsNullOrWhiteSpace(tc.NguoiDaiDien) ? "Cư dân" : tc.NguoiDaiDien;

            var cashLogResult = QuyThuChi.CreateChi(
                maGiaoDich: maGiaoDich,
                ngayGiaoDich: DateTimeOffset.UtcNow,
                phuongThucThanhToan: PhuongThucThanhToan.ChuyenKhoan,
                nguoiGiaoDich: nguoiGiaoDich,
                chungTuGoc: $"Yêu cầu thi công: #{tc.Id}, Hạng mục: {tc.HangMucThiCong}"
            );

            if (cashLogResult.IsSuccess)
            {
                var quyChi = cashLogResult.Value;
                quyChi.AddChiTiet(
                    soTien: tc.TienThucHoan,
                    nhomThongKe: "Chi hoàn cọc thi công",
                    ghiChu: $"Tự động hoàn tiền ký quỹ thi công (Khấu trừ: {tc.TienKhauTru ?? 0:N0}đ; Lý do khấu trừ: {tc.LyDoKhauTru ?? "Không"}).",
                    dichVuId: null
                );

                await _thuChiRepository.AddAsync(quyChi, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
