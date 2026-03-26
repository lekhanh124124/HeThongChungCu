using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.QLCuTru.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLCuTru.Commands.DinhDanhNguoiDung;

public class LienKetTaiKhoanCommandHandler : ICommandHandler<LienKetTaiKhoanCommand, UserInfoResponse>
{
    private readonly ITaiKhoanEFRepository _accountRepository;
    private readonly INguoiDungEFRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LienKetTaiKhoanCommandHandler(
        ITaiKhoanEFRepository accountRepository,
        INguoiDungEFRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserInfoResponse>> Handle(LienKetTaiKhoanCommand request, CancellationToken cancellationToken)
    {
        // 1. Find account by Email
        var account = await _accountRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (account is null)
        {
            return Result.Failure<UserInfoResponse>(AuthErrors.AccountNotFound);
        }

        // 2. Link account to user if not already linked
        if (account.NguoiDungId != null && account.NguoiDungId != request.UserId)
        {
            return Result.Failure<UserInfoResponse>(new Error("Auth.AccountAlreadyLinked", "Tài khoản đã được liên kết với một người dùng khác."));
        }

        if (account.NguoiDungId == null)
        {
            account.LinkToUser(request.UserId);
        }

        // 3. Promote role if Guest
        var roles = account.PhanQuyens.Select(pq => pq.RoleId).ToList();
        if (roles.Contains(Role.Guest) && !roles.Contains(Role.Resident))
        {
            account.RemoveRole(Role.Guest);
            account.AddRole(Role.Resident);
        }

        // 4. Revoke any pending identification tokens for this account
        var pendingTokens = account.Tokens.Where(t => t.TokenType == TokenType.UserCode && t.IsActive).ToList();
        foreach (var token in pendingTokens)
        {
            account.RevokeToken(token.TokenHash, DateTimeOffset.UtcNow, ReasonRevoked.AdminAction);
        }

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return User Info
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UserInfoResponse>(UserErrors.NotFoundById(request.UserId));
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
            Documents = user.TaiLieu.Select(d => new TaiLieuResponse
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
