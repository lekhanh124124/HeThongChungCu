using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetQuyThuChiById;

public record GetQuyThuChiByIdQuery(int Id) : IQuery<QuyThuChiResponse>;
