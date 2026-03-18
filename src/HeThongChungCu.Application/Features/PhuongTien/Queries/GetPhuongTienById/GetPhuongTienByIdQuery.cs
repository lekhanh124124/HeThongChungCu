using HeThongChungCu.Application.Features.PhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.PhuongTien.Queries.GetPhuongTienById;

public record GetPhuongTienByIdQuery(int Id) : IQuery<PhuongTienResponse>;


