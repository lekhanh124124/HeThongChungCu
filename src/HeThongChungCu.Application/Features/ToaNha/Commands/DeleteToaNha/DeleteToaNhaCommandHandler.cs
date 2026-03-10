using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.ToaNha.DTOs;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.ToaNha.Commands.DeleteToaNha;

public class DeleteToaNhaCommandHandler : ICommandHandler<DeleteToaNhaCommand, IReadOnlyList<ToaNhaDetailResponse>>
{
    private readonly IToaNhaEFRepository _toaNhaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteToaNhaCommandHandler(IToaNhaEFRepository toaNhaRepository, IUnitOfWork unitOfWork)
    {
        _toaNhaRepository = toaNhaRepository;
        _unitOfWork = unitOfWork;
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
                $"KhÃ´ng tÃ¬m tháº¥y tÃ²a nhÃ  vá»›i ID: {ids}."));
        }

        var response = toaNhas.Select(t => new ToaNhaDetailResponse
        {
            Id = t.Id,
            MaToaNha = t.MaToaNha,
            TenToaNha = t.TenToaNha,
            SoTang = t.SoTang,
            SoTangHam = t.SoTangHam,
            DiaChi = t.DiaChi,
            MoTa = t.MoTa,
            TrangThaiToaNhaId = TrangThaiToaNha.FromValue(t.TrangThaiToaNhaId)?.Value ?? 0,
            TenTrangThaiToaNha = TrangThaiToaNha.FromValue(t.TrangThaiToaNhaId)?.Name ?? string.Empty
        }).ToList();

        foreach (var toaNha in toaNhas)
        {
            _toaNhaRepository.Remove(toaNha);
        }

        return Result.Success<IReadOnlyList<ToaNhaDetailResponse>>(response);
    }
}
