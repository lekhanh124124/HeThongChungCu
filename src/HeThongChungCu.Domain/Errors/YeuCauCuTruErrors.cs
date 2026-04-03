namespace HeThongChungCu.Domain.Errors;

using HeThongChungCu.Domain.Common;

public static class YeuCauCuTruErrors
{
    public static readonly Error NotFound = Error.NotFound("Yêu cầu cư trú");

    public static readonly Error Forbidden = Error.Forbidden("thực hiện hành động này");

    public static Error NotFoundById(int id) => Error.NotFound("Yêu cầu cư trú", id);

    public static Error NotFoundByIds(List<int> ids) => Error.NotFound("Yêu cầu cư trú", string.Join(", ", ids));

    public static Error InvalidType(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Loại yêu cầu", allowedValues);

    public static readonly Error LyDoNotEmpty = Error.NotEmpty("Lý do");
    public static readonly Error CanHoIdRange = Error.Range("Căn hộ", 1, int.MaxValue);
    public static readonly Error FileIdRange = Error.Range("Tệp tin", 1, int.MaxValue);
    public static readonly Error YeuCauCuTruIdRange = Error.Range("Yêu cầu cư trú", 1, int.MaxValue);

    public static Error InvalidDocumentType(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Loại giấy tờ", allowedValues);

    public static Error InvalidRelationType(IEnumerable<string> allowedValues) => 
        Error.InvalidType("Loại quan hệ cư trú", allowedValues);

    public static readonly Error QuanHeIdRange = Error.Range("Quan hệ cư trú", 1, int.MaxValue);
    public static readonly Error GiayToIdRange = Error.Range("Giấy tờ", 1, int.MaxValue);
    public static readonly Error SoGiayToNotEmpty = Error.NotEmpty("Số giấy tờ");
    public static readonly Error SoGiayToMaxLength = Error.MaxLength("Số giấy tờ", 100);
    public static readonly Error FileIdsNotEmpty = Error.NotEmpty("Tệp tin đính kèm");
    public static readonly Error LoaiYeuCauNotEmpty = Error.NotEmpty("Loại yêu cầu");
    public static readonly Error LyDoMaxLength = Error.MaxLength("Lý do", 500);
    public static readonly Error YeuCauCuTruIdsNotEmpty = Error.NotEmpty("Danh sách yêu cầu cư trú");
}
