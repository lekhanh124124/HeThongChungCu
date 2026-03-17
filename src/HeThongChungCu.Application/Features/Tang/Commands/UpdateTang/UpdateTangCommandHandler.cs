using HeThongChungCu.Application.Features.Tang.DTOs;

namespace HeThongChungCu.Application.Features.Tang.Commands.UpdateTang;

public class UpdateTangCommandHandler : ICommandHandler<UpdateTangCommand, TangDetailResponse>
{
    private readonly ITangEFRepository _tangRepository;
    private readonly IToaNhaEFRepository _toaNhaRepository;

    public UpdateTangCommandHandler(
        ITangEFRepository tangRepository,
        IToaNhaEFRepository toaNhaRepository)
    {
        _tangRepository = tangRepository;
        _toaNhaRepository = toaNhaRepository;
    }

    public async Task<Result<TangDetailResponse>> Handle(UpdateTangCommand request, CancellationToken cancellationToken)
    {
        var toaNha = await _toaNhaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (toaNha == null)
            return Result.Failure<TangDetailResponse>(TangErrors.ToaNhaNotFound);

        var tang = toaNha.Tangs.FirstOrDefault(t => t.Id == request.Id);
        if (tang == null)
            return Result.Failure<TangDetailResponse>(TangErrors.NotFound);

        // Nếu mã thay đổi, kiểm tra trùng mã
        if (request.MaTang != tang.MaTang)
        {
            var maExists = toaNha.Tangs.Any(t => t.MaTang == request.MaTang);
            if (maExists)
                return Result.Failure<TangDetailResponse>(TangErrors.MaTangAlreadyExists);
        }
        
        var loaiTang = LoaiTang.FromValue(request.LoaiTangId);
        tang.Update(request.MaTang, request.TenTang, loaiTang!);

        _tangRepository.Update(tang);


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
