using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.CanHo.DTOs;

namespace HeThongChungCu.Application.Features.CanHo.Commands.DeleteCanHo;

public class DeleteCanHoCommandHandler : ICommandHandler<DeleteCanHoCommand, IReadOnlyList<CanHoDetailResponse>>
{
    private readonly ICanHoEFRepository _canHoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCanHoCommandHandler(ICanHoEFRepository canHoRepository, IUnitOfWork unitOfWork)
    {
        _canHoRepository = canHoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<CanHoDetailResponse>>> Handle(DeleteCanHoCommand request, CancellationToken cancellationToken)
    {
        var canHos = await _canHoRepository.GetByIdsAsync(request.Ids, cancellationToken);

        var notFoundIds = request.Ids.Except(canHos.Select(c => c.Id)).ToList();
        if (notFoundIds.Count > 0)
        {
            var ids = string.Join(", ", notFoundIds);
            return Result.Failure<IReadOnlyList<CanHoDetailResponse>>(new Error(
                "CanHo.NotFound",
                $"Không tìm thấy căn hộ với ID: {ids}."));
        }

        var response = canHos.Select(c => new CanHoDetailResponse
        {
            Id = c.Id,
            ToaNhaId = c.ToaNhaId,
            MaCanHo = c.MaCanHo,
            DienTich = c.DienTich,
            Tang = c.Tang,
            SoPhongNgu = c.SoPhongNgu,
            SoPhongTam = c.SoPhongTam,
            LoaiCanHoId = c.LoaiCanHoId,
            TenLoaiCanHo = LoaiCanHo.FromValue(c.LoaiCanHoId)?.Name ?? string.Empty,
            TinhTrangCanHoId = c.TinhTrangCanHoId,
            TenTinhTrangCanHo = TinhTrangCanHo.FromValue(c.TinhTrangCanHoId)?.Name ?? string.Empty
        }).ToList();

        foreach (var canHo in canHos)
        {
            _canHoRepository.Remove(canHo);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<IReadOnlyList<CanHoDetailResponse>>(response);
    }
}
