using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Queries.GetYeuCauCuTruById;

public class GetYeuCauCuTruByIdQueryHandler : IQueryHandler<GetYeuCauCuTruByIdQuery, YeuCauCuTruResponse>
{
    private readonly IYeuCauCuTruEFRepository _yeuCauRepository;

    public GetYeuCauCuTruByIdQueryHandler(IYeuCauCuTruEFRepository yeuCauRepository)
    {
        _yeuCauRepository = yeuCauRepository;
    }

    public async Task<Result<YeuCauCuTruResponse>> Handle(GetYeuCauCuTruByIdQuery request, CancellationToken cancellationToken)
    {
        var yeuCau = await _yeuCauRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (yeuCau == null)
            return Result.Failure<YeuCauCuTruResponse>(GeneralErrors.NotFoundById(request.RequestId));

        var response = new YeuCauCuTruResponse
        {
            Id = yeuCau.Id,
            CanHoId = yeuCau.CanHoId,
            LoaiYeuCauId = yeuCau.LoaiYeuCauId.Value,
            TenLoaiYeuCau = yeuCau.LoaiYeuCauId.Name,
            TrangThaiId = yeuCau.TrangThaiId.Value,
            TenTrangThai = yeuCau.TrangThaiId.Name,
            Reason = yeuCau.LyDo,
            NoiDung = yeuCau.NoiDung,
            CreatedAt = yeuCau.CreatedAt,
            QuanHeCuTruId = yeuCau.YeuCauLoaiQuanHeId,
        };

        return response;
    }
}
