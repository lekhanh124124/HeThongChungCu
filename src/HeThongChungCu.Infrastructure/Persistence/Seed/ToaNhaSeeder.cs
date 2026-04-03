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
                    Ten = "Skyline Residence (Block A)",
                    Block = "A",
                    DiaChi = "156 Tôn Đức Thắng, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh",
                    MoTa = "Là tòa tháp mặt tiền hướng trực diện ra sông Sài Gòn. Skyline Residence sở hữu vị trí đắc địa nhất khu phức hợp với hệ thống kính Low-E tràn viền. Tòa tháp này tập trung các căn hộ Penthouse sang trọng, hồ bơi vô cực trên tầng thượng và sảnh đón tiếp chuẩn 5 sao dành riêng cho giới thượng lưu."
                },
                new {
                    Ma = "HRP",
                    Ten = "Harmony Point (Block B)",
                    Block = "B",
                    DiaChi = "156 Tôn Đức Thắng, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh",
                    MoTa = "Nằm tại trung tâm nội khu, Harmony Point kết nối trực tiếp với khối đế thương mại và quảng trường nhạc nước. Đây là tòa tháp sôi động nhất, phù hợp cho các gia đình trẻ nhờ ưu thế sát cạnh khu vui chơi trẻ em, rạp chiếu phim và hệ thống nhà hàng cao cấp ngay dưới chân tòa nhà."
                },
                new {
                    Ma = "EMG",
                    Ten = "Emerald Garden (Block C)",
                    Block = "C",
                    DiaChi = "156 Tôn Đức Thắng, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh",
                    MoTa = "Tòa tháp nằm ở góc yên tĩnh nhất của dự án, được bao bọc bởi hệ thống vườn treo và công viên nội khu. Emerald Garden chú trọng vào không gian sống xanh và chăm sóc sức khỏe với khu vực Yoga ngoài trời, phòng Gym chuyên nghiệp và hồ bơi khoáng mặn nội khu, mang lại sự riêng tư tuyệt đối cho cư dân."
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
                            maCanHo: $"{toaNha.MaToaNha}-{apartmentNum}",
                            tenCanHo: $"Phòng {bData.Block}{apartmentNum}",
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
