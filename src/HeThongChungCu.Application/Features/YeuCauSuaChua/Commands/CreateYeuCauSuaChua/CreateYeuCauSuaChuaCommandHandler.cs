using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.UploadMedia.DTOs;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using YeuCauSuaChuaEntity = HeThongChungCu.Domain.Entities.YeuCauSuaChua;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.CreateYeuCauSuaChua;

public class CreateYeuCauSuaChuaCommandHandler : ICommandHandler<CreateYeuCauSuaChuaCommand, YeuCauSuaChuaResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateYeuCauSuaChuaCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaRepository,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaResponse>> Handle(CreateYeuCauSuaChuaCommand request, CancellationToken cancellationToken)
    {
        // 1. Domain Existence Validation
        var canHo = await _canHoRepository.GetByIdAsync(request.CanHoId, cancellationToken);
        if (canHo == null)
            return Result.Failure<YeuCauSuaChuaResponse>(CanHoErrors.NotFoundById(request.CanHoId));

        // 2. Fetch Files
        var tepTaiLieus = request.DanhSachTepIds != null && request.DanhSachTepIds.Count != 0
            ? await _tepTaiLieuRepository.GetByIdsAsync(request.DanhSachTepIds, cancellationToken)
            : [];

        var tepYeuCauSuaChuas = tepTaiLieus.Select(f =>
            f is TepYeuCauSuaChua tysc ? tysc : new TepYeuCauSuaChua(f.FileName, f.FileUrl, f.Size, f.ContentType)).ToList();

        // 3. Create Entity
        var ycsc = YeuCauSuaChuaEntity.Create(
            request.CanHoId,
            PhamViSuaChua.FromValue(request.PhamViId)!,
            LoaiSuCoKyThuat.FromValue(request.LoaiSuCoId)!,
            MucDoUuTien.FromValue(request.MucDoUuTienDeXuatId)!,
            request.NoiDung!,
            request.MoTaViTri,
            tepYeuCauSuaChuas);

        // 4. Persistence
        await _ycscRepository.AddAsync(ycsc, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build Response
        var toaNha = await _toaNhaRepository.GetToaNhaByTangIdAsync(canHo.TangId, cancellationToken);
        var tang = toaNha?.Tangs.FirstOrDefault(t => t.Id == canHo.TangId);

        var senderId = _currentUserService.UserId;
        var sender = senderId.HasValue ? await _nguoiDungRepository.GetByIdAsync(senderId.Value, cancellationToken) : null;

        return Result.Success(new YeuCauSuaChuaResponse
        {
            Id = ycsc.Id,
            CanHoId = ycsc.CanHoId,
            TenCanHo = canHo.MaCanHo,
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
