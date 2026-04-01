using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.PheDuyetYeuCauCuTru;

public class PheDuyetYeuCauCuTruCommandHandler : ICommandHandler<PheDuyetYeuCauCuTruCommand, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruCommandRepository _yeuCauRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly IQuanHeCuTruCommandRepository _quanHeCuTruRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public PheDuyetYeuCauCuTruCommandHandler(
        IYeuCauCuTruCommandRepository yeuCauRepository,
        INguoiDungCommandRepository userRepository,
        IQuanHeCuTruCommandRepository quanHeCuTruRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _userRepository = userRepository;
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(PheDuyetYeuCauCuTruCommand request, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId;
        if (adminId == null)
            return Result.Failure<YeuCauCuTruResponse>(UserErrors.NotFound);

        var yeuCau = await _yeuCauRepository.GetByIdAsync(request.YeuCauCuTruId, cancellationToken);
        if (yeuCau == null)
            return Result.Failure<YeuCauCuTruResponse>(YeuCauCuTruErrors.NotFound);

        var now = _dateTimeProvider.UtcNow;
        yeuCau.Approve(adminId.Value, now);

        // Logic Phê duyệt
        if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Them)
        {
            // Check if CCCD already exists
            if (!string.IsNullOrEmpty(yeuCau.YeuCauCCCD))
            {
                var cccdExists = await _userRepository.AnyAsync(u => u.CCCD == yeuCau.YeuCauCCCD, cancellationToken);
                if (cccdExists)
                {
                    return Result.Failure<YeuCauCuTruResponse>(UserErrors.IdCardAlreadyExists);
                }
            }

            // Check if SoDienThoai already exists
            if (!string.IsNullOrEmpty(yeuCau.YeuCauSoDienThoai))
            {
                var phoneExists = await _userRepository.AnyAsync(u => u.SoDienThoai == yeuCau.YeuCauSoDienThoai, cancellationToken);
                if (phoneExists)
                {
                    return Result.Failure<YeuCauCuTruResponse>(UserErrors.PhoneNumberAlreadyExists);
                }
            }

            // 1. Create User
            var newUser = new NguoiDung(
                yeuCau.YeuCauTen!,
                yeuCau.YeuCauHo!,
                yeuCau.YeuCauNgaySinh ?? DateTime.MinValue,
                GioiTinh.FromValue(yeuCau.YeuCauGioiTinhId ?? 1, null)!,
                yeuCau.YeuCauDiaChi,
                cccd: yeuCau.YeuCauCCCD,
                soDienThoai: yeuCau.YeuCauSoDienThoai);


            // 2. Add Documents if any
            foreach (var docReq in yeuCau.YeuCauTaiLieuCuTrus)
            {
                var newDoc = new TaiLieuNguoiDung(
                    null,
                    docReq.LoaiGiayToId,
                    docReq.SoGiayTo,
                    docReq.NgayPhatHanh,
                    docReq.Files);
                newUser.AddDocument(newDoc);
            }

            await _userRepository.AddAsync(newUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. Create Residency Relation
            var loaiQuanHe = LoaiQuanHeCuTru.FromValue(yeuCau.YeuCauLoaiQuanHeId!.Value, null);
            var existingRelations = await _quanHeCuTruRepository.GetByCanHoIdAsync(yeuCau.CanHoId, cancellationToken);
            var quanHe = new QuanHeCuTru(yeuCau.CanHoId, newUser.Id, loaiQuanHe!, now.DateTime, existingRelations);

            await _quanHeCuTruRepository.AddAsync(quanHe, cancellationToken);
        }
        else if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Sua)
        {
            var relation = await _quanHeCuTruRepository.GetByIdAsync(yeuCau.YeuCauQuanHeCuTruId!.Value, cancellationToken);
            if (relation == null)
                return Result.Failure<YeuCauCuTruResponse>(QuanHeCuTruErrors.NotFound);

            var user = await _userRepository.GetByIdWithDocumentsAsync(relation.NguoiDungId, cancellationToken);
            if (user == null)
                return Result.Failure<YeuCauCuTruResponse>(UserErrors.NotFound);

            if (yeuCau.YeuCauLoaiQuanHeId.HasValue)
            {
                relation.ThayDoiLoaiQuanHe(LoaiQuanHeCuTru.FromValue(yeuCau.YeuCauLoaiQuanHeId.Value, null)!);
                _quanHeCuTruRepository.Update(relation);
            }

            // --- Document Reconciliation Logic ---
            var currentDocs = user.TaiLieu.ToList();
            var proposedDocs = yeuCau.YeuCauTaiLieuCuTrus;

            // 1. Remove documents not in the request
            var proposedOriginalIds = proposedDocs.Where(d => d.TaiLieuCuTruId.HasValue)
                                                .Select(d => d.TaiLieuCuTruId!.Value)
                                                .ToList();

            foreach (var doc in currentDocs)
            {
                if (!proposedOriginalIds.Contains(doc.Id))
                {
                    user.RemoveDocument(doc.Id);
                }
            }

            // 2. Update existing or Add new
            foreach (var propDoc in proposedDocs)
            {
                if (propDoc.TaiLieuCuTruId.HasValue)
                {
                    // Update existing
                    var existingDoc = user.TaiLieu.FirstOrDefault(d => d.Id == propDoc.TaiLieuCuTruId.Value);
                    if (existingDoc != null)
                    {
                        existingDoc.UpdateInfo(propDoc.LoaiGiayToId, propDoc.SoGiayTo, propDoc.NgayPhatHanh);
                        existingDoc.SyncFiles(propDoc.Files);
                    }
                }
                else
                {
                    // Add new
                    var newDoc = new TaiLieuNguoiDung(
                        user.Id,
                        propDoc.LoaiGiayToId,
                        propDoc.SoGiayTo,
                        propDoc.NgayPhatHanh,
                        propDoc.Files);
                    user.AddDocument(newDoc);
                }
            }

            // Sync personal info if provided
            if (!string.IsNullOrEmpty(yeuCau.YeuCauTen) || !string.IsNullOrEmpty(yeuCau.YeuCauHo) || yeuCau.YeuCauNgaySinh.HasValue)
            {
                user.UpdateProfile(
                    yeuCau.YeuCauTen ?? user.Ten,
                    yeuCau.YeuCauHo ?? user.Ho,
                    yeuCau.YeuCauNgaySinh ?? user.NgaySinh,
                    yeuCau.YeuCauGioiTinhId.HasValue ? GioiTinh.FromValue(yeuCau.YeuCauGioiTinhId.Value, null)! : user.GioiTinhId,
                    yeuCau.YeuCauDiaChi ?? user.DiaChi,
                    yeuCau.YeuCauCCCD ?? user.CCCD,
                    yeuCau.YeuCauSoDienThoai ?? user.SoDienThoai);
            }
            _userRepository.Update(user);
        }
        else if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Xoa)
        {
            var relation = await _quanHeCuTruRepository.GetByIdAsync(yeuCau.YeuCauQuanHeCuTruId!.Value, cancellationToken);
            if (relation != null)
            {
                relation.KetThucCuTru(now.DateTime);
                _quanHeCuTruRepository.Update(relation);
            }
        }


        _yeuCauRepository.Update(yeuCau);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new YeuCauCuTruResponse
        {
            Id = yeuCau.Id,
            CanHoId = yeuCau.CanHoId,
            LoaiYeuCauId = yeuCau.LoaiYeuCauId.Value,
            TenLoaiYeuCau = yeuCau.LoaiYeuCauId.Name,
            TargetQuanHeCuTruId = yeuCau.YeuCauQuanHeCuTruId,
            YeuCauTen = yeuCau.YeuCauTen,
            YeuCauHo = yeuCau.YeuCauHo,
            YeuCauNgaySinh = yeuCau.YeuCauNgaySinh,
            YeuCauGioiTinhId = yeuCau.YeuCauGioiTinhId,
            YeuCauSoDienThoai = yeuCau.YeuCauSoDienThoai,
            YeuCauLoaiQuanHeId = yeuCau.YeuCauLoaiQuanHeId,
            NoiDung = yeuCau.NoiDung,
            LyDo = yeuCau.LyDo,
            TrangThaiId = yeuCau.TrangThaiId.Value,
            TenTrangThai = yeuCau.TrangThaiId.Name,
            CreatedAt = yeuCau.CreatedAt,
            NgayXuLy = yeuCau.NgayXuLy,
            NguoiXuLyId = yeuCau.NguoiXuLyId,
            Documents = yeuCau.YeuCauTaiLieuCuTrus.Select(d => new TaiLieuResponse
            {
                Id = d.Id,
                LoaiGiayToId = d.LoaiGiayToId.Value,
                TenLoaiGiayTo = d.LoaiGiayToId.Name,
                SoGiayTo = d.SoGiayTo,
                NgayPhatHanh = d.NgayPhatHanh,
                TargetTaiLieuCuTruId = d.TaiLieuCuTruId,
                Files = d.Files.Select(f => new TepTaiLieuResponse(f.Id, f.FileUrl, f.FileName, f.ContentType)).ToList()
            }).ToList(),
            YeuCauCCCD = yeuCau.YeuCauCCCD,
            YeuCauDiaChi = yeuCau.YeuCauDiaChi
        });
    }
}
