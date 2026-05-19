using System;
using System.Collections.Generic;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Application.Features.QLSystem.Queries.GetBackupHistory;

public class GetBackupHistorySpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id",
        "FileName",
        "CreatedAt",
        "Size"
    };

    public GetBackupHistorySpecification(
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize)
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("LoaiTepId", FilterOperator.Equal, LoaiTepTaiLieu.SaoLuuDb.Value);
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("FileName", FilterOperator.Contains, keyword);
        }
    }
}
