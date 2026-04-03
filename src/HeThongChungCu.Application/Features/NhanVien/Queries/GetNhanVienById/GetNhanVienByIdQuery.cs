using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.NhanVien.DTOs;

namespace HeThongChungCu.Application.Features.NhanVien.Queries.GetNhanVienById;

public record GetNhanVienByIdQuery(int Id) : IQuery<NhanVienResponse>;
