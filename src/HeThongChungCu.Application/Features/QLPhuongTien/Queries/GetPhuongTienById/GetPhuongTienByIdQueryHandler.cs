using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetPhuongTienById;

public class GetPhuongTienByIdQueryHandler : IQueryHandler<GetPhuongTienByIdQuery, PhuongTienResponse>
{
    private readonly IPhuongTienDapperRepository _phuongTienDapperRepository;

    public GetPhuongTienByIdQueryHandler(IPhuongTienDapperRepository phuongTienDapperRepository)
    {
        _phuongTienDapperRepository = phuongTienDapperRepository;
    }

    public async Task<Result<PhuongTienResponse>> Handle(GetPhuongTienByIdQuery request, CancellationToken cancellationToken)
    {
        var phuongTien = await _phuongTienDapperRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (phuongTien == null)
            return Result.Failure<PhuongTienResponse>(PhuongTienErrors.NotFound);

        return Result.Success(phuongTien);
    }
}
