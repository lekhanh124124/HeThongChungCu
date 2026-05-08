using System;
using System.Collections.Generic;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Application.Features.QLKhaoSat.Commands.CreateKhaoSat;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.UpdateKhaoSat;

public record UpdateKhaoSatCommand : ICommand<KhaoSatResponse>
{
    public int Id { get; init; }
    public string TieuDe { get; init; } = string.Empty;
    public string MoTa { get; init; } = string.Empty;
    public int LoaiKhaoSatId { get; init; }
    public int CoCheTinhDiemId { get; init; }
    public DateTimeOffset NgayBatDau { get; init; }
    public DateTimeOffset NgayKetThuc { get; init; }
    public decimal TyleThamGiaToiThieu { get; init; }
    public decimal TyLeDongYToiThieu { get; init; }
    public bool IsAnDanh { get; init; }
    public List<CreateCauHoiDto> CauHois { get; init; } = [];
}
