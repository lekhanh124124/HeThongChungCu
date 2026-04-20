using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Commands.DeleteTang;

public class DeleteTangCommandHandler : ICommandHandler<DeleteTangCommand, IReadOnlyList<TangDetailResponse>>
{
    private readonly IToaNhaCommandRepository _toaNhaRepository;

    public DeleteTangCommandHandler(IToaNhaCommandRepository toaNhaRepository)
    {
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<IReadOnlyList<TangDetailResponse>>> Handle(DeleteTangCommand request, CancellationToken cancellationToken)
    {
        var tangs = await _toaNhaRepository.GetTangByIdsAsync(request.Ids, cancellationToken);
        
        if (!tangs.Any())
            return TangErrors.NotFound;

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
