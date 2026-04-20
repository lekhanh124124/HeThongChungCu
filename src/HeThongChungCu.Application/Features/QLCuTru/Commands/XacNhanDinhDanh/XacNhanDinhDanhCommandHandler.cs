using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.XacNhanDinhDanh;

public class XacNhanDinhDanhCommandHandler : ICommandHandler<XacNhanDinhDanhCommand, UserInfoResponse>
{
    private readonly ITaiKhoanCommandRepository _accountRepository;
    private readonly INguoiDungCommandRepository _userRepository;
    private readonly IHasherService _hasherService;
    private readonly ITokenService _tokenService;
    private readonly IIdentityDomainService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public XacNhanDinhDanhCommandHandler(
        ITaiKhoanCommandRepository accountRepository,
        INguoiDungCommandRepository userRepository,
        IHasherService hasherService,
        ITokenService tokenService,
        IIdentityDomainService identityService,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _hasherService = hasherService;
        _tokenService = tokenService;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserInfoResponse>> Handle(XacNhanDinhDanhCommand request, CancellationToken cancellationToken)
    {
        // 1. Extract IDs from token
        var accountId = _tokenService.GetAccountIdFromToken(request.Token);
        var userId = _tokenService.GetUserIdFromToken(request.Token);

        if (accountId == null || userId == null)
        {
            return AuthErrors.InvalidToken;
        }

        // 2. Find account by Id (include tokens)
        var account = await _accountRepository.GetWithTokensAsync(accountId.Value, cancellationToken);
        if (account is null)
        {
            return AuthErrors.AccountNotFound;
        }

        // 3. Verify token exists and is active inside account using jti
        var jti = _tokenService.GetJwtIdFromToken(request.Token);
        var tokenEntity = account.Tokens.FirstOrDefault(t => t.TokenHash == jti && t.TokenType == TokenType.UserCode);
        if (tokenEntity == null || !tokenEntity.IsActive)
        {
            return AuthErrors.InvalidToken;
        }

        // 4. Validate business rules
        var isResidentAlreadyLinked = await _accountRepository.AnyAsync(a => a.NguoiDungId == userId.Value && a.Id != account.Id, cancellationToken);
        var canLinkResult = _identityService.CanLinkAccountToResident(account, userId.Value, isResidentAlreadyLinked);
        if (canLinkResult.IsFailure)
        {
            return canLinkResult.Errors[0];
        }

        // 5. Perform link and promote role
        _identityService.LinkAccountToResident(account, userId.Value);

        // 6. Revoke current token
        account.RevokeToken(jti!, DateTimeOffset.UtcNow, ReasonRevoked.UserAction);

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Return User Info
        var user = await _userRepository.GetByIdWithDocumentsAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return UserErrors.NotFoundById(userId.Value);
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
