using HeThongChungCu.Application.Features.QuanHeCuTru.DTOs;

namespace HeThongChungCu.Application.Features.QuanHeCuTru.Queries.LayUserByUsername;

public record GetUserByUsernameQuery(string Username) : IQuery<SearchUserByUsernameResponse>;
