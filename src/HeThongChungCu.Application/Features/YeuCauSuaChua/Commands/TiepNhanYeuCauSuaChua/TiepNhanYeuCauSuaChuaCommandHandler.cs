using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TiepNhanYeuCauSuaChua;

public class TiepNhanYeuCauSuaChuaCommandHandler : ICommandHandler<TiepNhanYeuCauSuaChuaCommand, YeuCauSuaChuaResponse>
{
    private readonly IYeuCauSuaChuaCommandRepository _ycscRepository;
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaRepository;
    private readonly INhanVienCommandRepository _nhanVienRepository;
    private readonly INguoiDungCommandRepository _nguoiDungRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public TiepNhanYeuCauSuaChuaCommandHandler(
        IYeuCauSuaChuaCommandRepository ycscRepository,
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaRepository,
        INhanVienCommandRepository nhanVienRepository,
        INguoiDungCommandRepository nguoiDungRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _ycscRepository = ycscRepository;
        _canHoRepository = canHoRepository;
        _toaNhaRepository = toaNhaRepository;
        _nhanVienRepository = nhanVienRepository;
        _nguoiDungRepository = nguoiDungRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauSuaChuaResponse>> Handle(TiepNhanYeuCauSuaChuaCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Request
        var ycsc = await _ycscRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ycsc == null)
            return Result.Failure<YeuCauSuaChuaResponse>(YeuCauSuaChuaErrors.NotFoundById(request.Id));

        // 2. Fetch Current Employee
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure<YeuCauSuaChuaResponse>(UserErrors.NotFound);

        // Fetch Employee record for the user
        var employee = await _nhanVienRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        if (employee == null)
            return Result.Failure<YeuCauSuaChuaResponse>(NhanVienErrors.NotFound);

        // 3. Logic
        ycsc.TiepNhan(employee.Id, _dateTimeProvider.Now);

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
