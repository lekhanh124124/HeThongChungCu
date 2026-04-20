using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.PheDuyetYeuCauSuaChua;

public record PheDuyetYeuCauSuaChuaCommand(int Id) : ICommand<YeuCauSuaChuaDetailResponse>;
