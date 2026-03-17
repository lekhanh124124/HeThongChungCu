using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Policies;

public class CuTruPolicy : ICuTruPolicy
{
    public void ValidateCreate(int userId, LoaiQuanHeCuTru loaiQuanHe, IEnumerable<QuanHeCuTru> existingRelations)
    {
        if (existingRelations.Any(x => x.UserId == userId && !x.IsKetThuc))
            throw new BusinessException("Cư dân này đã đang cư trú tại căn hộ.");

        if (loaiQuanHe == LoaiQuanHeCuTru.ChuHo && existingRelations.Any(x => x.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo && !x.IsKetThuc))
            throw new BusinessException("Căn hộ đã có chủ hộ.");
    }
}
