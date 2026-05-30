using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Queries.GetListTriThucChatbot;

public class GetListTriThucChatbotSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "TieuDe", "DanhMuc", "ThuTuHienThi", "IsActive", "IsSynced", "LastSyncedAt", "CreatedAt"
    };

    public GetListTriThucChatbotSpecification(
        string? danhMuc,
        bool? isActive,
        bool? isSynced,
        string? keyword,
        int? pageNumber = 1,
        int? pageSize = 20,
        string? sortBy = null,
        bool? isAsc = true)
        : base(sortBy, isAsc, pageNumber, pageSize)
    {
        if (!string.IsNullOrWhiteSpace(danhMuc))
            AddFilter("DanhMuc", FilterOperator.Equal, danhMuc);

        if (isActive.HasValue)
            AddFilter("IsActive", FilterOperator.Equal, isActive.Value);

        if (isSynced.HasValue)
            AddFilter("IsSynced", FilterOperator.Equal, isSynced.Value);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            AddKeyword("TieuDe", FilterOperator.Contains, keyword);
            AddKeyword("NoiDung", FilterOperator.Contains, keyword);
            AddKeyword("DanhMuc", FilterOperator.Contains, keyword);
        }

        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
