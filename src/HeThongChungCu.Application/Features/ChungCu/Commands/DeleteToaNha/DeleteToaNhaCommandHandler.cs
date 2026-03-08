using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.ChungCu.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.DeleteToaNha;

public class DeleteToaNhaCommandHandler : ICommandHandler<DeleteToaNhaCommand, IReadOnlyList<ToaNhaResponse>>
{
    private readonly IToaNhaEFRepository _toaNhaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteToaNhaCommandHandler(IToaNhaEFRepository toaNhaRepository, IUnitOfWork unitOfWork)
    {
        _toaNhaRepository = toaNhaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<ToaNhaResponse>>> Handle(DeleteToaNhaCommand request, CancellationToken cancellationToken)
    {
        var toaNhas = await _toaNhaRepository.GetByIdsAsync(request.Ids, cancellationToken);

        var notFoundIds = request.Ids.Except(toaNhas.Select(t => t.Id)).ToList();
        if (notFoundIds.Count > 0)
        {
            var ids = string.Join(", ", notFoundIds);
            return Result.Failure<IReadOnlyList<ToaNhaResponse>>(new Error(
                "ToaNha.NotFound",
                $"Không tìm thấy tòa nhà với ID: {ids}."));
        }

        var response = toaNhas.Select(t => new ToaNhaResponse
        {
            Id = t.Id,
            MaToaNha = t.MaToaNha,
            TenToaNha = t.TenToaNha,
            SoTang = t.SoTang
        }).ToList();

        foreach (var toaNha in toaNhas)
        {
            _toaNhaRepository.Remove(toaNha);
        }

        return Result.Success<IReadOnlyList<ToaNhaResponse>>(response);
    }
}
