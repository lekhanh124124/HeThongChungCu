using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreatePhieuBaoTri;

public class CreatePhieuBaoTriCommandHandler : ICommandHandler<CreatePhieuBaoTriCommand, PhieuBaoTriDetailResponse>
{
    private readonly IPhieuBaoTriCommandRepository _phieuBaoTriRepository;
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePhieuBaoTriCommandHandler(
        IPhieuBaoTriCommandRepository phieuBaoTriRepository,
        IThietBiCommandRepository thietBiRepository,
        IUnitOfWork unitOfWork)
    {
        _phieuBaoTriRepository = phieuBaoTriRepository;
        _thietBiRepository = thietBiRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhieuBaoTriDetailResponse>> Handle(CreatePhieuBaoTriCommand request, CancellationToken cancellationToken)
    {
        var maExists = await _phieuBaoTriRepository.MaPhieuExistsAsync(request.MaPhieu, cancellationToken);
        if (maExists)
            return BaoTriHaTangErrors.MaPhieuAlreadyExists;

        var thietBi = await _thietBiRepository.GetThietBiByIdAsync(request.ThietBiId, cancellationToken);
        if (thietBi == null)
            return BaoTriHaTangErrors.ThietBiNotFoundById(request.ThietBiId);

        var hangMuc = await _thietBiRepository.GetHangMucByIdAsync(request.HangMucBaoTriId, cancellationToken);
        if (hangMuc == null)
            return BaoTriHaTangErrors.HangMucNotFoundById(request.HangMucBaoTriId);

        var phieuBaoTri = PhieuBaoTri.Create(
            request.MaPhieu,
            request.ThietBiId,
            request.HangMucBaoTriId,
            null, // Thủ công không có lịch bảo trì
            DateTimeOffset.UtcNow, // NgayLapPhieu
            request.NgayDuKien,
            request.NoiDungChecklistBanDaus ?? []);

        if (request.HopDongDoiTacId.HasValue)
        {
            phieuBaoTri.AssignPartner(request.HopDongDoiTacId.Value);
        }

        if (request.NhanSus != null && request.NhanSus.Count > 0)
        {
            var staffs = request.NhanSus.Select(s => NhanSuBaoTri.Create(
                s.HoTen ?? string.Empty,
                s.SoCCCD ?? string.Empty,
                s.SoDienThoai,
                s.VaiTro,
                s.NhanVienId)).ToList();
            phieuBaoTri.AssignStaff(staffs);
        }

        await _phieuBaoTriRepository.AddPhieuBaoTriAsync(phieuBaoTri, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Lấy lại phiếu vừa tạo
        var savedPhieu = await _phieuBaoTriRepository.GetPhieuBaoTriByIdAsync(phieuBaoTri.Id, cancellationToken);

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
