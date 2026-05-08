using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.Enums;

public class CoCheTinhDiemBauCu : BaseEnum<CoCheTinhDiemBauCu, int>
{
    public static readonly CoCheTinhDiemBauCu MoiCanHoMotPhieu = new(1, "Một căn hộ - Một phiếu bầu");
    public static readonly CoCheTinhDiemBauCu TheoDienTichSoHuu = new(2, "Theo diện tích sở hữu riêng (m²)");

    private CoCheTinhDiemBauCu(int value, string name) : base(value, name) { }
}
