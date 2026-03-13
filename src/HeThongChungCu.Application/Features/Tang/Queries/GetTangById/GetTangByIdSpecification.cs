namespace HeThongChungCu.Application.Features.Tang.Queries.GetTangById;

public class GetTangByIdSpecification : BaseSpecification
{
    public GetTangByIdSpecification(int id) 
        : base(null, null, null, null)
    {
        AddFilter(nameof(Domain.Entities.ChungCu.Tang.Id), FilterOperator.Equal, id);
        AddFilter("TangIsDeleted", FilterOperator.Equal, false);
        AddFilter("ToaNhaIsDeleted", FilterOperator.Equal, false);
    }
}
