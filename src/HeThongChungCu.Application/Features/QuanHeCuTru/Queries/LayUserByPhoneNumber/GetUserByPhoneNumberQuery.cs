using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByPhoneNumber;

public record GetUserByPhoneNumberQuery(string PhoneNumber) : IQuery<SearchUserByUsernameResponse>;
