using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoYeuCauCuTru;

public class TaoYeuCauCuTruCommandHandler : ICommandHandler<TaoYeuCauCuTruCommand, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruEFRepository _yeuCauRepository;
    private readonly INguoiDungEFRepository _userRepository;
    private readonly IQuanHeCuTruEFRepository _quanHeRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public TaoYeuCauCuTruCommandHandler(
        IYeuCauCuTruEFRepository yeuCauRepository,
        INguoiDungEFRepository userRepository,
        IQuanHeCuTruEFRepository quanHeRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _userRepository = userRepository;
        _quanHeRepository = quanHeRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(TaoYeuCauCuTruCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<YeuCauCuTruResponse>(UserErrors.NotFound);

        var loaiYeuCau = LoaiYeuCau.FromValue(request.LoaiYeuCauId, null);
        if (loaiYeuCau == null)
            return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.InvalidEnumValue);

        // Fetch the relation of the current user for this apartment to validate ChuHo
        var requesterRelation = await _quanHeRepository.GetByUserAndCanHoAsync(userId.Value, request.CanHoId, cancellationToken);
        if (requesterRelation == null)
            return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.NotFoundById(request.CanHoId));

        if (requesterRelation.LoaiQuanHeCuTruId != LoaiQuanHeCuTru.ChuHo)
            return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.Forbidden("Chỉ có chủ hộ của căn hộ này mới có quyền tạo yêu cầu cư trú."));


        // Fetch all TepTaiLieus at once
        var allFileIds = request.TaiLieuCuTrus?.SelectMany(d => d.FileIds).Distinct().ToList() ?? new List<int>();
        var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);
        var tepTaiLieuDict = tepTaiLieus.ToDictionary(f => f.Id);

        var requestDocuments = request.TaiLieuCuTrus?.Select(a =>
        {
            var files = new List<TepTaiLieu>();
            foreach (var fileId in a.FileIds)
            {
                if (tepTaiLieuDict.TryGetValue(fileId, out var file))
                {
                    files.Add(file);
                }
            }

            return new YeuCauTaiLieuCuTru(
                LoaiGiayTo.FromValue(a.LoaiGiayToId, null)!,
                a.SoGiayTo,
                a.NgayPhatHanh,
                files);
        }).ToList();

        YeuCauCuTru yeuCau;
        if (loaiYeuCau == LoaiYeuCau.Them)
        {
            if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName) || request.LoaiQuanHeId == null)
                return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.BadRequest("Missing member information."));

            yeuCau = YeuCauCuTru.CreateAddMemberRequest(
                request.CanHoId,
                userId.Value,
                request.QuanHeCuTruId,
                request.LoaiQuanHeId.Value,
                request.FirstName,
                request.LastName,
                request.Dob,
                request.GioiTinhId,
                request.PhoneNumber,
                request.NoiDung,
                requestDocuments,
                _dateTimeProvider.UtcNow);
        }
        else if (loaiYeuCau == LoaiYeuCau.Sua)
        {
            if (request.QuanHeCuTruId == null)
                return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.BadRequest("QuanHeCuTruId is required for update request."));

            yeuCau = YeuCauCuTru.CreateUpdateMemberRequest(
                request.CanHoId,
                userId.Value,
                request.QuanHeCuTruId.Value,
                request.NewLoaiQuanHeId,
                request.FirstName,
                request.LastName,
                request.Dob,
                request.GioiTinhId,
                request.PhoneNumber,
                request.NoiDung,
                requestDocuments,
                _dateTimeProvider.UtcNow);
        }
        else // Xoa
        {
            if (request.QuanHeCuTruId == null)
                return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.BadRequest("QuanHeCuTruId is required for remove request."));

            yeuCau = YeuCauCuTru.CreateRemoveMemberRequest(
               request.CanHoId,
               userId.Value,
               request.QuanHeCuTruId.Value,
               request.NoiDung,
               _dateTimeProvider.UtcNow);
        }

        await _yeuCauRepository.AddAsync(yeuCau, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new YeuCauCuTruResponse
        {
            Id = yeuCau.Id,
            CanHoId = yeuCau.CanHoId,
            LoaiYeuCauId = yeuCau.LoaiYeuCauId.Value,
            TenLoaiYeuCau = yeuCau.LoaiYeuCauId.Name,
            TrangThaiId = yeuCau.TrangThaiId.Value,
            TenTrangThai = yeuCau.TrangThaiId.Name,
            Reason = yeuCau.LyDo,
            NoiDung = yeuCau.NoiDung,
            CreatedAt = yeuCau.CreatedAt,
            ProposedLoaiQuanHeId = yeuCau.YeuCauLoaiQuanHeId,
            QuanHeCuTruId = yeuCau.QuanHeCuTruId
        };
    }
}
