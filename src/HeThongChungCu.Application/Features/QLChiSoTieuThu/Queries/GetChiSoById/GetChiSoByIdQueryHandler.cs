using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.GetChiSoById;

public class GetChiSoByIdQueryHandler : IQueryHandler<GetChiSoByIdQuery, ChiSoDetailResponse>
{
    private readonly IChiSoTieuThuQueryRepository _repository;

    public GetChiSoByIdQueryHandler(IChiSoTieuThuQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ChiSoDetailResponse>> Handle(GetChiSoByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (result == null)
        {
            return Result.Failure<ChiSoDetailResponse>(new Error("ChiSo.NotFound", "Không tìm thấy chỉ số tiêu thụ."));
        }

        return Result.Success(result);
    }
}
