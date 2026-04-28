using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Domain.ValueObjects;

public record KyThanhToan
{
    public int Thang { get; init; }
    public int Nam { get; init; }

    public KyThanhToan(int thang, int nam)
    {
        if (thang < 1 || thang > 12)
            throw new ArgumentOutOfRangeException(nameof(thang), "Tháng phải từ 1 đến 12.");
        
        if (nam < 2000)
            throw new ArgumentOutOfRangeException(nameof(nam), "Năm không hợp lệ.");

        Thang = thang;
        Nam = nam;
    }

    public static KyThanhToan Current => new(DateTime.Now.Month, DateTime.Now.Year);

    public KyThanhToan Previous()
    {
        if (Thang == 1) return new KyThanhToan(12, Nam - 1);
        return new KyThanhToan(Thang - 1, Nam);
    }

    public KyThanhToan Next()
    {
        if (Thang == 12) return new KyThanhToan(1, Nam + 1);
        return new KyThanhToan(Thang + 1, Nam);
    }

    public override string ToString() => $"{Thang:D2}/{Nam}";
}
