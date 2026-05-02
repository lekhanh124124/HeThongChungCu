using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.RecordChiSoBatch;

public class RecordChiSoBatchCommandHandler : ICommandHandler<RecordChiSoBatchCommand, int>
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RecordChiSoBatchCommandHandler(IChiSoTieuThuCommandRepository chiSoRepository, IUnitOfWork unitOfWork)
    {
        _chiSoRepository = chiSoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(RecordChiSoBatchCommand request, CancellationToken cancellationToken)
    {
        var newChiSos = new List<ChiSoTieuThu>();

        foreach (var item in request.Items)
        {
            var chiSo = ChiSoTieuThu.Create(
                item.CanHoId,
                item.DichVuId,
                item.ChiSoCu,
                item.ChiSoMoi,
                request.Thang,
                request.Nam,
                request.NgayGhiNhan,
                item.AnhDongHoId,
                item.GhiChu
            );

            newChiSos.Add(chiSo);
        }

        if (newChiSos.Count > 0)
        {
            await _chiSoRepository.AddRangeAsync(newChiSos, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(newChiSos.Count);
    }
}
