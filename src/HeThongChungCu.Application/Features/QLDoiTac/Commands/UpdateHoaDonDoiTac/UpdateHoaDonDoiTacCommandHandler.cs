using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.UpdateHoaDonDoiTac;

public class UpdateHoaDonDoiTacCommandHandler : ICommandHandler<UpdateHoaDonDoiTacCommand, HoaDonDoiTacResponse>
{
    private readonly IHoaDonDoiTacCommandRepository _hoaDonDoiTacCommandRepository;
    private readonly IDoiTacCommandRepository _doiTacCommandRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHoaDonDoiTacCommandHandler(
        IHoaDonDoiTacCommandRepository hoaDonDoiTacCommandRepository,
        IDoiTacCommandRepository doiTacCommandRepository,
        ITepTaiLieuCommandRepository tepTaiLieuCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _hoaDonDoiTacCommandRepository = hoaDonDoiTacCommandRepository;
        _doiTacCommandRepository = doiTacCommandRepository;
        _tepTaiLieuCommandRepository = tepTaiLieuCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HoaDonDoiTacResponse>> Handle(
        UpdateHoaDonDoiTacCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Tìm hóa đơn đối tác
        var hoaDon = await _hoaDonDoiTacCommandRepository.GetByIdAsync(request.Id, cancellationToken);
        if (hoaDon == null)
        {
            return Result.Failure<HoaDonDoiTacResponse>(DoiTacErrors.HoaDonNotFound);
        }

        // 2. Kiểm tra trạng thái hóa đơn (Chỉ được sửa đổi khi Chưa thanh toán)
        if (hoaDon.TrangThaiThanhToanId == TrangThaiThanhToanDoiTac.DaThanhToan)
        {
            return Result.Failure<HoaDonDoiTacResponse>(DoiTacErrors.HoaDonAlreadyPaid);
        }

        // 3. Kiểm tra chặn lập trùng hóa đơn cho cùng một kỳ (Tháng/Năm) của hợp đồng
        if (hoaDon.Thang != request.Thang || hoaDon.Nam != request.Nam)
        {
            var exists = await _hoaDonDoiTacCommandRepository.ExistsByKyAsync(
                hoaDon.HopDongDoiTacId,
                request.Thang,
                request.Nam,
                cancellationToken);

            if (exists)
            {
                return Result.Failure<HoaDonDoiTacResponse>(DoiTacErrors.HoaDonDuplicateKy);
            }
        }

        // 4. Lấy thông tin hợp đồng & đối tác để điền đầy đủ DTO trả về
        var hopDong = await _doiTacCommandRepository.GetHopDongByIdAsync(hoaDon.HopDongDoiTacId, cancellationToken);
        var soHopDong = hopDong?.SoHopDong ?? string.Empty;
        var doiTacId = hopDong?.DoiTacId ?? 0;
        var tenDoiTac = string.Empty;

        if (hopDong != null)
        {
            var doiTac = await _doiTacCommandRepository.GetByIdAsync(hopDong.DoiTacId, cancellationToken);
            tenDoiTac = doiTac?.TenDoiTac ?? string.Empty;
        }

        // 5. Quản lý tệp chứng từ đính kèm (MarkAsUsed / MarkAsUnused)
        TepTaiLieu? activeFile = hoaDon.FileHoaDon;
        if (hoaDon.FileHoaDonId != request.FileHoaDonId)
        {
            // Giải phóng tệp cũ
            if (hoaDon.FileHoaDonId.HasValue)
            {
                var oldFile = await _tepTaiLieuCommandRepository.GetByIdAsync(hoaDon.FileHoaDonId.Value, cancellationToken);
                if (oldFile != null)
                {
                    oldFile.MarkAsUnused();
                    _tepTaiLieuCommandRepository.Update(oldFile);
                }
            }

            // Kích hoạt tệp mới
            if (request.FileHoaDonId.HasValue)
            {
                activeFile = await _tepTaiLieuCommandRepository.GetByIdAsync(request.FileHoaDonId.Value, cancellationToken);
                if (activeFile != null)
                {
                    activeFile.MarkAsUsed();
                    _tepTaiLieuCommandRepository.Update(activeFile);
                }
            }
            else
            {
                activeFile = null;
            }
        }

        // 6. Cập nhật thông tin hóa đơn
        hoaDon.UpdateInfo(
            request.Thang,
            request.Nam,
            request.SoTien,
            request.FileHoaDonId,
            request.GhiChu);

        _hoaDonDoiTacCommandRepository.Update(hoaDon);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Tạo kết quả trả về
        var result = new HoaDonDoiTacResponse
        {
            Id = hoaDon.Id,
            HopDongDoiTacId = hoaDon.HopDongDoiTacId,
            SoHopDong = soHopDong,
            DoiTacId = doiTacId,
            TenDoiTac = tenDoiTac,
            Thang = hoaDon.Thang,
            Nam = hoaDon.Nam,
            SoTien = hoaDon.SoTien.SoTien,
            NgayGhiNhan = hoaDon.NgayGhiNhan,
            GhiChu = hoaDon.GhiChu,
            TrangThaiThanhToanId = hoaDon.TrangThaiThanhToanId.Value,
            TrangThaiThanhToanTen = hoaDon.TrangThaiThanhToanId.Name,
            FileHoaDonId = hoaDon.FileHoaDonId,
            FileHoaDonUrl = activeFile?.FileUrl,
            FileHoaDonName = activeFile?.FileName
        };

        return Result.Success(result);
    }
}
