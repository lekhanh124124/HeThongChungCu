using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.ThongBao.DTOs;

namespace HeThongChungCu.Application.Features.ThongBao.Commands.SendDemoNotification;

public record SendDemoNotificationCommand(int? UserId = null) : ICommand<ThongBaoResponse>;
