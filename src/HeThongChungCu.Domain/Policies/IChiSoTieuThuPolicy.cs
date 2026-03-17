using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Policies;

public interface IChiSoTieuThuPolicy
{
    void ValidateUpdate(ChiSoTieuThu chiSo);
}
