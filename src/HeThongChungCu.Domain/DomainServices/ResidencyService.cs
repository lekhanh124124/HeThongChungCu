using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Exceptions;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Domain.DomainServices;

public class ResidencyService : IResidencyService
{
    private readonly IDocumentReconciliationService _documentReconciliationService;

    public ResidencyService(IDocumentReconciliationService documentReconciliationService)
    {
        _documentReconciliationService = documentReconciliationService;
    }

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

    public Result CheckHeadPermission(QuanHeCuTru? requesterRelation)
    {
        if (requesterRelation == null)
        {
            return Result.Failure(QuanHeCuTruErrors.NotFound);
        }

        if (requesterRelation.TrangThaiCuTruId != TrangThaiCuTru.DangCuTru ||
            (requesterRelation.LoaiQuanHeCuTruId != LoaiQuanHeCuTru.ChuHo &&
             requesterRelation.LoaiQuanHeCuTruId != LoaiQuanHeCuTru.NguoiThue))
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
            request.YeuCauNgaySinh ?? DateTimeOffset.MinValue,
            GioiTinh.FromValue(request.YeuCauGioiTinhId ?? 1, null)!,
            request.YeuCauDiaChi.FullAddress,
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
                docReq.Files.Select(f => new TepTaiLieuNguoiDung(f.FileName, f.FileUrl, f.Size, f.ContentType)));
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
            request.YeuCauDiaChi?.FullAddress ?? user.DiaChi.FullAddress,
            request.YeuCauCCCD ?? user.CCCD,
            request.YeuCauSoDienThoai ?? user.SoDienThoai);

        // 2. Đồng bộ hóa tài liệu (Document Reconciliation) via Domain Service
        var proposedDocs = request.YeuCauTaiLieuCuTrus.Select(d => new DocumentSyncItem(
            d.TaiLieuCuTruId,
            d.LoaiGiayToId.Value,
            d.SoGiayTo,
            d.NgayPhatHanh,
            d.Files.Select(f => f.Id).ToList()
        ));

        var fetchedFiles = request.YeuCauTaiLieuCuTrus.SelectMany(d => d.Files);

        _documentReconciliationService.ReconcileNguoiDungDocuments(user, proposedDocs, fetchedFiles);
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
        DateTimeOffset startDate,
        IEnumerable<QuanHeCuTru> existingRelations)
    {
        // 1. Kiểm tra cư dân đã đang cư trú tại căn hộ chưa
        if (existingRelations.Any(x =>
                x.NguoiDungId == userId &&
                x.CanHoId == canHoId &&
                x.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru))
        {
            return Result.Failure<QuanHeCuTru>(QuanHeCuTruErrors.UserAlreadyResident);
        }

        // 2. Kiểm tra chủ hộ hoặc người thuê đại diện duy nhất
        bool isHeadRole = loaiQuanHe == LoaiQuanHeCuTru.ChuHo || loaiQuanHe == LoaiQuanHeCuTru.NguoiThue;
        bool hasActiveHead = existingRelations.Any(x =>
            (x.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || x.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue) &&
            x.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);

        if (isHeadRole && hasActiveHead)
        {
            return Result.Failure<QuanHeCuTru>(QuanHeCuTruErrors.HouseholderAlreadyExists);
        }

        // 3. Đảm bảo căn hộ phải có chủ hộ hoặc người thuê đại diện trước khi thêm các thành viên khác
        if (!isHeadRole && !hasActiveHead)
        {
            return Result.Failure<QuanHeCuTru>(QuanHeCuTruErrors.HouseholderNotFound);
        }

        try
        {
            var quanHe = new QuanHeCuTru(canHoId, userId, loaiQuanHe, startDate);
            return Result.Success(quanHe);
        }
        catch (BusinessException ex)
        {
            return Result.Failure<QuanHeCuTru>(new Error("QuanHeCuTru.InvalidRelation", ex.Message));
        }
    }

    public void UpdateApartmentStatus(CanHo canHo, IEnumerable<QuanHeCuTru> activeRelations)
    {
        if (activeRelations.Any(r => r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru))
        {
            canHo.MarkAsOccupied();
        }
        else
        {
            canHo.MarkAsVacant();
        }
    }

    public void StartResidency(CanHo canHo, QuanHeCuTru relation, IEnumerable<QuanHeCuTru> allRelations)
    {
        // relation is assumed to be already created/added to the collection
        UpdateApartmentStatus(canHo, allRelations);
    }

    public void EndResidency(CanHo canHo, QuanHeCuTru relation, IEnumerable<QuanHeCuTru> allRelations, DateTimeOffset endDate)
    {
        relation.KetThucCuTru(endDate);
        UpdateApartmentStatus(canHo, allRelations);
    }
}
