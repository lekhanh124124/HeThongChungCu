using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Commands.CreateTang;

public class CreateTangCommandHandler : ICommandHandler<CreateTangCommand, TangDetailResponse>
{
    private readonly ITangEFRepository _tangRepository;
    private readonly IToaNhaEFRepository _toaNhaRepository;

    public CreateTangCommandHandler(
        ITangEFRepository tangRepository,
        IToaNhaEFRepository toaNhaRepository)
    {
        _tangRepository = tangRepository;
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<TangDetailResponse>> Handle(CreateTangCommand request, CancellationToken cancellationToken)
    {
        var toaNha = await _toaNhaRepository.GetByIdAsync(request.ToaNhaId, cancellationToken);
        if (toaNha == null)
            return Result.Failure<TangDetailResponse>(TangErrors.ToaNhaNotFound);

        var maExists = await _tangRepository.MaTangExistsAsync(request.MaTang, cancellationToken);
        if (maExists)
            return Result.Failure<TangDetailResponse>(TangErrors.MaTangAlreadyExists);

        var loaiTang = LoaiTang.FromValue(request.LoaiTangId);

        var tang = new Domain.Entities.ChungCu.Tang(
            request.MaTang,
            request.TenTang,
            loaiTang!.Value,
            request.ToaNhaId);

        await _tangRepository.AddAsync(tang, cancellationToken);

        return Result.Success(new TangDetailResponse
        {
            Id = tang.Id,
            MaTang = tang.MaTang,
            TenTang = tang.TenTang,
            LoaiTangId = tang.LoaiTangId,
            TenLoaiTang = LoaiTang.FromValue(tang.LoaiTangId)?.Name ?? string.Empty,
            ToaNhaId = tang.ToaNhaId,
            TenToaNha = toaNha.TenToaNha
        });
    }
}
