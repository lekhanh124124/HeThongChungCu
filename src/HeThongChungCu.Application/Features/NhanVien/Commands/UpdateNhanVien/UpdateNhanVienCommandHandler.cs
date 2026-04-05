using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.NhanVien.DTOs;
using HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;
using System.Linq;

namespace HeThongChungCu.Application.Features.NhanVien.Commands.UpdateNhanVien;

public class UpdateNhanVienCommandHandler : ICommandHandler<UpdateNhanVienCommand, NhanVienResponse>
{
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly INhanVienQueryRepository _nhanVienQueryRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IDocumentReconciliationService _documentReconciliationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNhanVienCommandHandler(
        INhanVienCommandRepository nhanVienRepository,
        INhanVienQueryRepository nhanVienQueryRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        IDocumentReconciliationService documentReconciliationService,
        IUnitOfWork unitOfWork)
    {
        _nhanVienRepository = nhanVienRepository;
        _nhanVienQueryRepository = nhanVienQueryRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _documentReconciliationService = documentReconciliationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NhanVienResponse>> Handle(UpdateNhanVienCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Staff and linked User Profile with documents
        var nhanVien = await _nhanVienRepository.GetByIdAsync(request.Id, cancellationToken);
        if (nhanVien == null)
            return Result.Failure<NhanVienResponse>(NhanVienErrors.NotFoundById(request.Id));

        var nguoiDung = await _nguoiDungRepository.GetByIdWithDocumentsAsync(nhanVien.NguoiDungId, cancellationToken);
        if (nguoiDung == null)
            return Result.Failure<NhanVienResponse>(UserErrors.NotFoundById(nhanVien.NguoiDungId));

        // 2. Validate CCCD/Phone unique if changed
        if (!string.IsNullOrEmpty(request.CCCD) && request.CCCD != nguoiDung.CCCD)
        {
            var exists = await _nguoiDungRepository.AnyAsync(u => u.CCCD == request.CCCD, cancellationToken);
            if (exists)
                return Result.Failure<NhanVienResponse>(UserErrors.IdCardAlreadyExists);
        }

        if (!string.IsNullOrEmpty(request.SoDienThoai) && request.SoDienThoai != nguoiDung.SoDienThoai)
        {
            var exists = await _nguoiDungRepository.AnyAsync(u => u.SoDienThoai.Value == request.SoDienThoai, cancellationToken);
            if (exists)
                return Result.Failure<NhanVienResponse>(UserErrors.PhoneNumberAlreadyExists);
        }

        // 3. Update User Profile
        var gioiTinh = GioiTinh.FromValue(request.GioiTinhId);
        if (gioiTinh == null)
            return Result.Failure<NhanVienResponse>(UserErrors.InvalidGender(GioiTinh.GetAll().Select(g => g.Name)));

        nguoiDung.UpdateProfile(
            request.Ten,
            request.Ho,
            request.NgaySinh,
            gioiTinh,
            request.DiaChi,
            request.CCCD,
            request.SoDienThoai);

        // 4. Document Reconciliation Logic via Domain Service
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

        // 5. Update Staff Details
        var loaiNhanVien = LoaiNhanVien.FromValue(request.LoaiNhanVienId);
        if (loaiNhanVien == null)
            return Result.Failure<NhanVienResponse>(NhanVienErrors.LoaiNhanVienInvalid(LoaiNhanVien.GetAll().Select(l => l.Name)));

        var trangThai = TrangThaiNhanVien.FromValue(request.TrangThaiNhanVienId);
        if (trangThai == null)
            return Result.Failure<NhanVienResponse>(Error.InvalidType("Trạng thái nhân viên", TrangThaiNhanVien.GetAll().Select(t => t.Name)));

        nhanVien.UpdateProfile(loaiNhanVien, request.NgayVaoLam, request.GhiChu);
        nhanVien.CapNhatTrangThai(trangThai, DateTime.Now);

        // 6. Atomic Save
        _nguoiDungRepository.Update(nguoiDung);
        await _nhanVienRepository.UpdateAsync(nhanVien, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Return full response
        var response = await _nhanVienQueryRepository.GetByIdAsync(new GetNhanVienByIdSpecification(nhanVien.Id), cancellationToken);
        
        return response != null 
            ? Result.Success(response) 
            : Result.Failure<NhanVienResponse>(NhanVienErrors.NotFound);
    }
}
