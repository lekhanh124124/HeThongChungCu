using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.LienKetTaiKhoan;

public class LienKetTaiKhoanCommandHandler : ICommandHandler<LienKetTaiKhoanCommand, UserInfoResponse>
{
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly IIdentityDomainService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public LienKetTaiKhoanCommandHandler(
        ITaiKhoanCommandRepository accountRepository,
        INguoiDungCommandRepository userRepository,
        IIdentityDomainService identityService,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserInfoResponse>> Handle(LienKetTaiKhoanCommand request, CancellationToken cancellationToken)
    {
        // 1. Find account by Email
        var account = await _accountRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (account is null)
        {
            return AuthErrors.AccountNotFound;
        }

        // 2. Link account and promote role via Domain Service
        var isResidentAlreadyLinked = await _accountRepository.AnyAsync(a => a.NguoiDungId == request.UserId && a.Id != account.Id, cancellationToken);
        
        var canLinkResult = _identityService.CanLinkAccountToResident(account, request.UserId, isResidentAlreadyLinked);
        if (canLinkResult.IsFailure)
            return canLinkResult.Errors[0];

        _identityService.LinkAccountToResident(account, request.UserId);

        // 3. Revoke any pending identification tokens
        _identityService.RevokeIdentificationTokens(account, ReasonRevoked.AdminAction);

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return User Info
        var user = await _userRepository.GetByIdWithDocumentsAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return UserErrors.NotFoundById(request.UserId);
        }

        return Result.Success(new UserInfoResponse
        {
            Id = user.Id,
            FirstName = user.Ten,
            LastName = user.Ho,
            Dob = user.NgaySinh,
            GioiTinhId = user.GioiTinhId.Value,
            GioiTinhName = user.GioiTinhId.Name,
            DiaChi = user.DiaChi.FullAddress,
            IdCard = user.CCCD,
            PhoneNumber = user.SoDienThoai ?? string.Empty,
            TaiLieuCuTrus = user.TaiLieu.Select(d => new TaiLieuResponse
            {
                Id = d.Id,
                LoaiGiayToId = d.LoaiGiayToId.Value,
                TenLoaiGiayTo = d.LoaiGiayToId.Name,
                SoGiayTo = d.SoGiayTo,
                NgayPhatHanh = d.NgayPhatHanh,
                Files = d.Files.Select(f => new TepTaiLieuResponse(f.Id, f.FileUrl, f.FileName, f.ContentType)).ToList()
            }).ToList()
        });
    }
}
