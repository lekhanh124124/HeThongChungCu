using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLDoiTac.Queries.GetDoiTacById;

public class GetDoiTacByIdSpecification : BaseSpecification
{
    public GetDoiTacByIdSpecification(int id)
        : base(null, null, null, null)
    {
        AddFilter("Id", FilterOperator.Equal, id);
        AddFilter("IsDeleted", FilterOperator.Equal, false);

        // Filters for related HopDongDoiTac
        AddFilter("HopDongIsDeleted", FilterOperator.Equal, false);

        // Filters for related TepTaiLieu
        AddFilter("TepIsDeleted", FilterOperator.Equal, false);
        AddFilter("LoaiTepTaiLieu", FilterOperator.Equal, "TepHopDongDoiTac");

        // Filters for related DichVu
        AddFilter("DichVuIsDeleted", FilterOperator.Equal, false);
    }
}
