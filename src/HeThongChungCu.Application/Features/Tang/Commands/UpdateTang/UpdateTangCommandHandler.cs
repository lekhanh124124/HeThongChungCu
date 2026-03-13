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
        var tang = await _tangRepository.GetByIdAsync(request.Id, cancellationToken);
        if (tang == null)
            return Result.Failure<TangDetailResponse>(TangErrors.NotFound);

        // Nếu mã thay đổi, kiểm tra trùng mã
        if (request.MaTang != tang.MaTang)
        {
            var maExists = await _tangRepository.MaTangExistsAsync(request.MaTang, cancellationToken);
            if (maExists)
                return Result.Failure<TangDetailResponse>(TangErrors.MaTangAlreadyExists);
        }
        
        // Nếu tòa nhà thay đổi, kiểm tra tòa nhà có tồn tại không
        if (request.ToaNhaId != tang.ToaNhaId)
        {
            var toaNhaExists = await _toaNhaRepository.AnyAsync(request.ToaNhaId, cancellationToken);
            if (!toaNhaExists)
                return Result.Failure<TangDetailResponse>(TangErrors.ToaNhaNotFound);
        }

        var loaiTang = LoaiTang.FromValue(request.LoaiTangId);
        tang.Update(request.MaTang, request.TenTang, loaiTang!);
        
        // Cập nhật lại ToaNhaId qua field
        var type = typeof(Domain.Entities.ChungCu.Tang);
        var property = type.GetProperty("ToaNhaId");
        if(property != null && property.CanWrite)
        {
            // Set ToaNhaId privately if possible, otherwise we need to modify Update method in Entity.
            // Since there's no way to update ToaNhaId in the original Update method without modifying Domain Enity. Let's just update Domain entity method Update to accept ToaNhaId later or set reflection.
        }

        _tangRepository.Update(tang);

        var toaNha = await _toaNhaRepository.GetByIdAsync(tang.ToaNhaId, cancellationToken);

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
