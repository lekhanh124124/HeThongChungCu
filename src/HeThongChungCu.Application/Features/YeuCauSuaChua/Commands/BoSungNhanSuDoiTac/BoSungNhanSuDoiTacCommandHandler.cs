using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.BoSungNhanSuDoiTac;

public class BoSungNhanSuDoiTacCommandHandler : ICommandHandler<BoSungNhanSuDoiTacCommand, YeuCauSuaChuaResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BoSungNhanSuDoiTacCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaResponse>> Handle(BoSungNhanSuDoiTacCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Request
        var ycsc = await _ycscRepository.GetByIdWithPersonnelAsync(request.Id, cancellationToken);
        if (ycsc == null)
            return Result.Failure<YeuCauSuaChuaResponse>(YeuCauSuaChuaErrors.NotFoundById(request.Id));

        // 2. Validate Partner Assignment
        if (ycsc.HopDongDoiTacId == null)
            return Result.Failure<YeuCauSuaChuaResponse>(new Error("YeuCauSuaChua.NoPartnerAssigned", "Yêu cầu này chưa được giao cho đối tác, không thể bổ sung nhân sự đối tác."));

        // 3. Logic
        foreach (var ns in request.NhanSu)
        {
            ycsc.AddNhanSuPartner(ns.HoTen, ns.SoCCCD, ns.SoDienThoai, ns.VaiTro, ns.GhiChu);
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
