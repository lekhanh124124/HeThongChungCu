using HeThongChungCu.Application.Common.Messaging;

namespace HeThongChungCu.Application.Features.QLPhuongTien.Queries.GoiYMaThePhuongTien;

public record GoiYMaThePhuongTienQuery(int PhuongTienId) : IQuery<string>;
