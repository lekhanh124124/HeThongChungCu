using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.NghiemThuThiCong;

public record NghiemThuThiCongCommand(int Id) : ICommand<YeuCauThiCongResponse>;
