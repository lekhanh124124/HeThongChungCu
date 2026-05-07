using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetHoaDonDoiTacById;

public class GetHoaDonDoiTacByIdSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase) { "Id" };

    public GetHoaDonDoiTacByIdSpecification(int id) : base("Id", true, 1, 1)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);
    }
}
