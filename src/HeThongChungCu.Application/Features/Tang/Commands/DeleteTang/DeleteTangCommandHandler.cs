using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.Tang.Commands.DeleteTang;

public class DeleteTangCommandHandler : ICommandHandler<DeleteTangCommand, IReadOnlyList<TangDetailResponse>>
{
    private readonly ITangEFRepository _tangRepository;
    private readonly IToaNhaEFRepository _toaNhaRepository;

    public DeleteTangCommandHandler(ITangEFRepository tangRepository, IToaNhaEFRepository toaNhaRepository)
    {
        _tangRepository = tangRepository;
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<IReadOnlyList<TangDetailResponse>>> Handle(DeleteTangCommand request, CancellationToken cancellationToken)
    {
        var tangs = await _tangRepository.GetByIdsAsync(request.Ids, cancellationToken);
        
        if (!tangs.Any())
            return Result.Failure<IReadOnlyList<TangDetailResponse>>(TangErrors.NotFound);

        var deletedTangs = new List<TangDetailResponse>();

        foreach (var tang in tangs)
        {
            // Soft delete
            _tangRepository.Remove(tang);

            var toaNha = await _toaNhaRepository.GetByIdAsync(tang.ToaNhaId, cancellationToken);

            deletedTangs.Add(new TangDetailResponse
            {
                Id = tang.Id,
                MaTang = tang.MaTang,
                TenTang = tang.TenTang,
                LoaiTangId = tang.LoaiTangId,
                TenLoaiTang = LoaiTang.FromValue(tang.LoaiTangId)?.Name ?? string.Empty,
                ToaNhaId = tang.ToaNhaId,
                TenToaNha = toaNha?.TenToaNha ?? string.Empty
            });
        }

        return Result.Success<IReadOnlyList<TangDetailResponse>>(deletedTangs);
    }
}
