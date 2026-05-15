using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoCanHo;
using HeThongChungCu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.ExportCongNoCanHoExcel;

public class ExportCongNoCanHoExcelQueryHandler : IQueryHandler<ExportCongNoCanHoExcelQuery, ExportFileResponse>
{
    private readonly IQuyThuChiQueryRepository _queryRepository;
    private readonly IExcelService _excelService;

    public ExportCongNoCanHoExcelQueryHandler(
        IQuyThuChiQueryRepository queryRepository,
        IExcelService excelService)
    {
        _queryRepository = queryRepository;
        _excelService = excelService;
    }

    public async Task<Result<ExportFileResponse>> Handle(ExportCongNoCanHoExcelQuery request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        int thang = request.Thang ?? now.Month;
        int nam = request.Nam ?? now.Year;

        // Use standard specification to fetch the report
        var spec = new GetBaoCaoCongNoCanHoSpecification(request.ToaNhaId, thang, nam);
        var list = await _queryRepository.GetBaoCaoCongNoCanHoAsync(spec, cancellationToken);

        if (list == null || list.Count == 0)
        {
            return Result.Failure<ExportFileResponse>(new Error("Export.Empty", "Không có dữ liệu công nợ căn hộ để xuất Excel."));
        }

        var excelItems = new List<object>();
        foreach (var x in list)
        {
            excelItems.Add(new
            {
                Mã_Căn_Hộ = x.MaCanHo,
                Tòa_Nhà = x.TenToaNha,
                Tầng = x.TenTang,
                Chủ_Hộ = x.TenChuHo,
                Nợ_Đầu_Kỳ = x.NoDauKy,
                Phát_Sinh_Trong_Kỳ = x.PhatSinhTrongKy,
                Đã_Thanh_Toán = x.DaThanhToanTrongKy,
                Nợ_Cuối_Kỳ = x.NoCuoiKy
            });
        }

        var bytes = _excelService.CreateTemplate(excelItems, $"Công nợ căn hộ T{thang}-{nam}");

        return Result.Success(new ExportFileResponse
        {
            Data = bytes,
            FileName = $"Bao_Cao_Cong_No_Can_Ho_T{thang}_{nam}_{DateTime.Now:yyyyMMddHHmmss}.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });
    }
}
