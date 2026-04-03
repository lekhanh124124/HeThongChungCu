using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.NhanVien.DTOs;
using HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using System.Linq;

namespace HeThongChungCu.Application.Features.NhanVien.Commands.UpdateNhanVien;

public class UpdateNhanVienCommandHandler : ICommandHandler<UpdateNhanVienCommand, NhanVienResponse>
{
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly INhanVienQueryRepository _nhanVienQueryRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNhanVienCommandHandler(
        INhanVienCommandRepository nhanVienRepository,
        INhanVienQueryRepository nhanVienQueryRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _nhanVienRepository = nhanVienRepository;
        _nhanVienQueryRepository = nhanVienQueryRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
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
            var exists = await _nguoiDungRepository.AnyAsync(u => u.SoDienThoai == request.SoDienThoai, cancellationToken);
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

        // 4. Document Reconciliation Logic
        if (request.TaiLieus != null)
        {
            var currentDocs = nguoiDung.TaiLieu.ToList();
            var proposedDocs = request.TaiLieus;

            // Remove documents not in the request
            var proposedOriginalIds = proposedDocs.Where(d => d.TaiLieuCuTruId.HasValue)
                                                .Select(d => d.TaiLieuCuTruId!.Value)
                                                .ToList();

            foreach (var doc in currentDocs)
            {
                if (!proposedOriginalIds.Contains(doc.Id))
                {
                    nguoiDung.RemoveDocument(doc.Id);
                }
            }

            // Sync proposed documents
            var allFileIds = proposedDocs.SelectMany(d => d.FileIds).Distinct().ToList();
            var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);
            var tepTaiLieuDict = tepTaiLieus.ToDictionary(f => f.Id);

            foreach (var propDoc in proposedDocs)
            {
                var files = propDoc.FileIds
                    .Where(id => tepTaiLieuDict.ContainsKey(id))
                    .Select(id => tepTaiLieuDict[id])
                    .ToList();

                if (propDoc.TaiLieuCuTruId.HasValue)
                {
                    // Update existing
                    var existingDoc = nguoiDung.TaiLieu.FirstOrDefault(d => d.Id == propDoc.TaiLieuCuTruId.Value);
                    if (existingDoc != null)
                    {
                        existingDoc.UpdateInfo(LoaiGiayTo.FromValue(propDoc.LoaiGiayToId)!, propDoc.SoGiayTo, propDoc.NgayPhatHanh);
                        existingDoc.SyncFiles(files);
                    }
                }
                else
                {
                    // Add new
                    var newDoc = new TaiLieuNguoiDung(
                        nguoiDung.Id,
                        LoaiGiayTo.FromValue(propDoc.LoaiGiayToId)!,
                        propDoc.SoGiayTo,
                        propDoc.NgayPhatHanh,
                        files);
                    nguoiDung.AddDocument(newDoc);
                }
            }
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
