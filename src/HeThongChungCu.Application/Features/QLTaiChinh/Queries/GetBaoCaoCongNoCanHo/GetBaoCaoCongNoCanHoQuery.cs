using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoCanHo;

public record GetBaoCaoCongNoCanHoQuery : IQuery<List<BaoCaoCongNoCanHoResponse>>
{
    public int? ToaNhaId { get; init; }
    public int Thang { get; init; }
    public int Nam { get; init; }
}
