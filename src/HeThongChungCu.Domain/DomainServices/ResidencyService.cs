using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Domain.DomainServices;

public class ResidencyService : IResidencyService
{
    public Result CheckUniqueness(bool cccdExists, bool phoneExists)
    {
        if (cccdExists)
        {
            return Result.Failure(UserErrors.IdCardAlreadyExists);
        }

        if (phoneExists)
        {
            return Result.Failure(UserErrors.PhoneNumberAlreadyExists);
        }

        return Result.Success();
    }

    public Result CheckChuHoPermission(QuanHeCuTru? requesterRelation)
    {
        if (requesterRelation == null)
        {
            return Result.Failure(CanHoErrors.NotFound);
        }

        if (requesterRelation.TrangThaiCuTruId != TrangThaiCuTru.DangCuTru || 
            requesterRelation.LoaiQuanHeCuTruId != LoaiQuanHeCuTru.ChuHo)
        {
            return Result.Failure(YeuCauCuTruErrors.Forbidden);
        }

        return Result.Success();
    }

    public NguoiDung CreateUserFromRequest(YeuCauCuTru request)
    {
        // 1. Khởi tạo User
        var newUser = new NguoiDung(
            request.YeuCauTen!,
            request.YeuCauHo!,
            request.YeuCauNgaySinh ?? DateTime.MinValue,
            GioiTinh.FromValue(request.YeuCauGioiTinhId ?? 1, null)!,
            request.YeuCauDiaChi,
            cccd: request.YeuCauCCCD,
            soDienThoai: request.YeuCauSoDienThoai);

        // 2. Thêm tài liệu
        foreach (var docReq in request.YeuCauTaiLieuCuTrus)
        {
            var newDoc = new TaiLieuNguoiDung(
                null,
                docReq.LoaiGiayToId,
                docReq.SoGiayTo,
                docReq.NgayPhatHanh,
                docReq.Files);
            newUser.AddDocument(newDoc);
        }

        return newUser;
    }

    public void UpdateUserFromRequest(NguoiDung user, YeuCauCuTru request)
    {
        // 1. Cập nhật thông tin cá nhân
        user.UpdateProfile(
            request.YeuCauTen ?? user.Ten,
            request.YeuCauHo ?? user.Ho,
            request.YeuCauNgaySinh ?? user.NgaySinh,
            request.YeuCauGioiTinhId.HasValue ? GioiTinh.FromValue(request.YeuCauGioiTinhId.Value, null)! : user.GioiTinhId,
            request.YeuCauDiaChi ?? user.DiaChi,
            request.YeuCauCCCD ?? user.CCCD,
            request.YeuCauSoDienThoai ?? user.SoDienThoai);

        // 2. Đồng bộ hóa tài liệu (Document Reconciliation)
        var currentDocs = user.TaiLieu.ToList();
        var proposedDocs = request.YeuCauTaiLieuCuTrus;

        // 2.1. Xóa tài liệu không có trong yêu cầu
        var proposedOriginalIds = proposedDocs
            .Where(d => d.TaiLieuCuTruId.HasValue)
            .Select(d => d.TaiLieuCuTruId!.Value)
            .ToList();

        foreach (var doc in currentDocs)
        {
            if (!proposedOriginalIds.Contains(doc.Id))
            {
                user.RemoveDocument(doc.Id);
            }
        }

        // 2.2. Cập nhật tài liệu cũ hoặc thêm tài liệu mới
        foreach (var propDoc in proposedDocs)
        {
            if (propDoc.TaiLieuCuTruId.HasValue)
            {
                // Cập nhật tài liệu hiện có
                var existingDoc = user.TaiLieu.FirstOrDefault(d => d.Id == propDoc.TaiLieuCuTruId.Value);
                if (existingDoc != null)
                {
                    existingDoc.UpdateInfo(propDoc.LoaiGiayToId, propDoc.SoGiayTo, propDoc.NgayPhatHanh);
                    existingDoc.SyncFiles(propDoc.Files);
                }
            }
            else
            {
                // Thêm tài liệu mới
                var newDoc = new TaiLieuNguoiDung(
                    user.Id,
                    propDoc.LoaiGiayToId,
                    propDoc.SoGiayTo,
                    propDoc.NgayPhatHanh,
                    propDoc.Files);
                user.AddDocument(newDoc);
            }
        }
    }

    public Result CheckCanUpdateOrDeleteCanHo(CanHo canHo, IEnumerable<QuanHeCuTru> currentResidents)
    {
        if (currentResidents.Any(r => r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru))
        {
            return Result.Failure(new Error("CanHo.HasActiveResidents", "Không được thực hiện thao tác này khi đang có cư dân cư trú."));
        }

        return Result.Success();
    }

    public Result<QuanHeCuTru> CreateRelation(
        int canHoId,
        int userId,
        LoaiQuanHeCuTru loaiQuanHe,
        DateTime startDate,
        IEnumerable<QuanHeCuTru> existingRelations)
    {
        // 1. Kiểm tra cư dân đã đang cư trú tại căn hộ chưa
        if (existingRelations.Any(x =>
                x.NguoiDungId == userId &&
                x.CanHoId == canHoId &&
                x.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru))
        {
            return Result.Failure<QuanHeCuTru>(new Error("Residency.AlreadyResident", "Cư dân này đã đang cư trú tại căn hộ này."));
        }

        // 2. Kiểm tra chủ hộ duy nhất
        if (loaiQuanHe == LoaiQuanHeCuTru.ChuHo &&
            existingRelations.Any(x =>
                x.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo &&
                x.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru))
        {
            return Result.Failure<QuanHeCuTru>(new Error("Residency.OwnerAlreadyExists", "Căn hộ đã có chủ hộ."));
        }

        // 3. Đảm bảo căn hộ phải có chủ hộ trước khi thêm các thành viên khác
        if (loaiQuanHe != LoaiQuanHeCuTru.ChuHo)
        {
            var hasActiveHouseholder = existingRelations.Any(x =>
                x.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo &&
                x.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);

            if (!hasActiveHouseholder)
            {
                return Result.Failure<QuanHeCuTru>(new Error("Residency.HouseholderNotFound", "Căn hộ chưa có chủ hộ. Cần thiết lập chủ hộ trước."));
            }
        }

        try
        {
            var quanHe = new QuanHeCuTru(canHoId, userId, loaiQuanHe, startDate);
            return Result.Success(quanHe);
        }
        catch (BusinessException ex)
        {
            return Result.Failure<QuanHeCuTru>(new Error("Residency.InvalidRelation", ex.Message));
        }
    }
}
