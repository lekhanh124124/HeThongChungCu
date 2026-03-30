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
        // Fetch the relation of the current user for this apartment to validate ChuHo
        var requesterRelation = await _quanHeRepository.GetByUserAndCanHoAsync(userId.Value, request.CanHoId, cancellationToken);
        if (requesterRelation == null)
            return Result.Failure<YeuCauCuTruResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        if (requesterRelation.LoaiQuanHeCuTruId != LoaiQuanHeCuTru.ChuHo)
            return Result.Failure<YeuCauCuTruResponse>(YeuCauCuTruErrors.Forbidden);


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
                files,
                a.TaiLieuCuTruId);
        }).ToList();

        var initialStatus = request.IsSubmit ? TrangThaiYeuCau.Pending : TrangThaiYeuCau.Saved;

        YeuCauCuTru yeuCau;
        if (loaiYeuCau == LoaiYeuCau.Them)
        {
            yeuCau = YeuCauCuTru.CreateAddMemberRequest(
                request.CanHoId,
                userId.Value,
                request.TargetQuanHeCuTruId,
                request.LoaiQuanHeId!.Value,
                request.FirstName,
                request.LastName,
                request.Dob,
                request.GioiTinhId,
                request.PhoneNumber,
                request.CCCD,
                request.DiaChi,
                request.NoiDung,
                requestDocuments,
                _dateTimeProvider.Now,
                initialStatus);
        }
        else if (loaiYeuCau == LoaiYeuCau.Sua)
        {
            yeuCau = YeuCauCuTru.CreateUpdateMemberRequest(
                request.CanHoId,
                userId.Value,
                request.TargetQuanHeCuTruId!.Value,
                request.LoaiQuanHeId,
                request.FirstName,
                request.LastName,
                request.Dob,
                request.GioiTinhId,
                request.PhoneNumber,
                request.CCCD,
                request.DiaChi,
                request.NoiDung,
                requestDocuments,
                _dateTimeProvider.Now,
                initialStatus);
        }
        else // Xoa
        {
            yeuCau = YeuCauCuTru.CreateRemoveMemberRequest(
               request.CanHoId,
               userId.Value,
               request.TargetQuanHeCuTruId!.Value,
               request.NoiDung,
               _dateTimeProvider.Now,
               initialStatus);
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
            LyDo = yeuCau.LyDo,
            NoiDung = yeuCau.NoiDung,
            CreatedAt = yeuCau.CreatedAt,
            YeuCauTen = yeuCau.YeuCauTen,
            YeuCauHo = yeuCau.YeuCauHo,
            YeuCauNgaySinh = yeuCau.YeuCauNgaySinh,
            YeuCauGioiTinhId = yeuCau.YeuCauGioiTinhId,
            YeuCauSoDienThoai = yeuCau.YeuCauSoDienThoai,
            YeuCauCCCD = yeuCau.YeuCauCCCD,
            YeuCauDiaChi = yeuCau.YeuCauDiaChi,
            YeuCauLoaiQuanHeId = yeuCau.YeuCauLoaiQuanHeId,
            TargetQuanHeCuTruId = yeuCau.QuanHeCuTruId,
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
                Files = d.Files.Select(f => new TepTaiLieuResponse(
                    f.Id,
                    f.FileUrl,
                    f.FileName,
                    f.ContentType)).ToList()
            }).ToList()
        };
    }
}
