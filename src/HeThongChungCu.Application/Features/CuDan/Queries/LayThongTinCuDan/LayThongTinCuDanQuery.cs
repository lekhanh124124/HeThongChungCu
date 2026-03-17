using HeThongChungCu.Application.Features.CuDan.DTOs;

namespace HeThongChungCu.Application.Features.CuDan.Queries.LayThongTinCuDan;

public record LayThongTinCuDanQuery(int UserId, int QuanHeCuTruId) : IQuery<LayThongTinCuDanResponse>;
