using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.ImportChiSo;

public record ImportChiSoCommand(
    Stream FileStream, 
    int Thang, 
    int Nam, 
    DateTimeOffset NgayGhiNhan) : ICommand<ChiSoBatchResultResponse>;
