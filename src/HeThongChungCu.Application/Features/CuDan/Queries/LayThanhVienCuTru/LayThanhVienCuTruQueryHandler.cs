using HeThongChungCu.Application.Features.CuDan.DTOs;

namespace HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru
{
    public class LayThanhVienCuTruQueryHandler : IQueryHandler<LayThanhVienCuTruQuery, IReadOnlyList<ThanhVienCuTruResponse>>
    {
        private readonly IQuanHeCuTruDapperRepository _quanHeCuTruDapperRepository;

        public LayThanhVienCuTruQueryHandler(IQuanHeCuTruDapperRepository quanHeCuTruDapperRepository)
        {
            _quanHeCuTruDapperRepository = quanHeCuTruDapperRepository;
        }

        public async Task<Result<IReadOnlyList<ThanhVienCuTruResponse>>> Handle(LayThanhVienCuTruQuery request, CancellationToken cancellationToken)
        {
            var spec = new LayThanhVienCuTruSpecification(request.CanHoId);
            var result = await _quanHeCuTruDapperRepository.LayThanhVienCuTru(spec, cancellationToken);
            return Result.Success(result);
        }
    }
}
