using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateHoaDonDoiTac;

public class CreateHoaDonDoiTacCommandHandler : ICommandHandler<CreateHoaDonDoiTacCommand, HoaDonDoiTacResponse>
{
    private readonly IHoaDonDoiTacCommandRepository _hoaDonDoiTacCommandRepository;
    private readonly IDoiTacCommandRepository _doiTacCommandRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHoaDonDoiTacCommandHandler(
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
        CreateHoaDonDoiTacCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Kiểm tra hợp đồng đối tác có tồn tại không
        var hopDong = await _doiTacCommandRepository.GetHopDongByIdAsync(request.HopDongDoiTacId, cancellationToken);
        if (hopDong == null)
        {
            return Result.Failure<HoaDonDoiTacResponse>(DoiTacErrors.HopDongNotFound);
        }

        // 2. Kiểm tra trạng thái hợp đồng (phải hoạt động)
        if (!hopDong.IsActive())
        {
            return Result.Failure<HoaDonDoiTacResponse>(DoiTacErrors.HopDongNotActive);
        }

        // 3. Kiểm tra chặn lập trùng hóa đơn cho cùng một kỳ (Tháng/Năm) của hợp đồng này
        var exists = await _hoaDonDoiTacCommandRepository.ExistsByKyAsync(
            request.HopDongDoiTacId,
            request.Thang,
            request.Nam,
            cancellationToken);

        if (exists)
        {
            return Result.Failure<HoaDonDoiTacResponse>(DoiTacErrors.HoaDonDuplicateKy);
        }

        // 4. Tìm kiếm thông tin đối tác để trả về đầy đủ DTO phản hồi
        var doiTac = await _doiTacCommandRepository.GetByIdAsync(hopDong.DoiTacId, cancellationToken);
        var tenDoiTac = doiTac?.TenDoiTac ?? string.Empty;

        // 5. Quản lý chứng từ đính kèm (MarkAsUsed)
        TepTaiLieu? fileHoaDon = null;
        if (request.FileHoaDonId.HasValue)
        {
            fileHoaDon = await _tepTaiLieuCommandRepository.GetByIdAsync(request.FileHoaDonId.Value, cancellationToken);
            if (fileHoaDon != null)
            {
                fileHoaDon.MarkAsUsed();
                _tepTaiLieuCommandRepository.Update(fileHoaDon);
            }
        }

        // 6. Lập hóa đơn đối tác mới
        var hoaDon = new HoaDonDoiTac(
            request.HopDongDoiTacId,
            request.Thang,
            request.Nam,
            request.SoTien,
            request.FileHoaDonId,
            request.GhiChu);

        await _hoaDonDoiTacCommandRepository.AddAsync(hoaDon, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Tạo kết quả trả về
        var result = new HoaDonDoiTacResponse
        {
            Id = hoaDon.Id,
            HopDongDoiTacId = hoaDon.HopDongDoiTacId,
            SoHopDong = hopDong.SoHopDong,
            DoiTacId = hopDong.DoiTacId,
            TenDoiTac = tenDoiTac,
            Thang = hoaDon.Thang,
            Nam = hoaDon.Nam,
            SoTien = hoaDon.SoTien.SoTien,
            NgayGhiNhan = hoaDon.NgayGhiNhan,
            GhiChu = hoaDon.GhiChu,
            TrangThaiThanhToanId = hoaDon.TrangThaiThanhToanId.Value,
            TrangThaiThanhToanTen = hoaDon.TrangThaiThanhToanId.Name,
            FileHoaDonId = hoaDon.FileHoaDonId,
            FileHoaDonUrl = fileHoaDon?.FileUrl,
            FileHoaDonName = fileHoaDon?.FileName
        };

        return Result.Success(result);
    }
}
