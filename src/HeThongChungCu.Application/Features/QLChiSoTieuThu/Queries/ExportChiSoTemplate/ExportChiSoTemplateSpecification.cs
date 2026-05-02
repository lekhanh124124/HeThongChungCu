using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Models;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Queries.ExportChiSoTemplate;

public class ExportChiSoTemplateSpecification : BaseSpecification
{
    public override HashSet<string> AllowedSortColumns => new(StringComparer.OrdinalIgnoreCase)
    {
        "Block", "TenTang", "MaCanHo"
    };

    public int Thang { get; }
    public int Nam { get; }

    public ExportChiSoTemplateSpecification(int dichVuId, int? toanhaId, int? tangId, int thang, int nam)
        : base("Block", true, 1, int.MaxValue)
    {
        Thang = thang;
        Nam = nam;
        AddFilter("IsDeleted", FilterOperator.Equal, false);
        AddFilter("DichVuId", FilterOperator.Equal, dichVuId);
        
        // Chỉ lấy số cũ từ các bản ghi đã xác nhận hoặc đã chốt
        AddFilter("TrangThaiChiSoId", FilterOperator.In, new List<int> { 2, 3 });

        if (toanhaId.HasValue)
        {
            AddFilter("ToaNhaId", FilterOperator.Equal, toanhaId.Value);
        }

        if (tangId.HasValue)
        {
            AddFilter("TangId", FilterOperator.Equal, tangId.Value);
        }
    }
}
