using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Domain.DomainServices;

public class CanHoDomainService : ICanHoDomainService
{
    public Result CanCreateCanHo(Tang tang, string maCanHo, bool isMaExists)
    {
        // 1. Không được tạo căn hộ ở tầng hầm
        if (tang.LoaiTangId == LoaiTang.TangHam)
        {
            return Result.Failure(CanHoErrors.CanHoInBasement);
        }

        // 2. Kiểm tra trùng mã căn hộ
        if (isMaExists)
        {
            return Result.Failure(CanHoErrors.MaCanHoAlreadyExists);
        }

        return Result.Success();
    }

    public Result CanUpdateStructure(CanHo canHo, string newMaCanHo, bool isMaExists, bool hasActiveResidents)
    {
        // 1. Nếu mã thay đổi, kiểm tra trùng mã
        if (newMaCanHo != canHo.MaCanHo && isMaExists)
        {
            return Result.Failure(CanHoErrors.MaCanHoAlreadyExists);
        }

        // 2. Không được thay đổi cấu trúc khi có cư dân (Sẽ được phối hợp từ Application layer)
        if (hasActiveResidents)
        {
            return Result.Failure(new Error("CanHo.StructureChangeForbidden", "Không được thay đổi cấu trúc căn hộ khi đang có cư dân cư trú."));
        }

        return Result.Success();
    }
}
