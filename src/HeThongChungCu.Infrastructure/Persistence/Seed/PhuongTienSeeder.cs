using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;

namespace HeThongChungCu.Infrastructure.Persistence.Seed;

public class PhuongTienSeeder
{
    private static readonly object _lock = new();
    private static readonly HashSet<string> _usedBienSos = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> _usedMaThes = new(StringComparer.OrdinalIgnoreCase);

    public static async Task InitializeAsync(AppDbContext context)
    {
        lock (_lock)
        {
            _usedBienSos.Clear();
            _usedMaThes.Clear();
        }

        var existingBienSos = await context.PhuongTiens.IgnoreQueryFilters().Select(pt => pt.BienSo).ToListAsync();
        var existingMaThes = await context.ThePhuongTiens.IgnoreQueryFilters().Select(t => t.MaThe).ToListAsync();

        lock (_lock)
        {
            foreach (var bs in existingBienSos) if (!string.IsNullOrEmpty(bs)) _usedBienSos.Add(bs);
            foreach (var mt in existingMaThes) if (!string.IsNullOrEmpty(mt)) _usedMaThes.Add(mt);
        }
    }

    public static async Task SeedAsync(AppDbContext context, ILogger logger, int soLuongPhuongTien)
    {
        logger.LogInformation("Seeding {Count} PhuongTiens with logic-based cards...", soLuongPhuongTien);

        var canHoIds = await context.CanHos.Select(c => c.Id).ToListAsync();
        if (canHoIds.Count == 0) return;

        var faker = new Faker("vi");
        var loaiPhuongTiens = LoaiPhuongTien.GetAll().ToArray();
        var trangThais = TrangThaiPhuongTien.GetAll().ToArray();

        var vehicleModels = new Dictionary<LoaiPhuongTien, string[]>
        {
            { LoaiPhuongTien.Oto, ["Toyota Camry", "Honda CR-V", "Mazda 3", "Hyundai SantaFe", "Mercedes E200", "VinFast Lux A"] },
            { LoaiPhuongTien.XeMay, ["Honda SH", "Honda Vision", "Yamaha Exciter", "Vespa Primavera", "Air Blade"] },
            { LoaiPhuongTien.XeDap, ["Giant Escape", "Thống Nhất", "Trek Marlin"] },
            { LoaiPhuongTien.XeDien, ["VinFast Klara", "Pega", "VinFast Vento"] }
        };

        for (int i = 0; i < soLuongPhuongTien; i++)
        {
            var loaiId = faker.PickRandom(loaiPhuongTiens);
            var model = faker.PickRandom(vehicleModels[loaiId]);
            var status = faker.PickRandom(trangThais);

            var pt = new PhuongTien(
                canHoId: faker.PickRandom(canHoIds),
                tenPhuongTien: $"{model} {GenerateBienSo(faker)}",
                loaiPhuongTienId: loaiId,
                bienSo: GenerateBienSo(faker),
                mauXe: model,
                hinhAnhs: null
            );

            await context.PhuongTiens.AddAsync(pt);

            // Status-based card logic
            if (status == TrangThaiPhuongTien.Active)
            {
                pt.AddThe(GenerateUniqueMaThe(faker), DateTimeOffset.UtcNow.AddMonths(-1));
            }
            else if (status == TrangThaiPhuongTien.Inactive)
            {
                // Inactive vehicles might still have old cards
                pt.AddThe(GenerateUniqueMaThe(faker), DateTimeOffset.UtcNow.AddMonths(-2));
                pt.Huy(DateTimeOffset.UtcNow);
            }
            else if (status == TrangThaiPhuongTien.Blocked)
            {
                pt.AddThe(GenerateUniqueMaThe(faker), DateTimeOffset.UtcNow.AddMonths(-3));
                pt.Khoa(DateTimeOffset.UtcNow);
            }
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Finished seeding PhuongTiens.");
    }

    private static string GenerateBienSo(Faker faker)
    {
        string bienSo;
        lock (_lock)
        {
            do
            {
                bienSo = $"{faker.Random.Int(29, 31)}{faker.Random.String2(1, "ABCDEFGHJK")}-{faker.Random.Int(100, 999)}.{faker.Random.Int(10, 99)}";
            } while (!_usedBienSos.Add(bienSo));
        }
        return bienSo;
    }

    public static string RegisterBienSo(string bienSo)
    {
        if (string.IsNullOrEmpty(bienSo)) return bienSo;
        lock (_lock)
        {
            _usedBienSos.Add(bienSo);
        }
        return bienSo;
    }

    public static string RegisterMaThe(string maThe)
    {
        if (string.IsNullOrEmpty(maThe)) return maThe;
        lock (_lock)
        {
            _usedMaThes.Add(maThe);
        }
        return maThe;
    }

    private static string GenerateUniqueMaThe(Faker faker, string prefix = "CARD-")
    {
        string maThe;
        lock (_lock)
        {
            do
            {
                maThe = faker.Random.Replace($"{prefix}##########");
            } while (!_usedMaThes.Add(maThe));
        }
        return maThe;
    }
}
