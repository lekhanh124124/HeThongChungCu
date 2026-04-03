using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public static class PhuongTienPolicy
{
    public static int GetQuota(LoaiCanHo loaiCanHo, LoaiPhuongTien loaiPhuongTien)
    {
        if (loaiPhuongTien == LoaiPhuongTien.XeMay || loaiPhuongTien == LoaiPhuongTien.XeDien)
        {
            if (loaiCanHo == LoaiCanHo.Penthouse) return 4;
            if (loaiCanHo == LoaiCanHo.Shophouse) return 5;
            return 2;
        }

        if (loaiPhuongTien == LoaiPhuongTien.Oto)
        {
            if (loaiCanHo == LoaiCanHo.Studio) return 0;
            if (loaiCanHo == LoaiCanHo.Penthouse || loaiCanHo == LoaiCanHo.Shophouse) return 2;
            return 1;
        }

        if (loaiPhuongTien == LoaiPhuongTien.XeDap)
        {
            if (loaiCanHo == LoaiCanHo.Penthouse || loaiCanHo == LoaiCanHo.Shophouse) return 5;
            return 2;
        }

        return 0;
    }

    public static bool IsOverQuota(int currentCount, int quota) => currentCount >= quota;
}
