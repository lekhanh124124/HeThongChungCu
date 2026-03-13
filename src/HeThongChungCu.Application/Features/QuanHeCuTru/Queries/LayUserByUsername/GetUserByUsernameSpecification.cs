namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByUsername;

public class GetUserByUsernameSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(User.Id),
        nameof(User.Username),
        nameof(User.Email),
        nameof(User.FirstName),
        nameof(User.LastName),
    };

    public GetUserByUsernameSpecification(
        string? username,
        List<int>? roleIds,
        string? sortCol = null,
        bool? isAsc = null,
        int? pageNumber = null,
        int? pageSize = null) 
        : base(sortCol, isAsc, pageNumber, pageSize)
    {
        AddFilter(nameof(User.IsDeleted), FilterOperator.Equal, false);

        if (!string.IsNullOrWhiteSpace(username))
        {
            AddFilter(nameof(User.Username), FilterOperator.Equal, username);
        }

        if (roleIds != null && roleIds.Count > 0)
        {
            AddFilter(nameof(User.RoleId), FilterOperator.In, roleIds);
        }
    }
}
