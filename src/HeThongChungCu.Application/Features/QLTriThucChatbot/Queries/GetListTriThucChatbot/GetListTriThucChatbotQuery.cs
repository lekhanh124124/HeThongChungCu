using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetListTriThucChatbot;

public record GetListTriThucChatbotQuery(
    string? DanhMuc,
    bool? IsActive,
    bool? IsSynced,
    string? Keyword,
    int? PageNumber,
    int? PageSize,
    string? SortCol,
    bool? IsAsc) : IQuery<PagedResult<TriThucChatbotResponse>>;
