using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Commands.DeleteTang;

public class DeleteTangCommandHandler : ICommandHandler<DeleteTangCommand, IReadOnlyList<TangDetailResponse>>
{
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly HeThongChungCu.Application.Common.Interfaces.Persistences.Commands.ICanHoCommandRepository _canHoRepository;

    public DeleteTangCommandHandler(IToaNhaCommandRepository toaNhaRepository, HeThongChungCu.Application.Common.Interfaces.Persistences.Commands.ICanHoCommandRepository canHoRepository)
    {
        _toaNhaRepository = toaNhaRepository;
        _canHoRepository = canHoRepository;
    }

    public async Task<Result<IReadOnlyList<TangDetailResponse>>> Handle(DeleteTangCommand request, CancellationToken cancellationToken)
    {
        var tangs = await _toaNhaRepository.GetTangByIdsAsync(request.Ids, cancellationToken);
        
        if (!tangs.Any())
            return TangErrors.NotFound;

        var hasCanHo = await _canHoRepository.AnyByTangIdsAsync(request.Ids, cancellationToken);
        if (hasCanHo)
        {
            return Result.Failure<IReadOnlyList<TangDetailResponse>>(new Error(
                "Tang.HasCanHo",
                "Không thể xóa tầng đã có căn hộ."));
        }

        var deletedTangs = new List<TangDetailResponse>();

        foreach (var tang in tangs)
        {
            _toaNhaRepository.Remove(tang);
            
            deletedTangs.Add(new TangDetailResponse
            {
                Id = tang.Id,
                MaTang = tang.MaTang,
                TenTang = tang.TenTang,
                LoaiTangId = tang.LoaiTangId.Value,
                TenLoaiTang = tang.LoaiTangId.Name,
                ToaNhaId = tang.ToaNhaId,
                TenToaNha = tang.ToaNha?.TenToaNha ?? string.Empty
            });
        }

        return Result.Success<IReadOnlyList<TangDetailResponse>>(deletedTangs);
    }
}
