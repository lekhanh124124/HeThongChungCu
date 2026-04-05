using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Common.Models;
using MediatR;

namespace HeThongChungCu.Application.Features.ThongBao.Queries.LayDSThongBao;

public record LayDSThongBaoQuery(
    string? Keyword = null,
    string? SortCol = null,
    bool? IsAsc = false,
    int? PageNumber = 1,
    int? PageSize = 10,
    bool? OnlyUnread = null) : IQuery<PagedResult<ThongBaoResponse>>;

public class ThongBaoResponse
{
    public int Id { get; set; }
    public int ThongBaoId { get; set; }
    public string TieuDe { get; set; } = null!;
    public string NoiDung { get; set; } = null!;
    public int LoaiThongBaoId { get; set; }
    public string TenLoaiThongBao { get; set; } = null!;
    public string? ReferenceId { get; set; }
    public string? Metadata { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
}
