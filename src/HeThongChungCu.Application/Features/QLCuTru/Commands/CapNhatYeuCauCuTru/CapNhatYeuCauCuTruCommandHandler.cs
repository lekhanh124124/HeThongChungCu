using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.CapNhatYeuCauCuTru;

public class CapNhatYeuCauCuTruCommandHandler : ICommandHandler<CapNhatYeuCauCuTruCommand, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruEFRepository _yeuCauRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CapNhatYeuCauCuTruCommandHandler(
        IYeuCauCuTruEFRepository yeuCauRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _yeuCauRepository = yeuCauRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(CapNhatYeuCauCuTruCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<YeuCauCuTruResponse>(UserErrors.NotFound);

        var yeuCau = await _yeuCauRepository.GetByIdAsync(request.Id, cancellationToken);
        if (yeuCau == null)
            return Result.Failure<YeuCauCuTruResponse>(YeuCauCuTruErrors.NotFoundById(request.Id));

        if (yeuCau.CreatedBy != userId)
            return Result.Failure<YeuCauCuTruResponse>(YeuCauCuTruErrors.Forbidden);

        if (request.IsWithdraw)
        {
            yeuCau.Withdraw();
        }
        else
        {
            // Fetch all TepTaiLieus if provided
            List<YeuCauTaiLieuCuTru>? requestDocuments = null;
            if (request.TaiLieuCuTrus != null)
            {
                var allFileIds = request.TaiLieuCuTrus.SelectMany(d => d.FileIds).Distinct().ToList();
                var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);
                var tepTaiLieuDict = tepTaiLieus.ToDictionary(f => f.Id);

                requestDocuments = request.TaiLieuCuTrus.Select(a =>
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
            }

            // Update details
            yeuCau.Update(
                request.FirstName,
                request.LastName,
                request.Dob,
                request.GioiTinhId,
                request.PhoneNumber,
                request.CCCD,
                request.DiaChi,
                request.LoaiQuanHeId ?? yeuCau.YeuCauLoaiQuanHeId, // Use existing if null? Actually command should probably send full state or handle partial
                request.NoiDung,
                requestDocuments);

            if (request.IsSubmit)
            {
                yeuCau.Submit();
            }
        }

        _yeuCauRepository.Update(yeuCau);
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
