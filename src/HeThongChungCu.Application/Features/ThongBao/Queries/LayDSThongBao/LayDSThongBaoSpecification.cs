using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.ThongBao.Queries.LayDSThongBao;

public class LayDSThongBaoSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "CreatedAt",
        "TieuDe",
        "IsRead"
    };

    public LayDSThongBaoSpecification(
        int userId,
        bool? onlyUnread,
        string? keyword,
        string? sortCol,
        bool? isAsc,
        int? pageNumber,
        int? pageSize) 
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter("UserId", FilterOperator.Equal, userId);

        if (onlyUnread == true)
        {
            AddFilter("IsRead", FilterOperator.Equal, false);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("TieuDe", FilterOperator.Contains, keyword);
            AddKeyword("NoiDung", FilterOperator.Contains, keyword);
        }
    }
}
