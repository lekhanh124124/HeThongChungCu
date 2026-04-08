using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Interfaces;

public interface IDichVuDomainService
{
    Result CanRegister(DichVu dichVu, int sumHienTai, int soLuongMoi, DateTimeOffset? ngayDangKy = null, KhungGioDichVu? khungGio = null);
}
