using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoYeuCauCuTru;

public class TaoYeuCauCuTruCommandHandler : ICommandHandler<TaoYeuCauCuTruCommand, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruCommandRepository _yeuCauRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly IQuanHeCuTruCommandRepository _quanHeRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public TaoYeuCauCuTruCommandHandler(
        IYeuCauCuTruCommandRepository yeuCauRepository,
        INguoiDungCommandRepository userRepository,
        IQuanHeCuTruCommandRepository quanHeRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _userRepository = userRepository;
        _quanHeRepository = quanHeRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(TaoYeuCauCuTruCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return UserErrors.NotFound;

        var loaiYeuCau = LoaiHanhDongYeuCau.FromValue(request.LoaiYeuCauId, null);
        // Validate permissions via Domain Service
        var requesterRelation = await _quanHeRepository.GetByUserAndCanHoAsync(userId.Value, request.CanHoId, cancellationToken);
        // Validate permissions
        if (requesterRelation == null)
        {
            return QuanHeCuTruErrors.NotFound;
        }

        if (requesterRelation.TrangThaiCuTruId != TrangThaiCuTru.DangCuTru ||
            (requesterRelation.LoaiQuanHeCuTruId != LoaiQuanHeCuTru.ChuHo &&
             requesterRelation.LoaiQuanHeCuTruId != LoaiQuanHeCuTru.NguoiThue))
        {
            return YeuCauCuTruErrors.Forbidden;
        }


        // Fetch all TepTaiLieus at once
        var allFileIds = request.TaiLieuCuTrus?.SelectMany(d => d.FileIds).Distinct().ToList() ?? [];
        var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);
        var tepTaiLieuDict = tepTaiLieus.ToDictionary(f => f.Id);

        var requestDocuments = request.TaiLieuCuTrus?.Select(a =>
        {
            var files = a.FileIds
                .Where(id => tepTaiLieuDict.ContainsKey(id))
                .Select(id => tepTaiLieuDict[id])
                .Select(f => new TepYeuCauTaiLieuCuTru(f.FileName, f.FileUrl, f.Size, f.ContentType))
                .ToList();

            return YeuCauCuTru.CreateYeuCauTaiLieuCuTru(
                LoaiGiayTo.FromValue(a.LoaiGiayToId, null)!,
                a.SoGiayTo,
                a.NgayPhatHanh,
                files,
                a.TaiLieuCuTruId);
        }).ToList();

        var initialStatus = request.IsSubmit ? TrangThaiYeuCau.Pending : TrangThaiYeuCau.Saved;

        YeuCauCuTru yeuCau;
        if (loaiYeuCau == LoaiHanhDongYeuCau.Them)
        {
            yeuCau = YeuCauCuTru.CreateAddMemberRequest(
                request.CanHoId,
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
                initialStatus);
        }
        else if (loaiYeuCau == LoaiHanhDongYeuCau.Sua)
        {
            yeuCau = YeuCauCuTru.CreateUpdateMemberRequest(
                request.CanHoId,
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
                initialStatus);
        }
        else // Xoa
        {
            yeuCau = YeuCauCuTru.CreateRemoveMemberRequest(
               request.CanHoId,
               request.TargetQuanHeCuTruId!.Value,
               request.NoiDung,
               initialStatus);
        }

        await _yeuCauRepository.AddAsync(yeuCau, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new YeuCauCuTruResponse
        {
            Id = yeuCau.Id,
            CanHoId = yeuCau.CanHoId,
            LoaiYeuCauId = yeuCau.LoaiHanhDongYeuCauId.Value,
            TenLoaiYeuCau = yeuCau.LoaiHanhDongYeuCauId.Name,
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
            YeuCauDiaChi = yeuCau.YeuCauDiaChi?.FullAddress,
            YeuCauLoaiQuanHeId = yeuCau.YeuCauLoaiQuanHeId,
            TargetQuanHeCuTruId = yeuCau.YeuCauQuanHeCuTruId,
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
