namespace HeThongChungCu.Application.Features.Profile.Queries.GetProfile;

public class GetProfileSpecification : BaseSpecification
{
    public GetProfileSpecification(int id) 
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
    }
}
