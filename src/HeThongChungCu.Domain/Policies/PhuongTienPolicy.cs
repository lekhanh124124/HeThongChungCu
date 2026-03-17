using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Policies;

public class PhuongTienPolicy : IPhuongTienPolicy
{
    public void ValidateAddThe(string maThe, PhuongTien phuongTien)
    {
        if (phuongTien.ThePhuongTiens.Any(x => x.MaThe == maThe && !x.IsLocked))
            throw new BusinessException("Thẻ phương tiện này đã tồn tại.");

        if (phuongTien.TrangThaiPhuongTienId != TrangThaiPhuongTien.Approved)
            throw new BusinessException("Phương tiện chưa được duyệt.");
    }
}
