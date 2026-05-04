namespace HeThongChungCu.Domain.Constants;

public static class ServiceCodeConstants
{
    // Dịch vụ thuê nhà
    public const string TIEN_THUE_NHA = "RENT_FEE";

    // Dịch vụ bắt buộc hệ thống
    public const string PHI_QUAN_LY = "MANAGEMENT_FEE";
    public const string PHI_BAO_TRI = "MAINTENANCE_FEE";

    // Dịch vụ gửi xe (Mặc định)
    public const string PK_MOTOR = "PK_MOTOR";
    public const string PK_CAR = "PK_CAR";
    public const string PK_BIKE = "PK_BIKE";
    public const string PK_EV = "PK_EV";

    // Dịch vụ điện nước
    public const string DIEN = "ELECTRICITY";
    public const string NUOC = "WATER";

    // Dịch vụ hệ thống (Lãi trễ hạn)
    public const string LAI_TRE_HAN = "LATE_INTEREST_FEE";
}
