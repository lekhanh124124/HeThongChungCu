using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Commands.ThietLapCuTru;

public class ThietLapCuTruCommandHandler : ICommandHandler<ThietLapCuTruCommand, CuDanResponse>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IUserEFRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ThietLapCuTruCommandHandler(
        ICanHoEFRepository canHoRepository,
        IUserEFRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _canHoRepository = canHoRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CuDanResponse>> Handle(ThietLapCuTruCommand request, CancellationToken cancellationToken)
    {
        // Validate LoaiQuanHeCuTruId
        if (!LoaiQuanHeCuTru.GetAll().Any(l => l.Value == request.LoaiQuanHeCuTruId))
            return Result.Failure<CuDanResponse>(QuanHeCuTruErrors.LoaiQuanHeKhongHopLe);

        // Load CanHo with its residents
        var canHo = await _canHoRepository.GetByIdWithQuanHeAsync(request.CanHoId, cancellationToken);
        if (canHo is null)
            return Result.Failure<CuDanResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        // Validate User exists
        var userExists = await _userRepository.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
            return Result.Failure<CuDanResponse>(new Error("User.NotFound", $"Không tìm thấy cư dân với ID '{request.UserId}'."));

        // Check user not already an active resident
        var alreadyResident = canHo.QuanHeCuTrus
            .Any(q => q.UserId == request.UserId && q.TrangThai);
        if (alreadyResident)
            return Result.Failure<CuDanResponse>(QuanHeCuTruErrors.UserAlreadyResident);

        canHo.AddQuanHeCuTru(request.UserId, request.LoaiQuanHeCuTruId, request.NgayBatDau);
        _canHoRepository.Update(canHo);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var newRecord = canHo.QuanHeCuTrus
            .Where(q => q.UserId == request.UserId && q.TrangThai)
            .OrderByDescending(q => q.NgayBatDau)
            .First();

        var loai = LoaiQuanHeCuTru.GetAll().First(l => l.Value == request.LoaiQuanHeCuTruId);

        return Result.Success(new CuDanResponse
        {
            QuanHeCuTruId = newRecord.Id,
            UserId = newRecord.UserId,
            HoTen = string.Empty, // populated by query; here we only return IDs
            Email = string.Empty,
            PhoneNumber = string.Empty,
            LoaiQuanHeCuTruId = newRecord.LoaiQuanHeCuTruId,
            LoaiQuanHeTen = loai.Name,
            NgayBatDau = newRecord.NgayBatDau
        });
    }
}
