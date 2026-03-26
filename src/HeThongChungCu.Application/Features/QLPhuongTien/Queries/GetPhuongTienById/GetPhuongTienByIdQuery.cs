using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetPhuongTienById;

public record GetPhuongTienByIdQuery(int Id) : IQuery<PhuongTienResponse>;


