using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.DTOs;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoToaNha;

public record GetBaoCaoCongNoToaNhaQuery : IQuery<List<BaoCaoCongNoToaNhaResponse>>
{
    public int Thang { get; init; }
    public int Nam { get; init; }
}
