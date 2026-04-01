using HeThongChungCu.Application.Features.CanHo.DTOs;
using HeThongChungCu.Domain.Enums;
namespace HeThongChungCu.Application.Features.CanHo.Commands.DeleteCanHo;

public class DeleteCanHoCommandHandler : ICommandHandler<DeleteCanHoCommand, IReadOnlyList<CanHoDetailResponse>>
{
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuanHeCuTruCommandRepository _quanHeCuTruRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;

    public DeleteCanHoCommandHandler(
        ICanHoCommandRepository canHoRepository,
        IUnitOfWork unitOfWork,
        IQuanHeCuTruCommandRepository quanHeCuTruRepository,
        IToaNhaCommandRepository toaNhaRepository)
    {
        _canHoRepository = canHoRepository;
        _unitOfWork = unitOfWork;
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _toaNhaRepository = toaNhaRepository;
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

        var tangIds = canHos.Select(c => c.TangId).Distinct().ToList();
        var tangs = await _toaNhaRepository.GetTangByIdsAsync(tangIds, cancellationToken);
        var tangDict = tangs.ToDictionary(t => t.Id, t => t.TenTang);

        var response = canHos.Select(c => new CanHoDetailResponse
        {
            Id = c.Id,
            MaCanHo = c.MaCanHo,
            DienTich = c.DienTich,
            TenCanHo = c.TenCanHo,
            TangId = c.TangId,
            TenTang = tangDict.GetValueOrDefault(c.TangId, string.Empty),
            SoPhongNgu = c.SoPhongNgu,
            SoPhongTam = c.SoPhongTam,
            LoaiCanHoId = c.LoaiCanHoId.Value,
            TenLoaiCanHo = c.LoaiCanHoId.Name,
            TinhTrangCanHoId = c.TinhTrangCanHoId.Value,
            TenTinhTrangCanHo = c.TinhTrangCanHoId.Name
        }).ToList();

        foreach (var canHo in canHos)
        {
            var relations = await _quanHeCuTruRepository.GetByCanHoIdAsync(canHo.Id, cancellationToken);
            bool hasActiveResidents = relations.Any(r => r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);

            canHo.Delete(hasActiveResidents);
            _canHoRepository.Remove(canHo);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success<IReadOnlyList<CanHoDetailResponse>>(response);
    }
}
