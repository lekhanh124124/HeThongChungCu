using HeThongChungCu.Application.Features.ToaNha.DTOs;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.DeleteToaNha;

public class DeleteToaNhaCommandHandler : ICommandHandler<DeleteToaNhaCommand, IReadOnlyList<ToaNhaDetailResponse>>
{
    private readonly IToaNhaEFRepository _toaNhaRepository;

    public DeleteToaNhaCommandHandler(IToaNhaEFRepository toaNhaRepository)
    {
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<IReadOnlyList<ToaNhaDetailResponse>>> Handle(DeleteToaNhaCommand request, CancellationToken cancellationToken)
    {
        var toaNhas = await _toaNhaRepository.GetByIdsAsync(request.Ids, cancellationToken);

        var notFoundIds = request.Ids.Except(toaNhas.Select(t => t.Id)).ToList();
        if (notFoundIds.Count > 0)
        {
            var ids = string.Join(", ", notFoundIds);
            return Result.Failure<IReadOnlyList<ToaNhaDetailResponse>>(new Error(
                "ToaNha.NotFound",
                $"Không tìm thấy tòa nhà với ID: {ids}."));
        }

        var response = toaNhas.Select(t => new ToaNhaDetailResponse
        {
            Id = t.Id,
            MaToaNha = t.MaToaNha,
            TenToaNha = t.TenToaNha,
            DiaChi = t.DiaChi,
            MoTa = t.MoTa,
            TrangThaiToaNhaId = t.TrangThaiToaNhaId.Value,
            TenTrangThaiToaNha = t.TrangThaiToaNhaId.Name
        }).ToList();

        foreach (var toaNha in toaNhas)
        {
            _toaNhaRepository.Remove(toaNha);
        }

        return Result.Success<IReadOnlyList<ToaNhaDetailResponse>>(response);
    }
}
