using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Options;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using Microsoft.Extensions.Options;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.BoSungHoSo;

public class BoSungHoSoCommandHandler : ICommandHandler<BoSungHoSoCommand, UserInfoResponse>
{
    private readonly INguoiDungEFRepository _userRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BoSungHoSoCommandHandler(
        INguoiDungEFRepository userRepository,
        ITepTaiLieuRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserInfoResponse>> Handle(BoSungHoSoCommand request, CancellationToken cancellationToken)
    {
        // 1. Get User
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<UserInfoResponse>(UserErrors.NotFound);

        // 2. Fetch all TepTaiLieus at once
        var allFileIds = request.Documents.SelectMany(d => d.FileIds).Distinct().ToList();
        var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);
        var tepTaiLieuDict = tepTaiLieus.ToDictionary(f => f.Id);

        foreach (var docRequest in request.Documents)
        {
            // 3. Map FileIds to TepTaiLieu
            var files = new List<TepTaiLieu>();
            foreach (var fileId in docRequest.FileIds)
            {
                if (tepTaiLieuDict.TryGetValue(fileId, out var file))
                {
                    files.Add(file);
                }
            }

            var document = new TaiLieuNguoiDung(
                request.UserId,
                LoaiGiayTo.FromValue(docRequest.LoaiGiayToId)!,
                docRequest.SoGiayTo,
                docRequest.NgayPhatHanh,
                files);

            user.AddDocument(document);
        }

        // 4. Save
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
            Documents = user.TaiLieu.Select(d => new TaiLieuResponse
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
