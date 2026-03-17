using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Exceptions;

namespace HeThongChungCu.Domain.Policies;

public class ToaNhaPolicy : IToaNhaPolicy
{
    public void ValidateAddTang(string maTang, ToaNha toaNha)
    {
        if (toaNha.Tangs.Any(x => x.MaTang == maTang))
            throw new BusinessException("Mã tầng đã tồn tại.");

        if (toaNha.TrangThaiToaNhaId != TrangThaiToaNha.DangHoatDong)
            throw new BusinessException("Tòa nhà chưa được hoạt động.");
    }
}
