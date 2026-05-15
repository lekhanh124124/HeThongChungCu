using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Events;

namespace HeThongChungCu.Application.Features.QLTaiChinh.EventHandlers;

/// <summary>
/// Handles GiaoDichThanhToanRecordedEvent to automatically log incomes (Thu) into the Operating Fund.
/// </summary>
public class AutoRecordIncomeOnPaymentHandler : INotificationHandler<GiaoDichThanhToanRecordedEvent>
{
    private readonly IQuyThuChiCommandRepository _thuChiRepository;
    private readonly IHoaDonQueryRepository _hoaDonQueryRepository;
    private readonly IDichVuQueryRepository _dichVuQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AutoRecordIncomeOnPaymentHandler(
        IQuyThuChiCommandRepository thuChiRepository,
        IHoaDonQueryRepository hoaDonQueryRepository,
        IDichVuQueryRepository dichVuQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _thuChiRepository = thuChiRepository;
        _hoaDonQueryRepository = hoaDonQueryRepository;
        _dichVuQueryRepository = dichVuQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(GiaoDichThanhToanRecordedEvent notification, CancellationToken cancellationToken)
    {
        var gd = notification.GiaoDichThanhToan;

        var (tenMucPhi, loaiChiTietId, residentName, dbDichVuId) = await _hoaDonQueryRepository.GetChiTietHoaDonInfoAsync(gd.ChiTietHoaDonId, cancellationToken);

        int? dichVuId = dbDichVuId;
        if (dichVuId == null)
        {
            var loaiCT = LoaiChiTietHoaDon.FromValue(loaiChiTietId);
            if (loaiCT?.TuongUngLoaiDichVu != null)
            {
                dichVuId = await _dichVuQueryRepository.GetDichVuIdByLoaiAsync(loaiCT.TuongUngLoaiDichVu, cancellationToken);
            }
        }

        string maGiaoDich = string.IsNullOrWhiteSpace(gd.MaGiaoDich)
            ? $"THU-AUTO-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}"
            : gd.MaGiaoDich;

        var phuongThuc = gd.PhuongThucThanhToanId;

        var cashLogResult = QuyThuChi.CreateThu(
            maGiaoDich: maGiaoDich,
            ngayGiaoDich: gd.NgayGiaoDich,
            phuongThucThanhToan: phuongThuc,
            nguoiGiaoDich: residentName ?? "Cư dân",
            chungTuGoc: $"Mã GD: {gd.MaGiaoDich ?? "N/A"}, Chi tiết hóa đơn: #{gd.ChiTietHoaDonId}"
        );

        if (cashLogResult.IsSuccess)
        {
            var quyThu = cashLogResult.Value;
            quyThu.AddChiTiet(
                soTien: gd.SoTien,
                nhomThongKe: tenMucPhi,
                ghiChu: $"Tự động ghi nhận thu phí cho khoản: {tenMucPhi}. " + gd.GhiChu,
                dichVuId: dichVuId
            );

            await _thuChiRepository.AddAsync(quyThu, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
