using HeThongChungCu.Application.Features.CuDan.DTOs;

namespace HeThongChungCu.Application.Features.CuDan.Queries.LayThanhVienCuTru
{
    public record LayThanhVienCuTruQuery(int CanHoId) : IQuery<List<ThanhVienCuTruResponse>>;
}
