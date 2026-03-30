using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Entities;

public static class PhuongTienPolicy
{
    /// <summary>
    /// Lấy số lượng tối đa cho phép của một loại phương tiện dựa trên loại căn hộ.
    /// Tuân thủ quy chuẩn QCVN 04:2021/BXD và thực tế vận hành.
    /// </summary>
    public static int GetQuota(LoaiCanHo loaiCanHo, LoaiPhuongTien loaiPhuongTien)
    {
        // Xe máy và Xe điện (Gộp chung hạn mức 6m2/căn hộ - Thường là 2 chiếc)
        if (loaiPhuongTien == LoaiPhuongTien.XeMay || loaiPhuongTien == LoaiPhuongTien.XeDien)
        {
            if (loaiCanHo == LoaiCanHo.Penthouse) return 4;
            if (loaiCanHo == LoaiCanHo.Shophouse) return 5;
            return 2; // mặc định 2 xe máy
        }

        // Ô tô
        if (loaiPhuongTien == LoaiPhuongTien.Oto)
        {
            if (loaiCanHo == LoaiCanHo.Studio) return 0; // Thường Studio không có slot ô tô
            if (loaiCanHo == LoaiCanHo.Penthouse || loaiCanHo == LoaiCanHo.Shophouse) return 2;
            return 1; // Standard mặc định 1 ô tô
        }

        // Xe đạp (Thường không giới hạn khắt khe, mặc định 2-3 chiếc)
        if (loaiPhuongTien == LoaiPhuongTien.XeDap)
        {
            if (loaiCanHo == LoaiCanHo.Penthouse || loaiCanHo == LoaiCanHo.Shophouse) return 5;
            return 2;
        }

        return 0;
    }

    public static bool IsOverQuota(int currentCount, int quota) => currentCount >= quota;
}
