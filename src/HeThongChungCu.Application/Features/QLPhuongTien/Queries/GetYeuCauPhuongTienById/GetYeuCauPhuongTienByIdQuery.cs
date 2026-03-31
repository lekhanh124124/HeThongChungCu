using HeThongChungCu.Application.Features.QLPhuongTien.DTOs;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GetYeuCauPhuongTienById;

public record GetYeuCauPhuongTienByIdQuery(int RequestId) : IQuery<YeuCauPhuongTienResponse>;
