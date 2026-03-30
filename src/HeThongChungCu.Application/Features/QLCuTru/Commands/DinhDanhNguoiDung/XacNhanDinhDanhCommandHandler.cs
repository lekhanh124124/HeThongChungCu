using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.DinhDanhNguoiDung;

public class XacNhanDinhDanhCommandHandler : ICommandHandler<XacNhanDinhDanhCommand, UserInfoResponse>
{
    private readonly ITaiKhoanEFRepository _accountRepository;
    private readonly INguoiDungEFRepository _userRepository;
    private readonly IHasherService _hasherService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public XacNhanDinhDanhCommandHandler(
        ITaiKhoanEFRepository accountRepository,
        INguoiDungEFRepository userRepository,
        IHasherService hasherService,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _hasherService = hasherService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserInfoResponse>> Handle(XacNhanDinhDanhCommand request, CancellationToken cancellationToken)
    {
        // 1. Get UserId from token via service
        var userId = _jwtTokenGenerator.GetUserIdFromToken(request.Token);
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

        var tokenEntity = account.Tokens.FirstOrDefault(t => t.TokenHash == tokenHash && t.TokenType == TokenType.UserCode);
        if (tokenEntity == null || !tokenEntity.IsActive)
        {
            return Result.Failure<UserInfoResponse>(AuthErrors.InvalidToken);
        }

        // 3. Link account to user if not already linked
        if (account.NguoiDungId != null && account.NguoiDungId != userId.Value)
        {
            return Result.Failure<UserInfoResponse>(new Error("Auth.AccountAlreadyLinked", "Tài khoản đã được liên kết với một người dùng khác."));
        }

        if (account.NguoiDungId == null)
        {
            account.LinkToUser(userId.Value);
        }

        // 4. Promote role if Guest
        var roles = account.PhanQuyens.Select(pq => pq.RoleId).ToList();
        if (roles.Contains(Role.Guest) && !roles.Contains(Role.Resident))
        {
            account.RemoveRole(Role.Guest);
            account.AddRole(Role.Resident);
        }

        // 5. Revoke token
        account.RevokeToken(tokenHash, DateTimeOffset.UtcNow, ReasonRevoked.UserAction);

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
            DiaChi = user.DiaChi,
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
