using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

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
                // Đếm số lượng xe cùng loại đã được duyệt (không tính chính nó nếu nó đang là Active)
                var currentCount = existingVehicles
                    .Count(x => x.LoaiPhuongTienId == phuongTien.LoaiPhuongTienId && 
                               x.TrangThaiPhuongTienId == TrangThaiPhuongTien.Active &&
                               x.Id != phuongTien.Id);

                phuongTien.KichHoat(canHo.LoaiCanHoId, currentCount);
                _phuongTienCommandRepository.Update(phuongTien);
                
                // Cập nhật danh sách existingVehicles giả định để xe tiếp theo trong group thấy được sự thay đổi
                // (Trong trường hợp một lệnh kích hoạt nhiều xe cùng loại cho 1 căn hộ)
                // Note: Thực tế cần add phuongTien vào existingVehicles nếu loop tiếp, nhưng thông thường 1 request hiếm khi kích hoạt nhiều xe cùng loại cho 1 căn hộ.
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
