using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Domain.Policies;

public interface ICanHoPolicy
{
    void ValidateCreate(
        decimal dienTich, 
        int soPhongNgu, 
        int soPhongTam);

    void ValidateUpdate(
        CanHo currentCanHo,
        decimal newDienTich,
        int newSoPhongNgu,
        int newSoPhongTam,
        LoaiCanHo newLoaiCanHo,
        bool hasActiveResidents);

    void ValidateDelete(CanHo canHo, bool hasActiveResidents);

    void ValidateStatusChange(
        CanHo canHo, 
        TrangThaiCanHo nextStatus);
}
