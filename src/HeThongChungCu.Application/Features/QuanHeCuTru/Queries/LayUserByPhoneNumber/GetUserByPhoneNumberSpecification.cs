namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByPhoneNumber;

public class GetUserByPhoneNumberSpecification : BaseSpecification
{
    public GetUserByPhoneNumberSpecification(
        string? phoneNumber,
        List<int>? roleIds) 
        : base(null, null, null, null)
    {
        AddFilter("IsKetThuc", FilterOperator.Equal, false);
        AddFilter("UserIsDeleted", FilterOperator.Equal, false);
        AddFilter("QuanHeCuTruIsDeleted", FilterOperator.Equal, false);

        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            AddFilter("PhoneNumber", FilterOperator.Equal, phoneNumber);
        }

        if (roleIds != null && roleIds.Count > 0)
        {
            AddFilter("RoleId", FilterOperator.In, roleIds);
        }
    }
}
