using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Features.Profile.DTOs;

namespace HeThongChungCu.Application.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, UserProfileDetailResponse>
{
    private readonly IUserEFRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(
        IUserEFRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserProfileDetailResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return Result.Failure<UserProfileDetailResponse>(AuthErrors.InvalidCredentials);
        }

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UserProfileDetailResponse>(UserErrors.NotFound);
        }

        // Kiểm tra CMND/CCCD có tồn tại không nếu thay đổi
        if (user.IdCard != request.IdCard)
        {
            var idCardExists = await _userRepository.AnyAsync(u => u.IdCard == request.IdCard, cancellationToken);
            if (idCardExists)
            {
                return Result.Failure<UserProfileDetailResponse>(UserErrors.IdCardAlreadyExists);
            }
        }

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.IdCard,
            request.Dob,
            request.GioiTinhId,
            request.DiaChi);

        _userRepository.Update(user);

        // TransactionBehavior will automatically commit if no exception is thrown, otherwise it will rollback
        var response = new UserProfileDetailResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            IdCard = user.IdCard,
            Dob = user.Dob,
            DiaChi = user.DiaChi,
            GioiTinhId = user.GioiTinhId,
            GioiTinhName = GioiTinh.FromValue(user.GioiTinhId)?.Name ?? string.Empty,
            RoleId = user.RoleId,
            RoleName = Role.FromValue(user.RoleId)?.Name ?? string.Empty,
            AnhDaiDienUrl = user.AnhDaiDienUrl ?? string.Empty
        };

        return Result.Success(response);
    }
}
