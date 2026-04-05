using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.ValueObjects;

public record ThongSoCanHo
{
    public decimal DienTich { get; private set; }
    public int SoPhongNgu { get; private set; }
    public int SoPhongTam { get; private set; }

    public ThongSoCanHo(decimal dienTich, int soPhongNgu, int soPhongTam)
    {
        if (dienTich <= 0)
            throw new BusinessException("Diện tích căn hộ phải lớn hơn 0.");

        if (soPhongNgu < 0)
            throw new BusinessException("Số phòng ngủ không được âm.");

        if (soPhongTam < 0)
            throw new BusinessException("Số phòng tắm không được âm.");

        DienTich = dienTich;
        SoPhongNgu = soPhongNgu;
        SoPhongTam = soPhongTam;
    }
}
