using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.AddTepThiCong;

public record AddTepThiCongCommand(
    int Id,
    List<int> TepIds) : ICommand<YeuCauThiCongResponse>;
