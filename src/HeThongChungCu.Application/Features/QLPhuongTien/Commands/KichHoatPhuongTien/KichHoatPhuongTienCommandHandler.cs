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
    private readonly IUnitOfWork _unitOfWork;

    public KichHoatPhuongTienCommandHandler(
        IPhuongTienCommandRepository phuongTienCommandRepository,
        ICanHoCommandRepository canHoCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _phuongTienCommandRepository = phuongTienCommandRepository;
        _canHoCommandRepository = canHoCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(KichHoatPhuongTienCommand request, CancellationToken cancellationToken)
    {
        var phuongTiens = await _phuongTienCommandRepository.GetPhuongTiensByIdsAsync(request.PhuongTienIds, cancellationToken);
        
        if (phuongTiens.Count == 0)
        {
            return PhuongTienErrors.NotFound;
        }

        // Nhóm theo căn hộ để tối ưu query hạn mức
        var groupedByCanHo = phuongTiens.GroupBy(x => x.CanHoId);

        foreach (var group in groupedByCanHo)
        {
            var canHoId = group.Key;
            var canHo = await _canHoCommandRepository.GetByIdAsync(canHoId, cancellationToken);
            
            if (canHo == null) continue;

            // Lấy tất cả phương tiện hiện có của căn hộ này (để check quota)
            var allVehicles = await _phuongTienCommandRepository.GetPhuongTiensByCanHoIdAsync(canHoId, cancellationToken);
            var activeVehicles = allVehicles.Where(v => v.TrangThaiPhuongTienId == TrangThaiPhuongTien.Active);
            
            foreach (var phuongTien in group)
            {
                // Logic moved from VehicleRegistryService
                var currentCount = activeVehicles.Count(v => v.LoaiPhuongTienId == phuongTien.LoaiPhuongTienId && v.Id != phuongTien.Id);
                var quota = PhuongTienPolicy.GetQuota(canHo.LoaiCanHoId, phuongTien.LoaiPhuongTienId);

                if (currentCount >= quota)
                {
                    return PhuongTienErrors.OverQuota(canHo.LoaiCanHoId, phuongTien.LoaiPhuongTienId, quota);
                }

                phuongTien.Activate();
                _phuongTienCommandRepository.Update(phuongTien);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
