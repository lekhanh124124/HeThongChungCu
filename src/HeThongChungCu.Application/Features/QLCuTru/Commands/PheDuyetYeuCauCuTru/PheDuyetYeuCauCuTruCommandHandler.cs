using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.PheDuyetYeuCauCuTru;

public class PheDuyetYeuCauCuTruCommandHandler : ICommandHandler<PheDuyetYeuCauCuTruCommand, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruCommandRepository _yeuCauRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly IQuanHeCuTruCommandRepository _quanHeCuTruRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IDocumentReconciliationService _documentReconciliationService;
    private readonly IUnitOfWork _unitOfWork;

    public PheDuyetYeuCauCuTruCommandHandler(
        IYeuCauCuTruCommandRepository yeuCauRepository,
        INguoiDungCommandRepository userRepository,
        IQuanHeCuTruCommandRepository quanHeCuTruRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        ICanHoCommandRepository canHoRepository,
        IDocumentReconciliationService documentReconciliationService,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _userRepository = userRepository;
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _canHoRepository = canHoRepository;
        _documentReconciliationService = documentReconciliationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(PheDuyetYeuCauCuTruCommand request, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId;
        if (adminId == null)
            return UserErrors.NotFound;

        var yeuCau = await _yeuCauRepository.GetByIdAsync(request.YeuCauCuTruId, cancellationToken);
        if (yeuCau == null)
            return YeuCauCuTruErrors.NotFound;

        var canHo = await _canHoRepository.GetByIdAsync(yeuCau.CanHoId, cancellationToken);
        if (canHo == null)
            return CanHoErrors.NotFound;

        var now = _dateTimeProvider.Now;
        var approveResult = yeuCau.Approve(adminId.Value, now);
        if (approveResult.IsFailure)
            return approveResult.Errors;

        // Logic Phê duyệt
        if (yeuCau.LoaiHanhDongYeuCauId == LoaiHanhDongYeuCau.Them)
        {
            // Gather data for uniqueness check
            var cccdExists = !string.IsNullOrEmpty(yeuCau.YeuCauCCCD) && await _userRepository.AnyAsync(u => u.CCCD == yeuCau.YeuCauCCCD, cancellationToken);
            var phoneExists = !string.IsNullOrEmpty(yeuCau.YeuCauSoDienThoai) && await _userRepository.AnyAsync(u => u.SoDienThoai!.Value == yeuCau.YeuCauSoDienThoai, cancellationToken);

            // Logic moved from ResidencyService.CheckUniqueness
            if (cccdExists) return UserErrors.IdCardAlreadyExists;
            if (phoneExists) return UserErrors.PhoneNumberAlreadyExists;

            // 1. Create User (Logic moved from ResidencyService.CreateUserFromRequest)
            var newUser = new NguoiDung(
                yeuCau.YeuCauTen!,
                yeuCau.YeuCauHo!,
                yeuCau.YeuCauNgaySinh ?? DateTimeOffset.MinValue,
                GioiTinh.FromValue(yeuCau.YeuCauGioiTinhId ?? 1, null)!,
                yeuCau.YeuCauDiaChi.FullAddress,
                cccd: yeuCau.YeuCauCCCD,
                soDienThoai: yeuCau.YeuCauSoDienThoai);

            foreach (var docReq in yeuCau.YeuCauTaiLieuCuTrus)
            {
                var newDoc = new TaiLieuNguoiDung(
                    null,
                    docReq.LoaiGiayToId,
                    docReq.SoGiayTo,
                    docReq.NgayPhatHanh,
                    docReq.Files.Select(f => new TepTaiLieuNguoiDung(f.FileName, f.FileUrl, f.Size, f.ContentType)));
                newUser.AddDocument(newDoc);
            }

            await _userRepository.AddAsync(newUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 2. Create Residency Relation (Logic moved from ResidencyService.CreateRelation)
            var loaiQuanHe = LoaiQuanHeCuTru.FromValue(yeuCau.YeuCauLoaiQuanHeId!.Value, null);
            var existingRelations = (await _quanHeCuTruRepository.GetByCanHoIdAsync(yeuCau.CanHoId, cancellationToken)).ToList();

            if (existingRelations.Any(x =>
                    x.NguoiDungId == newUser.Id &&
                    x.CanHoId == yeuCau.CanHoId &&
                    x.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru))
            {
                return QuanHeCuTruErrors.UserAlreadyResident;
            }

            bool isHeadRole = loaiQuanHe == LoaiQuanHeCuTru.ChuHo || loaiQuanHe == LoaiQuanHeCuTru.NguoiThue;
            bool hasActiveHead = existingRelations.Any(x =>
                (x.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || x.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue) &&
                x.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);

            if (isHeadRole && hasActiveHead)
            {
                return QuanHeCuTruErrors.HouseholderAlreadyExists;
            }

            if (!isHeadRole && !hasActiveHead)
            {
                return QuanHeCuTruErrors.HouseholderNotFound;
            }

            var relation = new QuanHeCuTru(yeuCau.CanHoId, newUser.Id, loaiQuanHe!, now);
            await _quanHeCuTruRepository.AddAsync(relation, cancellationToken);

            // 3. Update Apartment Status
            existingRelations.Add(relation);
            if (existingRelations.Any(r => r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru))
            {
                canHo.MarkAsOccupied();
            }
            else
            {
                canHo.MarkAsVacant();
            }
            _canHoRepository.Update(canHo);
        }
        else if (yeuCau.LoaiHanhDongYeuCauId == LoaiHanhDongYeuCau.Sua)
        {
            var relation = await _quanHeCuTruRepository.GetByIdAsync(yeuCau.YeuCauQuanHeCuTruId!.Value, cancellationToken);
            if (relation == null)
                return QuanHeCuTruErrors.NotFound;

            var user = await _userRepository.GetByIdWithDocumentsAsync(relation.NguoiDungId, cancellationToken);
            if (user == null)
                return UserErrors.NotFound;

            if (yeuCau.YeuCauLoaiQuanHeId.HasValue)
            {
                relation.ThayDoiLoaiQuanHe(LoaiQuanHeCuTru.FromValue(yeuCau.YeuCauLoaiQuanHeId.Value, null)!);
                _quanHeCuTruRepository.Update(relation);
            }

            // --- Update User Profile & Synchronize Documents ---
            user.UpdateProfile(
                yeuCau.YeuCauTen ?? user.Ten,
                yeuCau.YeuCauHo ?? user.Ho,
                yeuCau.YeuCauNgaySinh ?? user.NgaySinh,
                yeuCau.YeuCauGioiTinhId.HasValue ? GioiTinh.FromValue(yeuCau.YeuCauGioiTinhId.Value, null)! : user.GioiTinhId,
                yeuCau.YeuCauDiaChi?.FullAddress ?? user.DiaChi.FullAddress,
                yeuCau.YeuCauCCCD ?? user.CCCD,
                yeuCau.YeuCauSoDienThoai ?? user.SoDienThoai);

            var proposedDocs = yeuCau.YeuCauTaiLieuCuTrus.Select(d => new DocumentSyncItem(
                d.TaiLieuCuTruId,
                d.LoaiGiayToId.Value,
                d.SoGiayTo,
                d.NgayPhatHanh,
                d.Files.Select(f => f.Id).ToList()
            ));

            var fetchedFiles = yeuCau.YeuCauTaiLieuCuTrus.SelectMany(d => d.Files);

            _documentReconciliationService.ReconcileNguoiDungDocuments(user, proposedDocs, fetchedFiles);
            _userRepository.Update(user);
        }
        else if (yeuCau.LoaiHanhDongYeuCauId == LoaiHanhDongYeuCau.Xoa)
        {
            var relation = await _quanHeCuTruRepository.GetByIdAsync(yeuCau.YeuCauQuanHeCuTruId!.Value, cancellationToken);
            if (relation != null)
            {
                // End Residency & Update Apartment Status
                relation.KetThucCuTru(now.DateTime);

                var activeRelations = await _quanHeCuTruRepository.GetByCanHoIdAsync(yeuCau.CanHoId, cancellationToken);
                if (activeRelations.Any(r => r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru))
                {
                    canHo.MarkAsOccupied();
                }
                else
                {
                    canHo.MarkAsVacant();
                }

                _quanHeCuTruRepository.Update(relation);
                _canHoRepository.Update(canHo);
            }
        }


        _yeuCauRepository.Update(yeuCau);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new YeuCauCuTruResponse
        {
            Id = yeuCau.Id,
            CanHoId = yeuCau.CanHoId,
            LoaiYeuCauId = yeuCau.LoaiHanhDongYeuCauId.Value,
            TenLoaiYeuCau = yeuCau.LoaiHanhDongYeuCauId.Name,
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
            YeuCauDiaChi = yeuCau.YeuCauDiaChi!.FullAddress
        });
    }
}
