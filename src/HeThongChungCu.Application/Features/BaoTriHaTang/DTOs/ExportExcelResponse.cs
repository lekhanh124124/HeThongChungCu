namespace HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;

public record ExportExcelResponse(byte[] Content, string ContentType, string FileName);
