namespace HeThongChungCu.Application.Features.CanHo.Queries.GetCanHoById;

public class GetCanHoByIdSpecification : BaseSpecification
{
    public GetCanHoByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("CanHoIsDeleted", FilterOperator.Equal, false);
        AddFilter("TangIsDeleted", FilterOperator.Equal, false);
    }
}
