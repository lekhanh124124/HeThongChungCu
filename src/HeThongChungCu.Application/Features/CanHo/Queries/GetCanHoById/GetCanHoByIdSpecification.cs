namespace HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;

public class GetCanHoByIdSpecification : BaseSpecification
{
    public GetCanHoByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
    }
}
