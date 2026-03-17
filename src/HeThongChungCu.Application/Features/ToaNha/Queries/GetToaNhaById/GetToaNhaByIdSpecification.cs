using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.ToaNha.Queries.GetToaNhaById;

public class GetToaNhaByIdSpecification : BaseSpecification
{
    public GetToaNhaByIdSpecification(int id) 
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
