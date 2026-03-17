using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Policies;

public class ChiSoTieuThuPolicy : IChiSoTieuThuPolicy
{
    public void ValidateUpdate(ChiSoTieuThu chiSo)
    {
        if (chiSo.IsLock)
            throw new BusinessException("Chỉ số tiêu thụ đã bị khóa, không thể cập nhật.");
    }
}
