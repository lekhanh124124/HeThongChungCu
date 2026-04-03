using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Commands.KichHoatPhuongTien;

internal sealed class KichHoatPhuongTienCommandHandler : ICommandHandler<KichHoatPhuongTienCommand, bool>
{
    private readonly IPhuongTienCommandRepository _phuongTienCommandRepository;
    private readonly ICanHoCommandRepository _canHoCommandRepository;
    private readonly IVehicleRegistryService _vehicleRegistryService;
    private readonly IUnitOfWork _unitOfWork;

    public KichHoatPhuongTienCommandHandler(
        IPhuongTienCommandRepository phuongTienCommandRepository,
        ICanHoCommandRepository canHoCommandRepository,
        IVehicleRegistryService vehicleRegistryService,
        IUnitOfWork unitOfWork)
    {
        _phuongTienCommandRepository = phuongTienCommandRepository;
        _canHoCommandRepository = canHoCommandRepository;
        _vehicleRegistryService = vehicleRegistryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(KichHoatPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var phuongTiens = await _phuongTienCommandRepository.GetPhuongTiensByIdsAsync(request.PhuongTienIds, cancellationToken);
        
        if (phuongTiens.Count == 0)
        {
            return Result.Failure<bool>(PhuongTienErrors.NotFound);
        }

        // Nhóm theo căn hộ để tối ưu query hạn mức
        var groupedByCanHo = phuongTiens.GroupBy(x => x.CanHoId);

        foreach (var group in groupedByCanHo)
        {
            var canHoId = group.Key;
            var canHo = await _canHoCommandRepository.GetByIdAsync(canHoId, cancellationToken);
            
            if (canHo == null) continue;

            // Lấy tất cả phương tiện hiện có của căn hộ này (để check quota)
            var existingVehicles = await _phuongTienCommandRepository.GetPhuongTiensByCanHoIdAsync(canHoId, cancellationToken);
            
            foreach (var phuongTien in group)
            {
                var result = _vehicleRegistryService.KichHoatPhuongTien(phuongTien, canHo, existingVehicles.Where(v => v.TrangThaiPhuongTienId == TrangThaiPhuongTien.Active));
                if (result.IsFailure)
                    return Result.Failure<bool>(result.Errors);

                _phuongTienCommandRepository.Update(phuongTien);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
