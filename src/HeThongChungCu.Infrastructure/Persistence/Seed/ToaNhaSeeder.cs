using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class ToaNhaSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (!await context.ToaNhas.AnyAsync())
        {
            logger.LogInformation("Seeding ToaNhas, Tangs, and CanHos (Hardcoded)...");

            var faker = new Faker("vi");
            var buildings = new[]
            {
                new {
                    Ma = "SKR",
                    Ten = "Skyline Residence",
                    DiaChi = "156 Tôn Đức Thắng, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh",
                    MoTa = "Khu phức hợp căn hộ Skyline Residence tọa lạc tại vị trí đắc địa ven sông Sài Gòn. Tòa nhà cao cấp này mang đến không gian sống sang trọng với tầm nhìn toàn cảnh thành phố, hệ thống cửa kính chống tia UV, và các tiện ích chuẩn 5 sao bao gồm hồ bơi vô cực, vườn treo trên không và khu spa cao cấp."
                },
                new {
                    Ma = "HRP",
                    Ten = "Harmony Point",
                    DiaChi = "45 Mai Chí Thọ, Phường An Phú, TP. Thủ Đức, TP. Hồ Chí Minh",
                    MoTa = "Tòa nhà Harmony Point là biểu tượng của sự hiện đại và tiện nghi tại khu Đông Sài Gòn. Với thiết kế kiến trúc xanh, tối ưu hóa ánh sáng tự nhiên, tòa nhà cung cấp môi trường sống lý tưởng với công viên nội khu rộng lớn, khu vui chơi trẻ em hiện đại và trung tâm thương mại sầm uất ngay dưới chân tòa nhà."
                },
                new {
                    Ma = "EMG",
                    Ten = "Emerald Garden",
                    DiaChi = "102 Nguyễn Tất Thành, Phường 13, Quận 4, TP. Hồ Chí Minh",
                    MoTa = "Emerald Garden kết hợp giữa không gian sống thanh bình và sự năng động của trung tâm thành phố. Tòa nhà nổi bật với hệ thống vườn đứng bao quanh, khu gym chuyên nghiệp, sân tennis và đội ngũ an ninh 24/7, mang lại sự an tâm tuyệt đối cho cư dân."
                }
            };

            foreach (var bData in buildings)
            {
                var toaNha = new ToaNha(
                    maToaNha: bData.Ma,
                    tenToaNha: bData.Ten,
                    diaChi: bData.DiaChi,
                    moTa: bData.MoTa,
                    trangThaiToaNhaId: TrangThaiToaNha.DangHoatDong
                );

                await context.ToaNhas.AddAsync(toaNha);
                await context.SaveChangesAsync();

                // Each building has 2 basements
                for (int i = 1; i <= 2; i++)
                {
                    toaNha.AddTang($"B{i}-{toaNha.MaToaNha}", $"Tầng hầm B{i}", LoaiTang.TangHam);
                }

                // Each building has exactly 8 floors
                for (int f = 1; f <= 8; f++)
                {
                    var tang = toaNha.AddTang($"F{f}-{toaNha.MaToaNha}", $"Tầng {f}", LoaiTang.TangLau);
                    await context.SaveChangesAsync();

                    // Random 7-10 apartments per floor
                    int roomsCount = faker.Random.Int(7, 10);
                    for (int a = 1; a <= roomsCount; a++)
                    {
                        var apartmentNum = $"{f}{a:D2}";
                        var canHo = new CanHo(
                            tangId: tang.Id,
                            maCanHo: $"{toaNha.MaToaNha}.{apartmentNum}",
                            tenCanHo: $"Phòng {apartmentNum}",
                            dienTich: Math.Round(faker.Random.Decimal(45, 120), 1),
                            soPhongNgu: faker.Random.Int(1, 3),
                            soPhongTam: faker.Random.Int(1, 2),
                            loaiCanHoId: faker.PickRandom(LoaiCanHo.GetAll().ToArray()),
                            tinhTrangCanHoId: TrangThaiCanHo.DangTrong
                        );
                        await context.CanHos.AddAsync(canHo);
                    }
                }

                await context.SaveChangesAsync();
            }

            logger.LogInformation("Finished seeding 3 hardcoded buildings with their floors and apartments.");
        }
    }
}
