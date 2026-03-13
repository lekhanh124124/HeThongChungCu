using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;
// Removed invalid using

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByPhoneNumber;

public class GetUserByPhoneNumberQueryHandler : IQueryHandler<GetUserByPhoneNumberQuery, SearchUserByUsernameResponse>
{
    private readonly IUserDapperRepository _userRepository;

    public GetUserByPhoneNumberQueryHandler(IUserDapperRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<SearchUserByUsernameResponse>> Handle(GetUserByPhoneNumberQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetUserByPhoneNumberSpecification(
            phoneNumber: request.PhoneNumber,
            roleIds: new List<int> { Role.Guest.Value, Role.Resident.Value });

        var user = await _userRepository.SearchResidentOrGuestByPhoneNumberAsync(spec, cancellationToken);

        if (user is null)
        {
            return Result.Failure<SearchUserByUsernameResponse>(UserErrors.NotFoundByPhoneNumber(request.PhoneNumber));
        }

        return Result.Success(user);
    }
}
