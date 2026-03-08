using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.ChungCu.DTOs;

namespace HeThongChungCu.Application.Features.ChungCu.Commands.CreateToaNha;

public class CreateToaNhaCommandHandler : ICommandHandler<CreateToaNhaCommand, ToaNhaResponse>
{
    private readonly IToaNhaEFRepository _toaNhaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateToaNhaCommandHandler(IToaNhaEFRepository toaNhaRepository, IUnitOfWork unitOfWork)
    {
        _toaNhaRepository = toaNhaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ToaNhaResponse>> Handle(CreateToaNhaCommand request, CancellationToken cancellationToken)
    {
        var exists = await _toaNhaRepository.MaToaNhaExistsAsync(request.MaToaNha, cancellationToken);
        if (exists)
            return Result.Failure<ToaNhaResponse>(ToaNhaErrors.MaToaNhaAlreadyExists);

        var toaNha = new ToaNha(request.MaToaNha, request.TenToaNha, request.SoTang);
        await _toaNhaRepository.AddAsync(toaNha, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ToaNhaResponse
        {
            Id = toaNha.Id,
            MaToaNha = toaNha.MaToaNha,
            TenToaNha = toaNha.TenToaNha,
            SoTang = toaNha.SoTang
        });
    }
}
