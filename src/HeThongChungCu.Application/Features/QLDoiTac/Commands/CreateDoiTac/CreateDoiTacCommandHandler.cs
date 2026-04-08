using HeThongChungCu.Application.Features.QLDoiTac.DTOs;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLDoiTac.Commands.CreateDoiTac;

public class CreateDoiTacCommandHandler : ICommandHandler<CreateDoiTacCommand, DoiTacDetailResponse>
{
    private readonly IDoiTacCommandRepository _doiTacCommandRepository;
    private readonly IDichVuCommandRepository _dichVuCommandRepository;
    private readonly IDocumentReconciliationService _documentReconciliationService;
    private readonly ITepTaiLieuRepository _tepTaiLieuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDoiTacCommandHandler(
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

    public async Task<Result<DoiTacDetailResponse>> Handle(CreateDoiTacCommand request, CancellationToken cancellationToken)
    {
        var doiTac = new Domain.Entities.DoiTac(
            request.TenDoiTac,
            request.TenCongTy,
            request.NguoiDaiDien,
            request.SoGiayPhepKD,
            request.MaSoThue,
            request.DiaChi,
            request.SoDienThoai,
            request.Email,
            request.GhiChu);

        var dichVuDict = new Dictionary<HopDongRequestDto, Domain.Entities.DichVu>();
        var files = new List<Domain.Entities.TepTaiLieu>();

        if (request.HopDongs != null && request.HopDongs.Count > 0)
        {
            var fileIds = request.HopDongs
                .Where(h => h.TepFileIds != null)
                .SelectMany(h => h.TepFileIds!)
                .Distinct()
                .ToList();

            files = (await _tepTaiLieuRepository.GetByIdsAsync(fileIds, cancellationToken)).ToList();

            foreach (var h in request.HopDongs)
            {
                var loaiDichVu = HeThongChungCu.Domain.Enums.LoaiDichVu.FromValue(h.LoaiDichVuId)
                    ?? throw new Domain.Exceptions.BusinessException($"Loại dịch vụ không hợp lệ.");
                var dichVu = new Domain.Entities.DichVu(
                    h.MaDichVu, h.TenDichVu, loaiDichVu, h.DonViTinh,
                    h.MoTa, h.IconId, h.IsBatBuoc, h.SoLuongToiDa);
                
                await _dichVuCommandRepository.AddAsync(dichVu, cancellationToken);
                dichVuDict.Add(h, dichVu);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var syncItems = request.HopDongs.Select(h => new HopDongSyncItem(
                h.Id,
                h.SoHopDong,
                h.NgayKy,
                h.NgayHetHan,
                h.GiaTri,
                dichVuDict[h].Id,
                h.NoiDung,
                h.TepFileIds
            ));

            _documentReconciliationService.ReconcileDoiTacHopDongs(doiTac, syncItems, files);
        }

        await _doiTacCommandRepository.AddAsync(doiTac, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map directly from the entity in memory to avoid round-trip
        var dichVuIdMap = dichVuDict.Values.ToDictionary(v => v.Id);
        var fileMap = files.ToDictionary(f => f.Id);

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
                    TenLoaiDichVu = dv?.LoaiDichVuId.Name ?? string.Empty,
                    TrangThaiHopDongId = h.TrangThaiHopDongId.Value,
                    TenTrangThaiHopDong = h.TrangThaiHopDongId.Name,
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
            }).ToList()
        };

        // Calculate top-level expiry date from contracts
        response.NgayHetHan = response.HopDongs
            .Where(h => h.TrangThaiHopDongId == TrangThaiHopDong.ConHieuLuc.Value || h.TrangThaiHopDongId == TrangThaiHopDong.SapHetHan.Value)
            .Select(h => (DateTimeOffset?)h.NgayHetHan)
            .Max();

        return Result.Success(response);
    }
}
