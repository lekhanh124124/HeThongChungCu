namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByPhoneNumber;

public class GetUserByPhoneNumberSpecification : BaseSpecification
{
    public GetUserByPhoneNumberSpecification(
        string? phoneNumber,
        List<int>? roleIds) 
        : base(null, null, null, null)
    {
        AddFilter(nameof(User.IsDeleted), FilterOperator.Equal, false);

        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            AddFilter(nameof(User.PhoneNumber), FilterOperator.Equal, phoneNumber);
        }

        if (roleIds != null && roleIds.Count > 0)
        {
            AddFilter(nameof(User.RoleId), FilterOperator.In, roleIds);
        }
    }
}
