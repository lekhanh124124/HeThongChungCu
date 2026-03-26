using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLDichVu.Commands.TinhPhiDichVu;

public sealed record TinhPhiDichVuCommand(
    int CanHoId,
    int DichVuId,
    int Thang,
    int Nam) : ICommand<ChiTietPhiResponse>;

internal sealed class TinhPhiDichVuCommandHandler : ICommandHandler<TinhPhiDichVuCommand, ChiTietPhiResponse>
{
    private readonly IDangKyDichVuEFRepository _dangKyDichVuRepository;
    private readonly IDichVuEFRepository _dichVuRepository;
    private readonly IBangGiaEFRepository _bangGiaRepository;
    private readonly IChiSoTieuThuEFRepository _chiSoTieuThuRepository;
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TinhPhiDichVuCommandHandler(
        IDangKyDichVuEFRepository dangKyDichVuRepository,
        IDichVuEFRepository dichVuRepository,
        IBangGiaEFRepository bangGiaRepository,
        IChiSoTieuThuEFRepository chiSoTieuThuRepository,
        ICanHoEFRepository canHoRepository,
        IUnitOfWork unitOfWork)
    {
        _dangKyDichVuRepository = dangKyDichVuRepository;
        _dichVuRepository = dichVuRepository;
        _bangGiaRepository = bangGiaRepository;
        _chiSoTieuThuRepository = chiSoTieuThuRepository;
        _canHoRepository = canHoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChiTietPhiResponse>> Handle(TinhPhiDichVuCommand request, CancellationToken cancellationToken)
    {
        var registration = await _dangKyDichVuRepository.GetActiveAsync(request.CanHoId, request.DichVuId, cancellationToken);
        if (registration is null)
            return Result.Failure<ChiTietPhiResponse>(new Error("DichVu.NotRegistered", "Căn hộ chưa đăng ký dịch vụ này."));

        var dichVu = await _dichVuRepository.GetByIdAsync(request.DichVuId, cancellationToken);
        var date = new DateTime(request.Nam, request.Thang, 1);
        
        var bangGias = await _bangGiaRepository.GetByDichVuIdAsync(request.DichVuId, cancellationToken);
        var activeBangGia = bangGias.FirstOrDefault(p => p.IsOverlapping(date, date));
        
        if (activeBangGia is null)
            return Result.Failure<ChiTietPhiResponse>(new Error("BangGia.NotFound", "Không tìm thấy bảng giá áp dụng cho thời điểm này."));

        decimal thanhTien = 0;
        string ghiChu = "";
        double soLuongCalc = 0;

        if (activeBangGia.LoaiDinhGiaId == LoaiDinhGia.CoDinh)
        {
            thanhTien = activeBangGia.DonGia;
            soLuongCalc = 1;
            ghiChu = "Phí cố định";
        }
        else if (activeBangGia.LoaiDinhGiaId == LoaiDinhGia.TheoChiSo)
        {
            var chiSo = await _chiSoTieuThuRepository.GetByThangNamAsync(request.CanHoId, request.DichVuId, request.Thang, request.Nam, cancellationToken);
            if (chiSo is null)
                return Result.Failure<ChiTietPhiResponse>(new Error("ChiSoTieuThu.NotFound", "Chưa nhập chỉ số tiêu thụ cho tháng này."));
            
            soLuongCalc = chiSo.SoLuong;
            thanhTien = (decimal)soLuongCalc * activeBangGia.DonGia;
            ghiChu = $"Tiêu thụ: {soLuongCalc} {dichVu?.DonViTinh}";
        }
        else if (activeBangGia.LoaiDinhGiaId == LoaiDinhGia.LuyTien)
        {
            var chiSo = await _chiSoTieuThuRepository.GetByThangNamAsync(request.CanHoId, request.DichVuId, request.Thang, request.Nam, cancellationToken);
            if (chiSo is null)
                return Result.Failure<ChiTietPhiResponse>(new Error("ChiSoTieuThu.NotFound", "Chưa nhập chỉ số tiêu thụ cho tháng này."));

            double consumption = chiSo.SoLuong;
            soLuongCalc = consumption;
            
            foreach (var tier in activeBangGia.BangGiaLuyTiens.OrderBy(t => t.TuMuc))
            {
                double tierUsage = Math.Min(consumption, tier.DenMuc ?? double.MaxValue) - tier.TuMuc;
                if (tierUsage > 0)
                {
                    thanhTien += (decimal)tierUsage * tier.DonGia;
                }
                if (consumption <= (tier.DenMuc ?? double.MaxValue)) break;
            }
            ghiChu = $"Tính giá lũy tiến cho {consumption} {dichVu?.DonViTinh}";
        }
        else if (activeBangGia.LoaiDinhGiaId == LoaiDinhGia.TheoSoLuong)
        {
            soLuongCalc = registration.SoLuong;
            thanhTien = (decimal)soLuongCalc * activeBangGia.DonGia;
            ghiChu = $"Số lượng: {soLuongCalc}";
        }

        return new ChiTietPhiResponse(
            request.DichVuId,
            dichVu?.TenDichVu ?? "Dịch vụ",
            request.CanHoId,
            soLuongCalc,
            activeBangGia.DonGia,
            thanhTien,
            ghiChu);
    }
}
