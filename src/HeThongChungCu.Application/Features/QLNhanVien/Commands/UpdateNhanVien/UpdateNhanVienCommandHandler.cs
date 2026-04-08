using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLNhanVien.DTOs;
using HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;
using System.Linq;

namespace HeThongChungCu.Application.Features.QLNhanVien.Commands.UpdateNhanVien;

public class UpdateNhanVienCommandHandler : ICommandHandler<UpdateNhanVienCommand, NhanVienDetailResponse>
{
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly INhanVienQueryRepository _nhanVienQueryRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ITaiKhoanCommandRepository _taiKhoanRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IDocumentReconciliationService _documentReconciliationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNhanVienCommandHandler(
        INhanVienCommandRepository nhanVienRepository,
        INhanVienQueryRepository nhanVienQueryRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        ITaiKhoanCommandRepository taiKhoanRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        IDocumentReconciliationService documentReconciliationService,
        IUnitOfWork unitOfWork)
    {
        _nhanVienRepository = nhanVienRepository;
        _nhanVienQueryRepository = nhanVienQueryRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _taiKhoanRepository = taiKhoanRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _documentReconciliationService = documentReconciliationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NhanVienDetailResponse>> Handle(UpdateNhanVienCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Staff and linked User Profile with documents
        var nhanVien = await _nhanVienRepository.GetByIdAsync(request.Id, cancellationToken);
        if (nhanVien == null)
            return Result.Failure<NhanVienDetailResponse>(NhanVienErrors.NotFoundById(request.Id));

        var nguoiDung = await _nguoiDungRepository.GetByIdWithDocumentsAsync(nhanVien.NguoiDungId, cancellationToken);
        if (nguoiDung == null)
            return Result.Failure<NhanVienDetailResponse>(UserErrors.NotFoundById(nhanVien.NguoiDungId));

        var taiKhoan = await _taiKhoanRepository.GetByNguoiDungIdAsync(nguoiDung.Id, cancellationToken);
        if (taiKhoan == null)
            return Result.Failure<NhanVienDetailResponse>(UserErrors.AccountNotFound);

        // 2. Validate CCCD/Phone unique if changed
        if (!string.IsNullOrEmpty(request.CCCD) && request.CCCD != nguoiDung.CCCD)
        {
            var exists = await _nguoiDungRepository.AnyAsync(u => u.CCCD == request.CCCD, cancellationToken);
            if (exists)
                return Result.Failure<NhanVienDetailResponse>(UserErrors.IdCardAlreadyExists);
        }

        if (!string.IsNullOrEmpty(request.SoDienThoai) && request.SoDienThoai != nguoiDung.SoDienThoai?.Value)
        {
            var exists = await _nguoiDungRepository.AnyAsync(u => u.SoDienThoai!.Value == request.SoDienThoai, cancellationToken);
            if (exists)
                return Result.Failure<NhanVienDetailResponse>(UserErrors.PhoneNumberAlreadyExists);
        }

        // 3. Update User Profile
        var gioiTinh = GioiTinh.FromValue(request.GioiTinhId);
        if (gioiTinh == null)
            return Result.Failure<NhanVienDetailResponse>(UserErrors.InvalidGender(GioiTinh.GetAll().Select(g => g.Name)));

        nguoiDung.UpdateProfile(
            request.Ten,
            request.Ho,
            request.NgaySinh,
            gioiTinh,
            request.DiaChi,
            request.CCCD,
            request.SoDienThoai != null ? new SoDienThoai(request.SoDienThoai) : null);

        // 4. Update Avatar (Account)
        if (request.AnhDaiDienId != taiKhoan.AnhDaiDienId)
        {
            taiKhoan.UpdateAvatar(request.AnhDaiDienId);
        }

        // 5. Document Reconciliation Logic via Domain Service
        if (request.TaiLieus != null)
        {
            var allFileIds = request.TaiLieus.SelectMany(d => d.FileIds).Distinct().ToList();
            var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);

            var proposedDocs = request.TaiLieus.Select(d => new DocumentSyncItem(
                d.TaiLieuCuTruId,
                d.LoaiGiayToId,
                d.SoGiayTo,
                d.NgayPhatHanh.HasValue ? new DateTimeOffset(d.NgayPhatHanh.Value, TimeSpan.Zero) : null,
                d.FileIds
            ));

            _documentReconciliationService.ReconcileNguoiDungDocuments(nguoiDung, proposedDocs, tepTaiLieus);
        }

        // 6. Update Staff Details
        var loaiNhanVien = LoaiNhanVien.FromValue(request.LoaiNhanVienId);
        if (loaiNhanVien == null)
            return Result.Failure<NhanVienDetailResponse>(NhanVienErrors.LoaiNhanVienInvalid(LoaiNhanVien.GetAll().Select(l => l.Name)));

        var trangThai = TrangThaiNhanVien.FromValue(request.TrangThaiNhanVienId);
        if (trangThai == null)
            return Result.Failure<NhanVienDetailResponse>(Error.InvalidType("Trạng thái nhân viên", TrangThaiNhanVien.GetAll().Select(t => t.Name)));

        nhanVien.UpdateProfile(loaiNhanVien, request.NgayVaoLam, request.GhiChu);
        nhanVien.CapNhatTrangThai(trangThai, DateTimeOffset.UtcNow);

        // 7. Atomic Save
        _nguoiDungRepository.Update(nguoiDung);
        _taiKhoanRepository.Update(taiKhoan);
        await _nhanVienRepository.UpdateAsync(nhanVien, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 8. Return full response
        var response = await _nhanVienQueryRepository.GetByIdAsync(new GetNhanVienByIdSpecification(nhanVien.Id), cancellationToken);

        return response != null
            ? Result.Success(response)
            : Result.Failure<NhanVienDetailResponse>(NhanVienErrors.NotFound);
    }
}
