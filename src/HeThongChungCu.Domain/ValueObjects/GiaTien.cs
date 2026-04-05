using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.ValueObjects;

public record GiaTien
{
    public decimal SoTien { get; private set; }
    public string LoaiTien { get; private set; } = "VND";

    private GiaTien() { }

    public GiaTien(decimal soTien, string loaiTien = "VND")
    {
        if (soTien < 0)
            throw new BusinessException("Số tiền không được âm.");

        SoTien = soTien;
        LoaiTien = loaiTien;
    }

    public override string ToString() => $"{SoTien:N0} {LoaiTien}";

    public static implicit operator decimal(GiaTien giaTien) => giaTien.SoTien;
    public static implicit operator GiaTien(decimal soTien) => new(soTien);
}
