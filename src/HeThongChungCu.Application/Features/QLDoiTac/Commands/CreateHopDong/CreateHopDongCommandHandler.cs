using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateHopDong;

public class CreateHopDongCommandHandler : ICommandHandler<CreateHopDongCommand, DoiTacDetailResponse>
{
    private readonly IDoiTacCommandRepository _doiTacCommandRepository;
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IDocumentReconciliationService _documentReconciliationService;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHopDongCommandHandler(
        IDoiTacCommandRepository doiTacCommandRepository,
        IDichVuCommandRepository dichVuCommandRepository,
        IDocumentReconciliationService documentReconciliationService,
        ITepTaiLieuRepository tepTaiLieuRepository,
        IUnitOfWork unitOfWork)
    {
        _doiTacCommandRepository = doiTacCommandRepository;
        _dichVuCommandRepository = dichVuCommandRepository;
        _documentReconciliationService = documentReconciliationService;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DoiTacDetailResponse>> Handle(CreateHopDongCommand request, CancellationToken cancellationToken)
    {
        var doiTac = await _doiTacCommandRepository.GetByIdWithHopDongsAsync(request.DoiTacId, cancellationToken);
        if (doiTac == null)
            return Result.Failure<DoiTacDetailResponse>(DoiTacErrors.NotFoundById(request.DoiTacId));

        var h = request.HopDong;

        // 1. Create a new Service (DichVu)
        var loaiDichVu = HeThongChungCu.Domain.Enums.LoaiDichVu.FromValue(h.LoaiDichVuId)
            ?? throw new Domain.Exceptions.BusinessException($"Loại dịch vụ không hợp lệ.");

        var dichVu = new DichVu(
            h.MaDichVu,
            h.TenDichVu,
            loaiDichVu,
            h.DonViTinh,
            h.MoTa,
            h.IconId,
            h.IsBatBuoc,
            h.SoLuongToiDa);

        await _dichVuCommandRepository.AddAsync(dichVu, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 2. Prepare SyncItem and Files for Reconciliation
        var files = h.TepFileIds != null && h.TepFileIds.Any()
            ? await _tepTaiLieuRepository.GetByIdsAsync(h.TepFileIds, cancellationToken)
            : Enumerable.Empty<TepTaiLieu>();

        var syncItem = new HopDongSyncItem(
            null,
            h.SoHopDong,
            h.NgayKy,
            h.NgayHetHan,
            h.GiaTri,
            dichVu.Id,
            h.NoiDung,
            h.TepFileIds
        );

        // 3. Reconcile (Adds the new contract and handles file mapping)
        _documentReconciliationService.ReconcileDoiTacHopDongs(doiTac, new[] { syncItem }, files);

        _doiTacCommandRepository.Update(doiTac);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Return the updated partner details directly to avoid round-trip
        var allDichVuIds = doiTac.HopDongs.Select(h => h.DichVuId).Distinct().ToList();
        var allDichVus = await _dichVuCommandRepository.GetByIdsAsync(allDichVuIds, cancellationToken);
        var dichVuIdMap = allDichVus.ToDictionary(v => v.Id);

        var response = new DoiTacDetailResponse
        {
            Id = doiTac.Id,
            TenDoiTac = doiTac.TenDoiTac,
            TenCongTy = doiTac.TenCongTy,
            NguoiDaiDien = doiTac.NguoiDaiDien,
            SoGiayPhepKD = doiTac.SoGiayPhepKD,
            MaSoThue = doiTac.MaSoThue,
            DiaChi = doiTac.DiaChi.ToString(),
            SoDienThoai = doiTac.SoDienThoai?.ToString(),
            Email = doiTac.Email?.ToString(),
            GhiChu = doiTac.GhiChu,
            HopDongs = doiTac.HopDongs.Select(h =>
            {
                var dv = dichVuIdMap.GetValueOrDefault(h.DichVuId);
                return new HopDongResponse
                {
                    Id = h.Id,
                    SoHopDong = h.SoHopDong,
                    NgayKy = h.NgayKy,
                    NgayHetHan = h.NgayHetHan,
                    GiaTriHopDong = h.GiaTriHopDong.SoTien,
                    LoaiDichVuId = dv?.LoaiDichVuId.Value ?? 0,
                    LoaiDichVuTen = dv?.LoaiDichVuId.Name ?? string.Empty,
                    TrangThaiHopDongId = h.TrangThaiHopDongId.Value,
                    TrangThaiHopDongTen = h.TrangThaiHopDongId.Name,
                    NoiDung = h.NoiDung,
                    MaDichVu = dv?.MaDichVu ?? string.Empty,
                    TenDichVu = dv?.TenDichVu ?? string.Empty,
                    DonViTinh = dv?.DonViTinh ?? string.Empty,
                    IsBatBuoc = dv?.IsBatBuoc ?? false,
                    TrangThaiDichVuId = dv?.TrangThaiId.Value ?? 0,
                    TrangThaiDichVuTen = dv?.TrangThaiId.Name ?? string.Empty,
                    Teps = h.TepHopDongs.Select(t =>
                    {
                        return new UploadFileResponse(
                            t.Id,
                            t.FileName,
                            t.FileUrl,
                            t.ContentType);
                    }).ToList()
                };
            }).OrderByDescending(h => h.NgayKy).ToList()
        };

        return Result.Success(response);
    }
}
