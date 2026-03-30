using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.KetThucCuTru;

public class KetThucCuTruCommandHandler : ICommandHandler<KetThucCuTruCommand, CuDanResponse>
{
    private readonly IQuanHeCuTruEFRepository _quanHeCuTruRepository;
    private readonly INguoiDungEFRepository _userRepository;
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IToaNhaEFRepository _toaNhaRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public KetThucCuTruCommandHandler(
        IQuanHeCuTruEFRepository quanHeCuTruRepository,
        INguoiDungEFRepository userRepository,
        ICanHoEFRepository canHoRepository,
        IToaNhaEFRepository toaNhaRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _userRepository = userRepository;
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CuDanResponse>> Handle(KetThucCuTruCommand request, CancellationToken cancellationToken)
    {
        var quanHe = await _quanHeCuTruRepository.GetByIdAsync(request.QuanHeCuTruId, cancellationToken);
        if (quanHe is null)
            return Result.Failure<CuDanResponse>(QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId));

        var now = _dateTimeProvider.Now.DateTime;
        quanHe.KetThucCuTru(now);
        _quanHeCuTruRepository.Update(quanHe);

        var user = await _userRepository.GetByIdAsync(quanHe.NguoiDungId, cancellationToken);
        var canHo = await _canHoRepository.GetByIdAsync(quanHe.CanHoId, cancellationToken);
        var toaNha = canHo != null ? await _toaNhaRepository.GetToaNhaByTangIdAsync(canHo.TangId, cancellationToken) : null;
        var tang = toaNha?.Tangs.FirstOrDefault(t => t.Id == canHo?.TangId);

        return Result.Success(new CuDanResponse
        {
            MaToaNha = toaNha?.MaToaNha ?? string.Empty,
            MaTang = tang?.MaTang ?? string.Empty,
            MaCanHo = canHo?.MaCanHo ?? string.Empty,
            QuanHeCuTruId = quanHe.Id,
            UserId = quanHe.NguoiDungId,
            HoTen = user?.HoTen ?? string.Empty,
            PhoneNumber = user?.SoDienThoai,
            LoaiQuanHeCuTruId = quanHe.LoaiQuanHeCuTruId.Value,
            TenLoaiQuanHeCuTru = quanHe.LoaiQuanHeCuTruId.Name,
            NgayBatDau = quanHe.NgayBatDau,
            NgayKetThuc = quanHe.NgayKetThuc,
            TrangThaiCuTruId = quanHe.TrangThaiCuTruId.Value,
            TenTrangThaiCuTru = quanHe.TrangThaiCuTruId.Name
        });
    }
}
