using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.KetThucCuTru;

public class KetThucCuTruCommandHandler : ICommandHandler<KetThucCuTruCommand, CuDanResponse>
{
    private readonly IQuanHeCuTruCommandRepository _quanHeCuTruRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public KetThucCuTruCommandHandler(
        IQuanHeCuTruCommandRepository quanHeCuTruRepository,
        INguoiDungCommandRepository userRepository,
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _userRepository = userRepository;
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CuDanResponse>> Handle(KetThucCuTruCommand request, CancellationToken cancellationToken)
    {
        var quanHe = await _quanHeCuTruRepository.GetByIdAsync(request.QuanHeCuTruId, cancellationToken);
        if (quanHe is null)
            return QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId);

        var now = _dateTimeProvider.Now.DateTime;
        quanHe.KetThucCuTru(now);
        _quanHeCuTruRepository.Update(quanHe);

        var user = await _userRepository.GetByIdAsync(quanHe.NguoiDungId, cancellationToken);
        var canHo = await _canHoRepository.GetByIdAsync(quanHe.CanHoId, cancellationToken);

        if (canHo != null)
        {
            var activeRelations = await _quanHeCuTruRepository.GetByCanHoIdAsync(canHo.Id, cancellationToken);
            
            // Logic moved from ResidencyService.EndResidency & UpdateApartmentStatus
            bool hasOwner = activeRelations.Any(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);
            bool hasTenant = activeRelations.Any(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);
            
            canHo.SyncStatusWithResidency(hasOwner, hasTenant);
            
            _quanHeCuTruRepository.Update(quanHe);
            _canHoRepository.Update(canHo);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
            NgayBatDau = quanHe.ThoiGian.NgayBatDau,
            NgayKetThuc = quanHe.ThoiGian.NgayKetThuc,
            TrangThaiCuTruId = quanHe.TrangThaiCuTruId.Value,
            TenTrangThaiCuTru = quanHe.TrangThaiCuTruId.Name
        });
    }
}
