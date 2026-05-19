using System;
using System.Collections.Generic;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLSystem.DTOs;

namespace HeThongChungCu.Application.Features.QLSystem.Queries.GetBackupHistory;


public record GetBackupHistoryQuery(
    string? Keyword = null,
    string? SortCol = null,
    bool? IsAsc = false,
    int? PageNumber = 1,
    int? PageSize = 20) : IQuery<PagedResult<BackupHistoryResponse>>;
