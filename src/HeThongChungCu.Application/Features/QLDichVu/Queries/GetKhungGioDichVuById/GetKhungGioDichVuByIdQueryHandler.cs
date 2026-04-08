using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLDichVu.DTOs;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLDichVu.Queries.GetKhungGioDichVuById;

public class GetKhungGioDichVuByIdQueryHandler : IQueryHandler<GetKhungGioDichVuByIdQuery, KhungGioDichVuResponse>
{
    private readonly IDichVuQueryRepository _queryRepository;

    public GetKhungGioDichVuByIdQueryHandler(IDichVuQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<Result<KhungGioDichVuResponse>> Handle(GetKhungGioDichVuByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetKhungGioDichVuByIdSpecification(request.Id);
        var result = await _queryRepository.GetKhungGioByIdAsync(spec, cancellationToken);
        
        if (result == null)
            return Result.Failure<KhungGioDichVuResponse>(Error.NotFound("KhungGioDichVu.NotFound", $"Không tìm thấy khung giờ dịch vụ với ID {request.Id}"));

        return Result.Success(result);
    }
}
