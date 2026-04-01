using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetPhuongTienById;

public class GetPhuongTienByIdQueryHandler : IQueryHandler<GetPhuongTienByIdQuery, PhuongTienResponse>
{
    private readonly IPhuongTienQueryRepository _phuongTienQueryRepository;

    public GetPhuongTienByIdQueryHandler(IPhuongTienQueryRepository phuongTienQueryRepository)
    {
        _phuongTienQueryRepository = phuongTienQueryRepository;
    }

    public async Task<Result<PhuongTienResponse>> Handle(GetPhuongTienByIdQuery request, CancellationToken cancellationToken)
    {
        var phuongTien = await _phuongTienQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (phuongTien == null)
            return Result.Failure<PhuongTienResponse>(PhuongTienErrors.NotFound);

        return Result.Success(phuongTien);
    }
}
