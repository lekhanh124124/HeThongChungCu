using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Commands.UpdateTang;

public class UpdateTangCommandHandler : ICommandHandler<UpdateTangCommand, TangDetailResponse>
{
    private readonly IToaNhaEFRepository _toaNhaRepository;

    public UpdateTangCommandHandler(
        IToaNhaEFRepository toaNhaRepository)
    {
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<TangDetailResponse>> Handle(UpdateTangCommand request, CancellationToken cancellationToken)
    {
        var toaNha = await _toaNhaRepository.GetToaNhaByIdAsync(request.ToaNhaId, cancellationToken);
        if (toaNha == null)
            return Result.Failure<TangDetailResponse>(TangErrors.NotFound);

        var loaiTang = LoaiTang.FromValue(request.LoaiTangId);
        toaNha.UpdateTang(request.Id, request.MaTang, request.TenTang, loaiTang!);

        var tang = toaNha.Tangs.First(t => t.Id == request.Id);

        _toaNhaRepository.Update(toaNha);


        return Result.Success(new TangDetailResponse
        {
            Id = tang.Id,
            MaTang = tang.MaTang,
            TenTang = tang.TenTang,
            LoaiTangId = tang.LoaiTangId.Value,
            TenLoaiTang = tang.LoaiTangId.Name,
            ToaNhaId = tang.ToaNhaId,
            TenToaNha = toaNha?.TenToaNha ?? string.Empty
        });
    }
}
