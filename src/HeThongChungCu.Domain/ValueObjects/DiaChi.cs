using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.ValueObjects;

public record DiaChi
{
    public string SoNhaTenDuong { get; private set; } = string.Empty;
    public string PhuongXa { get; private set; } = string.Empty;
    public string QuanHuyen { get; private set; } = string.Empty;
    public string TinhThanhPho { get; private set; } = string.Empty;
    public string FullAddress { get; private set; } = string.Empty;

    private DiaChi() { }

    public DiaChi(string? fullAddress)
    {
        FullAddress = fullAddress ?? string.Empty;
    }

    public static DiaChi FromParts(string soNhaTenDuong, string phuongXa, string quanHuyen, string tinhThanhPho)
    {
        var full = $"{soNhaTenDuong}, {phuongXa}, {quanHuyen}, {tinhThanhPho}";
        return new DiaChi
        {
            SoNhaTenDuong = soNhaTenDuong,
            PhuongXa = phuongXa,
            QuanHuyen = quanHuyen,
            TinhThanhPho = tinhThanhPho,
            FullAddress = full
        };
    }

    public override string ToString() => FullAddress;
}
