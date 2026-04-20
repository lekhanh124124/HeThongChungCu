using HeThongChungCu.Application.Features.CuDan.DTOs;

namespace HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru
{
    public class LayThanhVienCuTruQueryHandler : IQueryHandler<LayThanhVienCuTruQuery, List<ThanhVienCuTruResponse>>
    {
        private readonly IQuanHeCuTruQueryRepository _quanHeCuTruQueryRepository;

        public LayThanhVienCuTruQueryHandler(IQuanHeCuTruQueryRepository quanHeCuTruQueryRepository)
        {
            _quanHeCuTruQueryRepository = quanHeCuTruQueryRepository;
        }

        public async Task<Result<List<ThanhVienCuTruResponse>>> Handle(LayThanhVienCuTruQuery request, CancellationToken cancellationToken)
        {
            var spec = new LayThanhVienCuTruSpecification(request.CanHoId);
            var result = await _quanHeCuTruQueryRepository.LayThanhVienCuTru(spec, cancellationToken);
            return result.ToList();
        }
    }
}
