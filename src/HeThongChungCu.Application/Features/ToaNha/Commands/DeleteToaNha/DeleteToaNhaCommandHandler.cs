using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.DeleteToaNha;

public class DeleteToaNhaCommandHandler : ICommandHandler<DeleteToaNhaCommand, IReadOnlyList<ToaNhaDetailResponse>>
{
    private readonly IToaNhaCommandRepository _toaNhaRepository;

    public DeleteToaNhaCommandHandler(IToaNhaCommandRepository toaNhaRepository)
    {
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<IReadOnlyList<ToaNhaDetailResponse>>> Handle(DeleteToaNhaCommand request, CancellationToken cancellationToken)
    {
        var toaNhas = await _toaNhaRepository.GetToaNhaByIdsAsync(request.Ids, cancellationToken);

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
            DiaChi = t.DiaChi.FullAddress,
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
