using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.QLNhanVien.DTOs;

namespace HeThongChungCu.Application.Features.QLNhanVien.Queries.GetNhanVienById;

public record GetNhanVienByIdQuery(int Id) : IQuery<NhanVienDetailResponse>;
