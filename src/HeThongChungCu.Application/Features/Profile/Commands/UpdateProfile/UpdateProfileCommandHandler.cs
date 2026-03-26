using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.Profile.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, UserProfileDetailResponse>
{
    private readonly INguoiDungEFRepository _userRepository;
    private readonly ITaiKhoanEFRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(
        INguoiDungEFRepository userRepository,
        ITaiKhoanEFRepository accountRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserProfileDetailResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.AccountId is null)
        {
            return Result.Failure<UserProfileDetailResponse>(AuthErrors.InvalidCredentials);
        }

        var account = await _accountRepository.GetWithAvatarAsync(_currentUserService.AccountId.Value, cancellationToken);
        if (account is null)
        {
            return Result.Failure<UserProfileDetailResponse>(AuthErrors.InvalidCredentials);
        }

        if (account.NguoiDungId is null)
        {
            return Result.Failure<UserProfileDetailResponse>(UserErrors.NotFound);
        }
 
        var user = await _userRepository.GetByIdAsync(account.NguoiDungId.Value, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UserProfileDetailResponse>(UserErrors.NotFound);
        }

        // Kiểm tra Email có tồn tại không nếu thay đổi (Email hiện tại nằm ở Account)
        if (account != null && account.Email != request.Email)
        {
            var emailExists = await _accountRepository.AnyAsync(a => a.Email == request.Email, cancellationToken);
            if (emailExists)
            {
                return Result.Failure<UserProfileDetailResponse>(UserErrors.EmailAlreadyExists);
            }
            account.UpdateEmail(request.Email); // Assumes Account has UpdateEmail method or similar. I'll check.
        }

        // Kiểm tra Số điện thoại có tồn tại không nếu thay đổi (Số điện thoại hiện tại đã chuyển sang User)
        if (user.SoDienThoai != request.PhoneNumber)
        {
            var phoneNumberExists = await _userRepository.AnyAsync(u => u.SoDienThoai == request.PhoneNumber, cancellationToken);
            if (phoneNumberExists)
            {
                return Result.Failure<UserProfileDetailResponse>(UserErrors.PhoneNumberAlreadyExists);
            }
        }

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.Dob,
            GioiTinh.FromValue(request.GioiTinhId)!,
            request.DiaChi,
            user.CCCD,
            request.PhoneNumber);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Get roles for response
        var roles = account?.PhanQuyens.Select(pq => pq.RoleId.Name).ToList() ?? [];

        var response = new UserProfileDetailResponse
        {
            Id = user.Id,
            Username = account?.TenDangNhap ?? string.Empty,
            Email = account?.Email ?? string.Empty,
            FirstName = user.Ten,
            LastName = user.Ho,
            PhoneNumber = user.SoDienThoai ?? string.Empty,
            Dob = user.NgaySinh,
            DiaChi = user.DiaChi,
            GioiTinhId = user.GioiTinhId.Value,
            GioiTinhName = user.GioiTinhId.Name,
            Roles = roles,
            AnhDaiDienUrl = account!.AnhDaiDien?.FileUrl ?? string.Empty
        };

        return Result.Success(response);
    }
}
