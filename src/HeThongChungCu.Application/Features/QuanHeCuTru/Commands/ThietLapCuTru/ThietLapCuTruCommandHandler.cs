using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.ThietLapCuTru;

public class ThietLapCuTruCommandHandler : ICommandHandler<ThietLapCuTruCommand, CuDanResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IUserEFRepository _userRepository;
    private readonly IQuanHeCuTruEFRepository _quanHeCuTruRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ThietLapCuTruCommandHandler(
        ICanHoEFRepository canHoRepository,
        IUserEFRepository userRepository,
        IQuanHeCuTruEFRepository quanHeCuTruRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _canHoRepository = canHoRepository;
        _userRepository = userRepository;
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CuDanResponse>> Handle(ThietLapCuTruCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdAsync(request.CanHoId, cancellationToken);
        if (canHo is null)
            return Result.Failure<CuDanResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<CuDanResponse>(UserErrors.NotFoundById(request.UserId));

        Role userRole = user.RoleId;

        if (userRole == Role.Guest)
        {
            user.ChangeRole(Role.Resident);
        }

        var loaiQuanHe = LoaiQuanHeCuTru.FromValue(request.LoaiQuanHeCuTruId);
        
        var existingRelations = await _quanHeCuTruRepository.GetByCanHoIdAsync(canHo.Id, cancellationToken);
        var now = _dateTimeProvider.Now.DateTime;

        var quanHe = new Domain.Entities.QuanHeCuTru(canHo.Id, user.Id, loaiQuanHe!, now, existingRelations);
        await _quanHeCuTruRepository.AddAsync(quanHe, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CuDanResponse
        {
            MaToaNha = canHo.Tang.ToaNha.MaToaNha,
            MaTang = canHo.Tang.MaTang,
            MaCanHo = canHo.MaCanHo,
            QuanHeCuTruId = quanHe.Id,
            UserId = user.Id,
            HoTen = $"{user.FirstName} {user.LastName}", 
            LoaiQuanHeCuTruId = quanHe.LoaiQuanHeCuTruId.Value,
            TenLoaiQuanHeCuTru = quanHe.LoaiQuanHeCuTruId.Name,
            NgayBatDau = quanHe.NgayBatDau,
            NgayKetThuc = quanHe.NgayKetThuc,
            TrangThaiCuTruId = quanHe.TrangThaiCuTruId.Value,
        });
    }
}
