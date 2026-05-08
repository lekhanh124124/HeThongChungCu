using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.PublishKhaoSat;

public record PublishKhaoSatCommand(int Id) : ICommand<KhaoSatResponse>;
