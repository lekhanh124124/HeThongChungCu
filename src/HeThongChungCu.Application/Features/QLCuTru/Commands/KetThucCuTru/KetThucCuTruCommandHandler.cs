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
    private readonly IHoaDonCommandRepository _hoaDonRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public KetThucCuTruCommandHandler(
        IQuanHeCuTruCommandRepository quanHeCuTruRepository,
        INguoiDungCommandRepository userRepository,
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaRepository,
        IHoaDonCommandRepository hoaDonRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _userRepository = userRepository;
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _hoaDonRepository = hoaDonRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CuDanResponse>> Handle(KetThucCuTruCommand request, CancellationToken cancellationToken)
    {
        var quanHe = await _quanHeCuTruRepository.GetByIdAsync(request.QuanHeCuTruId, cancellationToken);
        if (quanHe is null)
            return QuanHeCuTruErrors.NotFoundById(request.QuanHeCuTruId);

        var hasUnpaidInvoices = await _hoaDonRepository.HasUnpaidInvoicesAsync(quanHe.CanHoId, cancellationToken);
        if (hasUnpaidInvoices)
        {
            return new Error("KetThucCuTru.HasUnpaidInvoices", "Không thể kết thúc cư trú vì căn hộ vẫn còn dư nợ hóa đơn chưa thanh toán.");
        }

        var now = _dateTimeProvider.Now.DateTime;
        quanHe.KetThucCuTru(now);
        _quanHeCuTruRepository.Update(quanHe);

        var activeRelations = await _quanHeCuTruRepository.GetByCanHoIdAsync(quanHe.CanHoId, cancellationToken);

        if (quanHe.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || quanHe.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue)
        {
            var dependents = activeRelations.Where(r => r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru && r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiOCung && r.Id != quanHe.Id);
            foreach (var dependent in dependents)
            {
                dependent.KetThucCuTru(now);
                _quanHeCuTruRepository.Update(dependent);
            }
        }

        var user = await _userRepository.GetByIdAsync(quanHe.NguoiDungId, cancellationToken);
        var canHo = await _canHoRepository.GetByIdAsync(quanHe.CanHoId, cancellationToken);

        if (canHo != null)
        {
            bool hasOwner = activeRelations.Any(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru && r.Id != quanHe.Id);
            bool hasTenant = activeRelations.Any(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru && r.Id != quanHe.Id);
            
            canHo.SyncStatusWithResidency(hasOwner, hasTenant);
            
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
