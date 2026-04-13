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

            var admin = await context.TaiKhoan.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Email.Value == "admin@gmail.com");
            var adminId = admin?.Id ?? 0;

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
                    block: bData.Block,
                    diaChi: bData.DiaChi,
                    moTa: bData.MoTa,
                    trangThaiToaNhaId: TrangThaiToaNha.DangHoatDong
                );

                if (adminId != 0) toaNha.SetCreated(adminId, DateTimeOffset.Now);

                await context.ToaNhas.AddAsync(toaNha);

                // 1. Each building has 2 basements
                for (int i = 1; i <= 2; i++)
                {
                    var tang = toaNha.AddTang($"B{i}-{toaNha.MaToaNha}", $"Tầng hầm B{i}", LoaiTang.TangHam);
                    if (adminId != 0) tang.SetCreated(adminId, DateTimeOffset.Now);
                }

                // 2. Each building has exactly 8 floors
                for (int f = 1; f <= 8; f++)
                {
                    var tang = toaNha.AddTang($"F{f}-{toaNha.MaToaNha}", $"Tầng {f}", LoaiTang.TangLau);
                    if (adminId != 0) tang.SetCreated(adminId, DateTimeOffset.Now);
                }

                // IMPORTANT: Save now to generate IDs for ToaNha and Tangs 
                // so we can use them for CanHo.TangId
                DatabaseSeeder.ClearAllDomainEvents(context);
                await context.SaveChangesAsync();

                // 3. Create apartments for each floor
                var maxFloor = toaNha.Tangs
                    .Where(t => t.LoaiTangId == LoaiTang.TangLau)
                    .Select(t => int.Parse(t.TenTang.Split(' ')[1]))
                    .Max();

                foreach (var tang in toaNha.Tangs)
                {
                    if (tang.LoaiTangId != LoaiTang.TangLau) continue;

                    // Extract floor number from name (e.g., "Tầng 1" -> 1)
                    int floorNum = int.Parse(tang.TenTang.Split(' ')[1]);

                    // Random 7-10 apartments per floor
                    var faker = new Faker("vi");
                    int roomsCount = faker.Random.Int(7, 10);
                    for (int a = 1; a <= roomsCount; a++)
                    {
                        var apartmentNum = $"{floorNum}{a:D2}";
                        var status = faker.Random.WeightedRandom(
                            [TrangThaiCanHo.DangTrong, TrangThaiCanHo.ChuaBanGiao, TrangThaiCanHo.DangThiCong],
                            [0.85f, 0.10f, 0.05f]
                        );

                        // Logic-based Apartment Type selection
                        var loaiCanHo = LoaiCanHo.Standard;
                        var dienTich = Math.Round(faker.Random.Decimal(55, 90), 1);
                        var soPhongNgu = faker.Random.Int(1, 2);
                        var soPhongTam = faker.Random.Int(1, 1);

                        if (floorNum == 1)
                        {
                            // Floor 1 has a 40% chance of being a Shophouse
                            if (faker.Random.Bool(0.4f))
                            {
                                loaiCanHo = LoaiCanHo.Shophouse;
                                dienTich = Math.Round(faker.Random.Decimal(100, 200), 1);
                                soPhongNgu = 1; // Shophouses usually have less bedrooms, more open space
                                soPhongTam = 2;
                            }
                        }
                        else if (floorNum == maxFloor)
                        {
                            // Top floor has a 50% chance of being a Penthouse
                            if (faker.Random.Bool(0.5f))
                            {
                                loaiCanHo = LoaiCanHo.Penthouse;
                                dienTich = Math.Round(faker.Random.Decimal(180, 350), 1);
                                soPhongNgu = faker.Random.Int(3, 5);
                                soPhongTam = faker.Random.Int(3, 4);
                            }
                        }
                        else
                        {
                            // Other floors: Standard (70%) or Studio (30%)
                            if (faker.Random.Bool(0.3f))
                            {
                                loaiCanHo = LoaiCanHo.Studio;
                                dienTich = Math.Round(faker.Random.Decimal(35, 50), 1);
                                soPhongNgu = 1;
                                soPhongTam = 1;
                            }
                            else
                            {
                                loaiCanHo = LoaiCanHo.Standard;
                                dienTich = Math.Round(faker.Random.Decimal(65, 110), 1);
                                soPhongNgu = faker.Random.Int(2, 3);
                                soPhongTam = faker.Random.Int(1, 2);
                            }
                        }

                        var canHo = new CanHo(
                            tangId: tang.Id,
                            maCanHo: $"{toaNha.MaToaNha}-{apartmentNum}",
                            tenCanHo: $"Phòng {bData.Block}{apartmentNum}",
                            dienTich: dienTich,
                            soPhongNgu: soPhongNgu,
                            soPhongTam: soPhongTam,
                            loaiCanHoId: loaiCanHo,
                            tinhTrangCanHoId: status
                        );

                        if (adminId != 0) canHo.SetCreated(adminId, DateTimeOffset.Now);

                        await context.CanHos.AddAsync(canHo);
                    }
                }
            }

            DatabaseSeeder.ClearAllDomainEvents(context);
            await context.SaveChangesAsync();
            logger.LogInformation("Finished seeding 3 buildings with floors and apartments.");
        }
    }
}
