using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using System;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoThuChi;

public record GetBaoCaoThuChiQuery : IQuery<BaoCaoThuChiResponse>
{
    public DateTimeOffset TuNgay { get; init; }
    public DateTimeOffset DenNgay { get; init; }
}
