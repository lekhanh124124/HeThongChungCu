using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLPhanAnh.DTOs;

namespace HeThongChungCu.Application.Features.QLPhanAnh.Queries.GetPhanAnhById;

public record GetPhanAnhByIdQuery(int Id) : IQuery<PhanAnhDetailResponse>;
