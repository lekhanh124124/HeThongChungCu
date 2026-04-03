using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.NhanVien.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienById;

public class GetNhanVienByIdQueryHandler : IQueryHandler<GetNhanVienByIdQuery, NhanVienResponse>
{
    private readonly INhanVienQueryRepository _nhanVienQueryRepository;

    public GetNhanVienByIdQueryHandler(INhanVienQueryRepository nhanVienQueryRepository)
    {
        _nhanVienQueryRepository = nhanVienQueryRepository;
    }

    public async Task<Result<NhanVienResponse>> Handle(GetNhanVienByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetNhanVienByIdSpecification(request.Id);
        var response = await _nhanVienQueryRepository.GetByIdAsync(spec, cancellationToken);
        
        return response != null 
            ? Result.Success(response) 
            : Result.Failure<NhanVienResponse>(NhanVienErrors.NotFoundById(request.Id));
    }
}
