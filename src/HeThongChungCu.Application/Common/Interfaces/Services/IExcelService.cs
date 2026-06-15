using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IExcelService
{
    byte[] CreateTemplate<T>(IEnumerable<T> data, string sheetName);
    List<T> Import<T>(Stream stream) where T : new();
    byte[] ExportPhieuBaoTri(PhieuBaoTriDetailResponse phieu);
}
