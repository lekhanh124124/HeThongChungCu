using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.GiaoViecDoiTac;

public class GiaoViecDoiTacCommandHandler : ICommandHandler<GiaoViecDoiTacCommand, YeuCauSuaChuaResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly IDoiTacCommandRepository _doiTacRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GiaoViecDoiTacCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        IDoiTacCommandRepository doiTacRepository,
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _doiTacRepository = doiTacRepository;
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaResponse>> Handle(GiaoViecDoiTacCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Request
        var ycsc = await _ycscRepository.GetByIdWithPersonnelAsync(request.Id, cancellationToken);
        if (ycsc == null)
            return Result.Failure<YeuCauSuaChuaResponse>(YeuCauSuaChuaErrors.NotFoundById(request.Id));

        // 2. Fetch/Validate Contract
        var hopDong = await _doiTacRepository.GetHopDongByIdAsync(request.HopDongDoiTacId, cancellationToken);
        if (hopDong == null)
            return Result.Failure<YeuCauSuaChuaResponse>(DoiTacErrors.NotFoundById(request.HopDongDoiTacId));

        if (!hopDong.IsActive())
            return Result.Failure<YeuCauSuaChuaResponse>(new Error("HopDongDoiTac.Inactive", "Hợp đồng đối tác hiện không còn hiệu lực để gán việc."));

        // 3. Logic
        ycsc.AssignPartner(request.HopDongDoiTacId);

        if (request.NhanSu != null && request.NhanSu.Count > 0)
        {
            foreach (var ns in request.NhanSu)
            {
                ycsc.AddNhanSuPartner(ns.HoTen, ns.SoCCCD, ns.SoDienThoai, ns.VaiTro, ns.GhiChu);
            }
        }

        // 4. Persistence
        _ycscRepository.Update(ycsc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build Response
        var canHo = await _canHoRepository.GetByIdAsync(ycsc.CanHoId, cancellationToken);
        var toaNha = canHo != null ? await _toaNhaRepository.GetToaNhaByTangIdAsync(canHo.TangId, cancellationToken) : null;
        var tang = toaNha?.Tangs.FirstOrDefault(t => t.Id == canHo!.TangId);
        var sender = await _nguoiDungRepository.GetByIdAsync(ycsc.CreatedBy, cancellationToken);

        return Result.Success(new YeuCauSuaChuaResponse
        {
            Id = ycsc.Id,
            CanHoId = ycsc.CanHoId,
            TenCanHo = canHo?.MaCanHo,
            TenTang = tang?.MaTang,
            TenToaNha = toaNha?.MaToaNha,
            LoaiYeuCauCuDanId = ycsc.LoaiYeuCauCuDanId.Value,
            LoaiYeuCauCuDanTen = ycsc.LoaiYeuCauCuDanId.Name,
            TrangThaiYeuCauId = ycsc.TrangThaiId.Value,
            TrangThaiYeuCauTen = ycsc.TrangThaiId.Name,
            NoiDung = ycsc.NoiDung,
            LoaiSuCoId = ycsc.LoaiSuCoId.Value,
            LoaiSuCoTen = ycsc.LoaiSuCoId.Name,
            TrangThaiSuaChuaId = ycsc.TrangThaiSuaChuaId.Value,
            TrangThaiSuaChuaTen = ycsc.TrangThaiSuaChuaId.Name,
            MucDoUuTienDeXuatId = ycsc.MucDoUuTienDeXuatId.Value,
            MucDoUuTienDeXuatTen = ycsc.MucDoUuTienDeXuatId.Name,
            MucDoUuTienChotId = ycsc.MucDoUuTienChotId?.Value,
            MucDoUuTienChotTen = ycsc.MucDoUuTienChotId?.Name,
            CreatedAt = ycsc.CreatedAt,
            CreatedBy = ycsc.CreatedBy,
            TenNguoiGui = sender != null ? sender.HoTen : null!
        });
    }
}
