namespace HeThongChungCu.Application.Features.Profile.Queries.GetProfile;

public class GetProfileSpecification : BaseSpecification
{
    public GetProfileSpecification(int id) 
        : base(null, null, null, null)
    {
        AddFilter(nameof(User.Id), FilterOperator.Equal, id);
        AddFilter(nameof(User.IsDeleted), FilterOperator.Equal, false);
    }
}
