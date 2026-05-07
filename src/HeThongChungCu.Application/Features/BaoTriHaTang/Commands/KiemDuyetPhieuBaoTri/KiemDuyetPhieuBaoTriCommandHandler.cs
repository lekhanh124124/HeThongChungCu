using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.KiemDuyetPhieuBaoTri;

public class KiemDuyetPhieuBaoTriCommandHandler : ICommandHandler<KiemDuyetPhieuBaoTriCommand, PhieuBaoTriDetailResponse>
{
    private readonly IPhieuBaoTriCommandRepository _phieuBaoTriRepository;
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly IUnitOfWork _unitOfWork;

    public KiemDuyetPhieuBaoTriCommandHandler(
        IPhieuBaoTriCommandRepository phieuBaoTriRepository,
        IThietBiCommandRepository thietBiRepository,
        ICurrentUserService currentUserService,
        INhanVienCommandRepository nhanVienRepository,
        IUnitOfWork unitOfWork)
    {
        _phieuBaoTriRepository = phieuBaoTriRepository;
        _thietBiRepository = thietBiRepository;
        _currentUserService = currentUserService;
        _nhanVienRepository = nhanVienRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhieuBaoTriDetailResponse>> Handle(KiemDuyetPhieuBaoTriCommand request, CancellationToken cancellationToken)
    {
        var phieu = await _phieuBaoTriRepository.GetPhieuBaoTriByIdAsync(request.Id, cancellationToken);
        if (phieu == null)
            return BaoTriHaTangErrors.PhieuBaoTriNotFoundById(request.Id);

        var thietBi = await _thietBiRepository.GetThietBiByIdAsync(phieu.ThietBiId, cancellationToken);
        if (thietBi == null)
            return BaoTriHaTangErrors.ThietBiNotFoundById(phieu.ThietBiId);

        var hangMuc = await _thietBiRepository.GetHangMucByIdAsync(phieu.HangMucBaoTriId, cancellationToken);
        if (hangMuc == null)
            return BaoTriHaTangErrors.HangMucNotFoundById(phieu.HangMucBaoTriId);

        if (request.IsDuyet)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
                return UserErrors.NotFound;

            var nhanVien = await _nhanVienRepository.GetByUserIdAsync(userId.Value, cancellationToken);
            if (nhanVien == null)
                return NhanVienErrors.NotFound;

            phieu.NghiemThu(nhanVien.Id, DateTimeOffset.UtcNow);
            thietBi.UpdateTrangThai(TrangThaiThietBi.HoatDongTot);
            _thietBiRepository.UpdateThietBi(thietBi);
        }
        else
        {
            phieu.TuChoi(request.GhiChuXuLy ?? "Không đạt nghiệm thu");
        }

        _phieuBaoTriRepository.UpdatePhieuBaoTri(phieu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load lai phieu
        var savedPhieu = await _phieuBaoTriRepository.GetPhieuBaoTriByIdAsync(phieu.Id, cancellationToken);

        return Result.Success(new PhieuBaoTriDetailResponse
        {
            Id = savedPhieu!.Id,
            MaPhieu = savedPhieu.MaPhieu,
            ThietBiId = savedPhieu.ThietBiId,
            TenThietBi = thietBi.TenThietBi,
            MaThietBi = thietBi.MaThietBi,
            HangMucBaoTriId = savedPhieu.HangMucBaoTriId,
            TenHangMuc = hangMuc.TenHangMuc,
            LichBaoTriId = savedPhieu.LichBaoTriId,
            HopDongDoiTacId = savedPhieu.HopDongDoiTacId,
            NgayLapPhieu = savedPhieu.NgayLapPhieu,
            NgayDuKien = savedPhieu.NgayDuKien,
            NgayThucTe = savedPhieu.NgayThucTe,
            ChiPhiThucTe = savedPhieu.ChiPhiThucTe,
            TrangThaiPhieuBaoTriId = savedPhieu.TrangThaiPhieuBaoTriId.Value,
            TenTrangThaiPhieuBaoTri = savedPhieu.TrangThaiPhieuBaoTriId.Name,
            GhiChuXuLy = savedPhieu.GhiChuXuLy,
            Checklists = savedPhieu.Checklists.Select(c => new PhieuBaoTriChecklistDto
            {
                Id = c.Id,
                NoiDungChecklist = c.NoiDungChecklist,
                DatYeuCau = c.DatYeuCau,
                GhiChuThucTe = c.GhiChuThucTe,
                AnhMinhHoaId = c.AnhMinhHoaId
            }).ToList(),
            VatTus = savedPhieu.VatTus.Select(v => new PhieuBaoTriVatTuDto
            {
                Id = v.Id,
                TenVatTu = v.TenVatTu,
                SoLuong = v.SoLuong,
                DonGia = v.DonGia,
                ThanhTien = v.ThanhTien
            }).ToList(),
            NhanSuBaoTris = savedPhieu.NhanSuBaoTris.Select(ns => new NhanSuBaoTriDto
            {
                Id = ns.Id,
                NhanVienId = ns.NhanVienId,
                HoTen = ns.HoTen,
                SoCCCD = ns.SoCCCD,
                SoDienThoai = ns.SoDienThoai,
                VaiTro = ns.VaiTro
            }).ToList()
        });
    }
}
