using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.ThietLapCuTru;

public class ThietLapCuTruCommandHandler : ICommandHandler<ThietLapCuTruCommand, CuDanResponse>
{
    private readonly ICanHoCommandRepository _canHoRepository;
    private readonly IToaNhaCommandRepository _toaNhaCommandRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly IQuanHeCuTruCommandRepository _quanHeCuTruRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ThietLapCuTruCommandHandler(
        ICanHoCommandRepository canHoRepository,
        IToaNhaCommandRepository toaNhaCommandRepository,
        INguoiDungCommandRepository userRepository,
        ITaiKhoanCommandRepository accountRepository,
        IQuanHeCuTruCommandRepository quanHeCuTruRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _canHoRepository = canHoRepository;
        _toaNhaCommandRepository = toaNhaCommandRepository;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _quanHeCuTruRepository = quanHeCuTruRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CuDanResponse>> Handle(ThietLapCuTruCommand request, CancellationToken cancellationToken)
    {
        var canHo = await _canHoRepository.GetByIdAsync(request.CanHoId, cancellationToken);
        if (canHo is null)
            return CanHoErrors.NotFoundById(request.CanHoId);

        var toaNha = await _toaNhaCommandRepository.GetToaNhaByTangIdAsync(canHo.TangId, cancellationToken);
        if (toaNha == null)
            return ToaNhaErrors.NotFoundById(canHo.TangId);

        var tang = toaNha.Tangs.FirstOrDefault(t => t.Id == canHo.TangId);
        if (tang == null)
            return TangErrors.NotFoundById(canHo.TangId);

        // 1. Resolve User
        var user = await _userRepository.GetByIdWithDocumentsAsync(request.UserId, cancellationToken);
        if (user is null)
            return UserErrors.NotFoundById(request.UserId);

        // 2. Setup Residency (Logic moved from ResidencyService)
        var loaiQuanHe = LoaiQuanHeCuTru.FromValue(request.LoaiQuanHeCuTruId);
        var existingRelations = (await _quanHeCuTruRepository.GetByCanHoIdAsync(canHo.Id, cancellationToken)).ToList();
        var now = _dateTimeProvider.Now;

        // Validation logic from CreateRelation
        if (existingRelations.Any(x =>
                x.NguoiDungId == user.Id &&
                x.CanHoId == canHo.Id &&
                x.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru))
        {
            return QuanHeCuTruErrors.UserAlreadyResident;
        }

        bool isHeadRole = loaiQuanHe == LoaiQuanHeCuTru.ChuHo || loaiQuanHe == LoaiQuanHeCuTru.NguoiThue;
        bool hasActiveHead = existingRelations.Any(x =>
            (x.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo || x.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue) &&
            x.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);

        if (isHeadRole && hasActiveHead)
        {
            return QuanHeCuTruErrors.HouseholderAlreadyExists;
        }

        if (!isHeadRole && !hasActiveHead)
        {
            return QuanHeCuTruErrors.HouseholderNotFound;
        }

        var quanHe = new QuanHeCuTru(canHo.Id, user.Id, loaiQuanHe!, now);
        await _quanHeCuTruRepository.AddAsync(quanHe, cancellationToken);

        // 3. Update Apartment Status
        existingRelations.Add(quanHe);
        bool hasOwner = existingRelations.Any(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.ChuHo && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);
        bool hasTenant = existingRelations.Any(r => r.LoaiQuanHeCuTruId == LoaiQuanHeCuTru.NguoiThue && r.TrangThaiCuTruId == TrangThaiCuTru.DangCuTru);
        
        canHo.SyncStatusWithResidency(hasOwner, hasTenant);
        _canHoRepository.Update(canHo);

        // Update role if account exists
        var account = await _accountRepository.GetByNguoiDungIdAsync(user.Id, cancellationToken);
        if (account != null)
        {
            var roles = account.PhanQuyens.Select(pq => pq.RoleId).ToList();
            if (roles.Contains(Role.Guest) && !roles.Contains(Role.Resident))
            {
                account.RemoveRole(Role.Guest);
                account.AddRole(Role.Resident);
                _accountRepository.Update(account);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CuDanResponse
        {
            MaToaNha = toaNha.MaToaNha,
            MaTang = tang.MaTang,
            MaCanHo = canHo.MaCanHo,
            QuanHeCuTruId = quanHe.Id,
            UserId = user.Id,
            HoTen = $"{user.Ho} {user.Ten}",
            LoaiQuanHeCuTruId = quanHe.LoaiQuanHeCuTruId.Value,
            TenLoaiQuanHeCuTru = quanHe.LoaiQuanHeCuTruId.Name,
            NgayBatDau = quanHe.ThoiGian.NgayBatDau,
            NgayKetThuc = quanHe.ThoiGian.NgayKetThuc,
            TrangThaiCuTruId = quanHe.TrangThaiCuTruId.Value,
            TenTrangThaiCuTru = quanHe.TrangThaiCuTruId.Name
        });
    }
}
