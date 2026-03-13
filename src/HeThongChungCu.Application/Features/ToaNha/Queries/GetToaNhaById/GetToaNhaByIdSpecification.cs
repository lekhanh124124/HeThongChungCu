namespace HeThongChungCu.Application.Features.ToaNha.Queries.GetToaNhaById;

public class GetToaNhaByIdSpecification : BaseSpecification
{
    public GetToaNhaByIdSpecification(int id) 
        : base(null, null, null, null)
    {
        AddFilter(nameof(Domain.Entities.ChungCu.ToaNha.Id), FilterOperator.Equal, id);
        AddFilter(nameof(Domain.Entities.ChungCu.ToaNha.IsDeleted), FilterOperator.Equal, false);
    }
}
