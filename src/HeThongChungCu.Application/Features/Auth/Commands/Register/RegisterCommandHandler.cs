using System.Security.Cryptography;
using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
using HeThongChungCu.Application.Features.Auth.DTOs;

namespace HeThongChungCu.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserEFRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserEFRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var usernameExists = await _userRepository.AnyAsync(u => u.Username == request.Username, cancellationToken);
        if (usernameExists)
        {
            return Result.Failure<AuthResponse>(UserErrors.UsernameAlreadyExists);
        }

        var hashedPassword = _passwordHasher.HashPassword(request.Password);
        var user = new User(request.Username, request.Email, hashedPassword, request.FirstName, request.LastName, request.PhoneNumber, request.IdCard, request.Dob, request.GioiTinhId);

        user.ChangeRole(Role.Resident);

        user.AddDomainEvent(new Domain.Events.UserRegisteredEvent(user.Id, user.Username));

        await _userRepository.AddAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = new List<string> { Role.Resident.Name };
        var accessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Username, roles);
        var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var refreshToken = Tokens.CreateRefreshToken(user.Id, refreshTokenString, _dateTimeProvider.UtcNow.AddDays(7));
        user.AddToken(refreshToken);

        return Result.Success(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshTokenString
        });
    }
}
