using HeThongChungCu.Application.Features.QLChiSoTieuThu.DTOs;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTaiChinh.Queries.GetNhatKyThuChi;
using HeThongChungCu.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLTaiChinh.Queries.ExportNhatKyThuChiExcel;

public class ExportNhatKyThuChiExcelQueryHandler : IQueryHandler<ExportNhatKyThuChiExcelQuery, ExportFileResponse>
{
    private readonly IQuyThuChiQueryRepository _queryRepository;
    private readonly IExcelService _excelService;

    public ExportNhatKyThuChiExcelQueryHandler(
        IQuyThuChiQueryRepository queryRepository,
        IExcelService excelService)
    {
        _queryRepository = queryRepository;
        _excelService = excelService;
    }

    public async Task<Result<ExportFileResponse>> Handle(ExportNhatKyThuChiExcelQuery request, CancellationToken cancellationToken)
    {
        // Use standard specification to filter and load all items
        var spec = new GetNhatKyThuChiSpecification(
            request.LoaiGiaoDichId,
            request.DichVuId,
            request.NhomThongKe,
            request.TuNgay,
            request.DenNgay,
            request.Keyword,
            "NgayGiaoDich",
            true, // isAsc
            1,
            int.MaxValue); // Fetch all for Excel export

        var data = await _queryRepository.GetNhatKyThuChiAsync(spec, cancellationToken);

        if (data.Items == null || !data.Items.Any())
        {
            return Result.Failure<ExportFileResponse>(new Error("Export.Empty", "Không có dữ liệu giao dịch để xuất Excel."));
        }

        var excelItems = new List<object>();
        foreach (var x in data.Items)
        {
            excelItems.Add(new
            {
                Mã_Giao_Dịch = x.MaGiaoDich,
                Loại_Giao_Dịch = x.TenLoaiGiaoDich,
                Tổng_Số_Tiền = x.TongSoTien,
                Ngày_Giao_Dịch = x.NgayGiaoDich.ToString("dd/MM/yyyy HH:mm"),
                Phương_Thức = x.TenPhuongThucThanhToan,
                Người_Giao_Dịch = x.NguoiGiaoDich,
                Chứng_Từ_Gốc = x.ChungTuGoc ?? ""
            });
        }

        var bytes = _excelService.CreateTemplate(excelItems, "Nhật ký quỹ");

        return Result.Success(new ExportFileResponse
        {
            Data = bytes,
            FileName = $"Nhat_Ky_Thu_Chi_Quy_{DateTime.Now:yyyyMMddHHmmss}.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });
    }
}
