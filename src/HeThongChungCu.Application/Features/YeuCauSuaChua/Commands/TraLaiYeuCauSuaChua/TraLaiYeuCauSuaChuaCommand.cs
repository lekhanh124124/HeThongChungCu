using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauSuaChua.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauSuaChua.Commands.TraLaiYeuCauSuaChua;

public record TraLaiYeuCauSuaChuaCommand(int Id, string LyDo) : ICommand<YeuCauSuaChuaDetailResponse>;
