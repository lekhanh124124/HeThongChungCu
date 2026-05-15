using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetBaoCaoCongNoToaNha;
using HeThongChungCu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.ExportCongNoToaNhaExcel;

public class ExportCongNoToaNhaExcelQueryHandler : IQueryHandler<ExportCongNoToaNhaExcelQuery, ExportFileResponse>
{
    private readonly IQuyThuChiQueryRepository _queryRepository;
    private readonly IExcelService _excelService;

    public ExportCongNoToaNhaExcelQueryHandler(
        IQuyThuChiQueryRepository queryRepository,
        IExcelService excelService)
    {
        _queryRepository = queryRepository;
        _excelService = excelService;
    }

    public async Task<Result<ExportFileResponse>> Handle(ExportCongNoToaNhaExcelQuery request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        int thang = request.Thang ?? now.Month;
        int nam = request.Nam ?? now.Year;

        // Use standard specification to fetch the report
        var spec = new GetBaoCaoCongNoToaNhaSpecification(thang, nam);
        var list = await _queryRepository.GetBaoCaoCongNoToaNhaAsync(spec, cancellationToken);

        if (list == null || list.Count == 0)
        {
            return Result.Failure<ExportFileResponse>(new Error("Export.Empty", "Không có dữ liệu công nợ tòa nhà để xuất Excel."));
        }

        var excelItems = new List<object>();
        foreach (var x in list)
        {
            excelItems.Add(new
            {
                Tòa_Nhà = x.TenToaNha,
                Tổng_Số_Căn_Hộ = x.TongSoCanHo,
                Số_Căn_Hộ_Nợ_Phí = x.SoCanHoNoPhi,
                Nợ_Đầu_Kỳ_Tòa_Nhà = x.TongNoDauKy,
                Phát_Sinh_Trong_Kỳ = x.TongPhatSinh,
                Đã_Thu_Trong_Kỳ = x.TongDaThu,
                Còn_Nợ_Lại = x.TongNoConLai,
                Tỷ_Lệ_Thu_Hồi_Phần_Trăm = x.TyLeThuHoi
            });
        }

        var bytes = _excelService.CreateTemplate(excelItems, $"Công nợ tòa nhà T{thang}-{nam}");

        return Result.Success(new ExportFileResponse
        {
            Data = bytes,
            FileName = $"Bao_Cao_Cong_No_Toa_Nha_T{thang}_{nam}_{DateTime.Now:yyyyMMddHHmmss}.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });
    }
}
