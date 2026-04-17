using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.GiaoViecNoiBo;

public class GiaoViecNoiBoCommandHandler : ICommandHandler<GiaoViecNoiBoCommand, YeuCauSuaChuaResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GiaoViecNoiBoCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        INhanVienCommandRepository nhanVienRepository,
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _nhanVienRepository = nhanVienRepository;
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaResponse>> Handle(GiaoViecNoiBoCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Request
        var ycsc = await _ycscRepository.GetByIdWithPersonnelAsync(request.Id, cancellationToken);
        if (ycsc == null)
            return Result.Failure<YeuCauSuaChuaResponse>(YeuCauSuaChuaErrors.NotFoundById(request.Id));

        // 2. Fetch Employee
        var nhanVien = await _nhanVienRepository.GetByIdAsync(request.NhanVienId, cancellationToken);
        if (nhanVien == null)
            return Result.Failure<YeuCauSuaChuaResponse>(NhanVienErrors.NotFoundById(request.NhanVienId));

        // 3. Logic
        ycsc.AssignInternalStaff(request.NhanVienId);

        // 4. Persistence
        _ycscRepository.Update(ycsc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build Response (Manual Mapping to match project pattern)
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
