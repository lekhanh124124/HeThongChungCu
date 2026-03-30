using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Application.Common.Options;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoHoSo;

public class TaoHoSoCommandHandler : ICommandHandler<TaoHoSoCommand, UserInfoResponse>
{
    private readonly INguoiDungEFRepository _userRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaoHoSoCommandHandler(
        INguoiDungEFRepository userRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserInfoResponse>> Handle(TaoHoSoCommand request, CancellationToken cancellationToken)
    {
        // Double check by IdCard before creating to prevent duplicates if someone calls this directly after search
        NguoiDung? user;
        if (!string.IsNullOrEmpty(request.IdCard))
        {
            user = await _userRepository.GetByCCCDAsync(request.IdCard, cancellationToken);
            if (user != null)
            {
                return Result.Success(new UserInfoResponse
                {
                    Id = user.Id,
                    FirstName = user.Ten,
                    LastName = user.Ho,
                    Dob = user.NgaySinh,
                    GioiTinhId = user.GioiTinhId.Value,
                    GioiTinhName = user.GioiTinhId.Name,
                    DiaChi = user.DiaChi,
                    IdCard = user.CCCD,
                    PhoneNumber = user.SoDienThoai ?? string.Empty,
                    TaiLieuCuTrus = user.TaiLieu.Select(d => new TaiLieuResponse
                    {
                        Id = d.Id,
                        LoaiGiayToId = d.LoaiGiayToId.Value,
                        TenLoaiGiayTo = d.LoaiGiayToId.Name,
                        SoGiayTo = d.SoGiayTo,
                        NgayPhatHanh = d.NgayPhatHanh,
                        Files = d.Files.Select(f => new TepTaiLieuResponse(f.Id, f.FileUrl, f.FileName, f.ContentType)).ToList()
                    }).ToList()
                });
            }
        }

        user = new NguoiDung(
            request.FirstName,
            request.LastName,
            request.Dob,
            GioiTinh.FromValue(request.GioiTinhId)!,
            request.DiaChi,
            request.IdCard);

        await _userRepository.AddAsync(user, cancellationToken);

        // 2. Fetch all TepTaiLieus at once
        var allFileIds = request.TaiLieuCuTrus?.SelectMany(d => d.FileIds).Distinct().ToList() ?? new List<int>();
        var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);
        var tepTaiLieuDict = tepTaiLieus.ToDictionary(f => f.Id);

        if (request.TaiLieuCuTrus != null && request.TaiLieuCuTrus.Any())
        {
            foreach (var docReq in request.TaiLieuCuTrus)
            {
                var loaiGiayTo = LoaiGiayTo.FromValue(docReq.LoaiGiayToId, null);
                if (loaiGiayTo is null) continue;

                var files = new List<TepTaiLieu>();
                foreach (var fileId in docReq.FileIds)
                {
                    if (tepTaiLieuDict.TryGetValue(fileId, out var file))
                    {
                        files.Add(file);
                    }
                }

                var document = new TaiLieuNguoiDung(
                    user.Id,
                    loaiGiayTo,
                    docReq.SoGiayTo,
                    docReq.NgayPhatHanh,
                    files);

                user.AddDocument(document);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserInfoResponse
        {
            Id = user.Id,
            FirstName = user.Ten,
            LastName = user.Ho,
            Dob = user.NgaySinh,
            GioiTinhId = user.GioiTinhId.Value,
            GioiTinhName = user.GioiTinhId.Name,
            DiaChi = user.DiaChi,
            IdCard = user.CCCD,
            PhoneNumber = user.SoDienThoai ?? string.Empty,
            TaiLieuCuTrus = user.TaiLieu.Select(d => new TaiLieuResponse
            {
                Id = d.Id,
                LoaiGiayToId = d.LoaiGiayToId.Value,
                TenLoaiGiayTo = d.LoaiGiayToId.Name,
                SoGiayTo = d.SoGiayTo,
                NgayPhatHanh = d.NgayPhatHanh,
                Files = d.Files.Select(f => new TepTaiLieuResponse(f.Id, f.FileUrl, f.FileName, f.ContentType)).ToList()
            }).ToList()
        });
    }
}
