using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.TimHoSoTheoCCCD;

public class TimHoSoTheoCCCDQueryHandler : IQueryHandler<TimHoSoTheoCCCDQuery, UserInfoResponse>
{
    private readonly INguoiDungCommandRepository _userRepository;

    public TimHoSoTheoCCCDQueryHandler(INguoiDungCommandRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserInfoResponse>> Handle(TimHoSoTheoCCCDQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByCCCDAsync(request.IdCard, cancellationToken);

        if (user is null)
            return Result.Failure<UserInfoResponse>(UserErrors.NotFoundByIdCard(request.IdCard));

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
