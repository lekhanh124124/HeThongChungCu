using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Policies;

public interface ICuTruPolicy
{
    void ValidateCreate(int userId, LoaiQuanHeCuTru loaiQuanHe, IEnumerable<QuanHeCuTru> existingRelations);
}
