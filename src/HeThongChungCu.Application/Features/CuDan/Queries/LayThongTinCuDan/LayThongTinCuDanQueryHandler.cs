using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
using HeThongChungCu.Application.Features.CuDan.DTOs;
using HeThongChungCu.Application.Features.Profile.Queries.GetProfile;

namespace HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;

public class LayThongTinCuDanQueryHandler : IQueryHandler<LayThongTinCuDanQuery, LayThongTinCuDanResponse>
{
    private readonly INguoiDungDapperRepository _nguoiDungRepository;
    private readonly IQuanHeCuTruDapperRepository _quanHeCuTruRepository;

    public LayThongTinCuDanQueryHandler(
        INguoiDungDapperRepository nguoiDungRepository, 
        IQuanHeCuTruDapperRepository quanHeCuTruRepository)
    {
        _nguoiDungRepository = nguoiDungRepository;
        _quanHeCuTruRepository = quanHeCuTruRepository;
    }

    public async Task<Result<LayThongTinCuDanResponse>> Handle(LayThongTinCuDanQuery request, CancellationToken cancellationToken)
    {
        var spec = new LayThongTinCuDanSpecification(request.UserId, request.QuanHeCuTruId);
        var result = await _quanHeCuTruRepository.GetByIdAsync(spec, cancellationToken);
        
        if (result is null)
        {
            return Result.Failure<LayThongTinCuDanResponse>(AuthErrors.InvalidCredentials);
        }

        return Result.Success(result);
    }
}
