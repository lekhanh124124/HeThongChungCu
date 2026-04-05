using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;
using HeThongChungCu.Domain.Interfaces;
using HeThongChungCu.Domain.ValueObjects;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.DinhDanhNguoiDung;

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
        // 1. Get UserId from token via service
        var userId = _tokenService.GetUserIdFromToken(request.Token);
        if (userId == null)
        {
            return Result.Failure<UserInfoResponse>(AuthErrors.InvalidToken);
        }

        // 2. Find account by token hash
        var tokenHash = _hasherService.HashToken(request.Token);
        var account = await _accountRepository.GetByTokenAsync(tokenHash, TokenType.UserCode, cancellationToken);
        if (account is null)
        {
            return Result.Failure<UserInfoResponse>(AuthErrors.InvalidToken);
        }

        // 3. Verify token, link account and promote role via Domain Service
        var verifyResult = _identityService.VerifyAndLinkAccount(account, tokenHash, userId.Value, DateTimeOffset.UtcNow);
        if (verifyResult.IsFailure)
            return Result.Failure<UserInfoResponse>(verifyResult.Errors[0]);

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Return User Info
        var user = await _userRepository.GetByIdWithDocumentsAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UserInfoResponse>(UserErrors.NotFoundById(userId.Value));
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
