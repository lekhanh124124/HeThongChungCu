using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.TaoHoSo;

public class TaoHoSoCommandHandler : ICommandHandler<TaoHoSoCommand, UserInfoResponse>
{
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TaoHoSoCommandHandler(
        INguoiDungCommandRepository userRepository,
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
        if (!string.IsNullOrEmpty(request.IdCard))
        {
            var IdCardExists = await _userRepository.AnyAsync(u => u.CCCD == request.IdCard, cancellationToken);
            if (IdCardExists)
            {
                return Result.Failure<UserInfoResponse>(UserErrors.IdCardAlreadyExists);
            }
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            var phoneExists = await _userRepository.AnyAsync(u => u.SoDienThoai!.Value == request.PhoneNumber, cancellationToken);
            if (phoneExists)
            {
                return Result.Failure<UserInfoResponse>(UserErrors.PhoneNumberAlreadyExists);
            }
        }

        var user = new NguoiDung(
            request.FirstName,
            request.LastName,
            request.Dob,
            GioiTinh.FromValue(request.GioiTinhId)!,
            request.DiaChi,
            request.IdCard,
            request.PhoneNumber);

        await _userRepository.AddAsync(user, cancellationToken);

        // 2. Fetch all TepTaiLieus at once
        var allFileIds = request.TaiLieuCuTrus?.SelectMany(d => d.FileIds).Distinct().ToList() ?? new List<int>();
        var tepTaiLieus = await _tepTaiLieuRepository.GetByIdsAsync(allFileIds, cancellationToken);
        var tepTaiLieuDict = tepTaiLieus.ToDictionary(f => f.Id);

        if (request.TaiLieuCuTrus != null && request.TaiLieuCuTrus.Count != 0)
        {
            foreach (var docReq in request.TaiLieuCuTrus)
            {
                var loaiGiayTo = LoaiGiayTo.FromValue(docReq.LoaiGiayToId, null);
                if (loaiGiayTo is null) continue;

                var files = docReq.FileIds
                    .Where(id => tepTaiLieuDict.ContainsKey(id))
                    .Select(id => tepTaiLieuDict[id])
                    .Select(f => f is TepTaiLieuNguoiDung tp ? tp : new TepTaiLieuNguoiDung(f.FileName, f.FileUrl, f.Size, f.ContentType))
                    .ToList();

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
            DiaChi = user.DiaChi.FullAddress,
            IdCard = user.CCCD,
            PhoneNumber = user.SoDienThoai,
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
