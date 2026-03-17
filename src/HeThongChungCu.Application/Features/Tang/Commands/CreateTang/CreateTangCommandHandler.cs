using HeThongChungCu.Application.Features.Tang.DTOs;
using HeThongChungCu.Domain.Policies;

namespace HeThongChungCu.Application.Features.Tang.Commands.CreateTang;

public class CreateTangCommandHandler : ICommandHandler<CreateTangCommand, TangDetailResponse>
{
    private readonly ITangEFRepository _tangRepository;
    private readonly IToaNhaEFRepository _toaNhaRepository;
    private readonly IToaNhaPolicy _toaNhaPolicy;

    public CreateTangCommandHandler(
        ITangEFRepository tangRepository,
        IToaNhaEFRepository toaNhaRepository,
        IToaNhaPolicy toaNhaPolicy)
    {
        _tangRepository = tangRepository;
        _toaNhaRepository = toaNhaRepository;
        _toaNhaPolicy = toaNhaPolicy;
    }

    public async Task<Result<TangDetailResponse>> Handle(CreateTangCommand request, CancellationToken cancellationToken)
    {
        var toaNha = await _toaNhaRepository.GetByIdAsync(request.ToaNhaId, cancellationToken);
        if (toaNha == null)
            return Result.Failure<TangDetailResponse>(TangErrors.ToaNhaNotFound);
            
        var loaiTang = LoaiTang.FromValue(request.LoaiTangId);

        toaNha.AddTang(request.MaTang, request.TenTang, loaiTang!, _toaNhaPolicy);
        _toaNhaRepository.Update(toaNha);
 
        var response = toaNha.Tangs.Where(x => x.MaTang == request.MaTang).FirstOrDefault();
        return Result.Success(new TangDetailResponse
        {
            Id = response!.Id,
            MaTang = response.MaTang,
            TenTang = response.TenTang,
            LoaiTangId = response.LoaiTangId.Value,
            TenLoaiTang = response.LoaiTangId.Name,
            ToaNhaId = response.ToaNhaId,
            TenToaNha = toaNha.TenToaNha
        });
    }
}
