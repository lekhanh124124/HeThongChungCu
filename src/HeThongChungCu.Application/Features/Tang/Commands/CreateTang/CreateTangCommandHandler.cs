using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Commands.CreateTang;

public class CreateTangCommandHandler : ICommandHandler<CreateTangCommand, TangDetailResponse>
{
    private readonly IToaNhaCommandRepository _toaNhaRepository;

    public CreateTangCommandHandler(
        IToaNhaCommandRepository toaNhaRepository)
    {
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<TangDetailResponse>> Handle(CreateTangCommand request, CancellationToken cancellationToken)
    {
        var toaNha = await _toaNhaRepository.GetToaNhaByIdAsync(request.ToaNhaId, cancellationToken);
        if (toaNha == null)
            return TangErrors.ToaNhaNotFound;
            
        var loaiTang = LoaiTang.FromValue(request.LoaiTangId);

        var tang = toaNha.AddTang(request.MaTang, request.TenTang, loaiTang!);
        _toaNhaRepository.Update(toaNha);

        return Result.Success(new TangDetailResponse
        {
            Id = tang.Id,
            MaTang = tang.MaTang,
            TenTang = tang.TenTang,
            LoaiTangId = tang.LoaiTangId.Value,
            TenLoaiTang = tang.LoaiTangId.Name,
            ToaNhaId = tang.ToaNhaId,
            TenToaNha = toaNha.TenToaNha
        });
    }
}
