using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.ThietLapCuTru;

public class ThietLapCuTruCommandHandler : ICommandHandler<ThietLapCuTruCommand, CuDanResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IUserEFRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ThietLapCuTruCommandHandler(
        ICanHoEFRepository canHoRepository,
        IUserEFRepository userRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _canHoRepository = canHoRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CuDanResponse>> Handle(ThietLapCuTruCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdWithQuanHeAsync(request.CanHoId, cancellationToken);
        if (canHo is null)
            return Result.Failure<CuDanResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<CuDanResponse>(UserErrors.NotFoundById(request.UserId));

        var alreadyResident = canHo.QuanHeCuTrus
            .Any(q => q.UserId == user.Id && !q.IsKetThuc);

        if (alreadyResident)
            return Result.Failure<CuDanResponse>(QuanHeCuTruErrors.UserAlreadyResident);

        Role userRole = Role.FromValue(user.RoleId)!;

        if (userRole == Role.Guest)
        {
            user.ChangeRole(Role.Resident);
        }

        var loaiQuanHe = LoaiQuanHeCuTru.FromValue(request.LoaiQuanHeCuTruId);
        var now = _dateTimeProvider.Now.DateTime;
        canHo.AddQuanHeCuTru(user.Id, loaiQuanHe!.Value, now);

        _canHoRepository.Update(canHo);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var newRecord = canHo.QuanHeCuTrus
            .Where(q => q.UserId == user.Id && !q.IsKetThuc)
            .OrderByDescending(q => q.NgayBatDau)
            .First();

        return Result.Success(new CuDanResponse
        {
            QuanHeCuTruId = newRecord.Id,
            UserId = newRecord.UserId,
            HoTen = $"{user.FirstName} {user.LastName}", 
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            LoaiQuanHeCuTruId = newRecord.LoaiQuanHeCuTruId,
            TenLoaiQuanHeCuTru = loaiQuanHe.Name,
            NgayBatDau = newRecord.NgayBatDau
        });
    }
}
