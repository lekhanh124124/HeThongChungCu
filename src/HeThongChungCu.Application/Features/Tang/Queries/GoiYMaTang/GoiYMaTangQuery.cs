using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.Tang.Queries.GoiYMaTang;

public record GoiYMaTangQuery(int ToaNhaId, int LoaiTangId) : IQuery<string>;
