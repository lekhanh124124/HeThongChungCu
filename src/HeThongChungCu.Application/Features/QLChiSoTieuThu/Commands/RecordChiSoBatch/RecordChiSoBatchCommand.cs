using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.RecordChiSoBatch;

public record RecordChiSoBatchCommand(
    List<ChiSoBatchItemDto> Items,
    int Thang,
    int Nam,
    DateTimeOffset NgayGhiNhan) : ICommand<int>;
