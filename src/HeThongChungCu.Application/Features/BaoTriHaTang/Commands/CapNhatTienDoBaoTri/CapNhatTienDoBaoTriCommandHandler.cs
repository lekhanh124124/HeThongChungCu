using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CapNhatTienDoBaoTri;

public class CapNhatTienDoBaoTriCommandHandler : ICommandHandler<CapNhatTienDoBaoTriCommand, PhieuBaoTriDetailResponse>
{
    private readonly IPhieuBaoTriCommandRepository _phieuBaoTriRepository;
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatTienDoBaoTriCommandHandler(
        IPhieuBaoTriCommandRepository phieuBaoTriRepository,
        IThietBiCommandRepository thietBiRepository,
        IUnitOfWork unitOfWork)
    {
        _phieuBaoTriRepository = phieuBaoTriRepository;
        _thietBiRepository = thietBiRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PhieuBaoTriDetailResponse>> Handle(CapNhatTienDoBaoTriCommand request, CancellationToken cancellationToken)
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

        // Bắt đầu thực hiện nếu chưa bắt đầu
        if (phieu.TrangThaiPhieuBaoTriId == TrangThaiPhieuBaoTri.DaGiaoViec)
        {
            phieu.Start();
            thietBi.UpdateTrangThai(TrangThaiThietBi.DangBaoTri);
            _thietBiRepository.UpdateThietBi(thietBi);
        }

        // Tạo dữ liệu cập nhật checklist
        var checklistDict = request.Checklists.ToDictionary(
            x => x.ChecklistId,
            x => (x.IsDatYeuCau, x.GhiChuThucTe, x.AnhMinhHoaId));

        // Tạo danh sách vật tư sử dụng
        var materials = new List<PhieuBaoTriVatTu>();
        if (request.VatTus != null)
        {
            foreach (var input in request.VatTus)
            {
                materials.Add(PhieuBaoTriVatTu.Create(input.TenVatTu, input.SoLuong, input.DonGia));
            }
        }

        // Nộp kết quả báo cáo (Sẽ tự động chuyển sang Chờ nghiệm thu)
        phieu.SubmitResults(checklistDict, materials, 0, request.GhiChuXuLy);

        _phieuBaoTriRepository.UpdatePhieuBaoTri(phieu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load lại phiếu mới cập nhật đầy đủ các Id vừa phát sinh
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
