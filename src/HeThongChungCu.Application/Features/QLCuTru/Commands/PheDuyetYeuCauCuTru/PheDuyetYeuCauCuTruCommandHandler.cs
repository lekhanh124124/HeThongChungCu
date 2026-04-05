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
    private readonly IResidencyService _residencyService;
    private readonly IUnitOfWork _unitOfWork;

    public PheDuyetYeuCauCuTruCommandHandler(
        IYeuCauCuTruCommandRepository yeuCauRepository,
        INguoiDungCommandRepository userRepository,
        IQuanHeCuTruCommandRepository quanHeCuTruRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        ICanHoCommandRepository canHoRepository,
        IResidencyService residencyService,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _userRepository = userRepository;
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _canHoRepository = canHoRepository;
        _residencyService = residencyService;
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

        var canHo = await _canHoRepository.GetByIdAsync(yeuCau.CanHoId, cancellationToken);
        if (canHo == null)
            return Result.Failure<YeuCauCuTruResponse>(CanHoErrors.NotFound);

        var now = _dateTimeProvider.UtcNow;
        yeuCau.Approve(adminId.Value, now);

        // Logic Phê duyệt
        if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Them)
        {
            // Gather data for uniqueness check
            var cccdExists = !string.IsNullOrEmpty(yeuCau.YeuCauCCCD) && await _userRepository.AnyAsync(u => u.CCCD == yeuCau.YeuCauCCCD, cancellationToken);
            var phoneExists = !string.IsNullOrEmpty(yeuCau.YeuCauSoDienThoai) && await _userRepository.AnyAsync(u => u.SoDienThoai == yeuCau.YeuCauSoDienThoai, cancellationToken);

            var uniquenessResult = _residencyService.CheckUniqueness(cccdExists, phoneExists);
            if (uniquenessResult.IsFailure)
                return Result.Failure<YeuCauCuTruResponse>(uniquenessResult.Errors[0]);

            // 1. Create User via Domain Service
            var newUser = _residencyService.CreateUserFromRequest(yeuCau);
            await _userRepository.AddAsync(newUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 2. Create Residency Relation via Domain Service
            var loaiQuanHe = LoaiQuanHeCuTru.FromValue(yeuCau.YeuCauLoaiQuanHeId!.Value, null);
            var existingRelations = await _quanHeCuTruRepository.GetByCanHoIdAsync(yeuCau.CanHoId, cancellationToken);
            
            var relationResult = _residencyService.CreateRelation(yeuCau.CanHoId, newUser.Id, loaiQuanHe!, now.DateTime, existingRelations);
            if (relationResult.IsFailure)
                return Result.Failure<YeuCauCuTruResponse>(relationResult.Errors[0]);

            await _quanHeCuTruRepository.AddAsync(relationResult.Value, cancellationToken);

            // 3. Update Apartment Status via Domain Service
            _residencyService.StartResidency(canHo, relationResult.Value, existingRelations.Append(relationResult.Value));
            _canHoRepository.Update(canHo);
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

            // --- Delegate to Domain Service for Update & Reconciliation ---
            _residencyService.UpdateUserFromRequest(user, yeuCau);
            _userRepository.Update(user);
        }
        else if (yeuCau.LoaiYeuCauId == LoaiYeuCau.Xoa)
        {
            var relation = await _quanHeCuTruRepository.GetByIdAsync(yeuCau.YeuCauQuanHeCuTruId!.Value, cancellationToken);
            if (relation != null)
            {
                // Update Apartment Status via Domain Service
                var activeRelations = await _quanHeCuTruRepository.GetByCanHoIdAsync(yeuCau.CanHoId, cancellationToken);
                _residencyService.EndResidency(canHo, relation, activeRelations, now.DateTime);

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
            YeuCauDiaChi = yeuCau.YeuCauDiaChi.FullAddress
        });
    }
}
