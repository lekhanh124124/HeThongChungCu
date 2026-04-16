# HeThongChungCu

WebAPI cho hệ thống quản lý chung cư, xây dựng trên **.NET 8** theo **Clean Architecture**.

## Tổng quan

- Kiến trúc: Clean Architecture (Domain / Application / Infrastructure / WebAPI)
- Pattern: CQRS (Command/Query repositories) + MediatR pipeline behaviors
- Database: **SQL Server** (EF Core migrations)
- Query: **Dapper** (các `QueryRepository`)
- Auth: **JWT Bearer**
- Realtime: **SignalR** (`/notifications`)
- Docs API: **Swagger** + API Versioning (query string `api-version`)
- Logging: **Serilog** (Console + File; Application Insights khi non-Development)
- File storage: **Azure Blob Storage** (bắt buộc cấu hình để chạy)
- Health checks: `/health`, `/health/live`, `/health/ready`

Một số module hiện có nằm trong `src/HeThongChungCu.Application/Features/` như: Auth, Toà nhà, Tầng, Căn hộ, Cư dân, Quản lý cư trú, Phương tiện, Dịch vụ, Thông báo, Upload media, ...

## Tài liệu

- Checklist yêu cầu (đã có/chưa có): [docs/requirements_checklist.md](docs/requirements_checklist.md)

## Cấu trúc thư mục

```text
src/
	HeThongChungCu.Domain/         # Entities, ValueObjects, Enums, Domain Services, Errors/Exceptions
	HeThongChungCu.Application/    # Use cases (Features), CQRS, Behaviors, Validators, Interfaces
	HeThongChungCu.Infrastructure/ # Persistence (EF Core + Dapper), Auth, Email, FileStorage, Qdrant, Notifications, HealthChecks
	HeThongChungCu.WebAPI/         # API host, Controllers, Middlewares, Swagger, Static files
```

## Yêu cầu môi trường

- .NET SDK 8.x
- SQL Server (LocalDB/Express/Developer đều được)
- Azure Blob Storage hoặc **Azurite** (local) *(bắt buộc, vì tầng Infrastructure sẽ throw nếu thiếu connection string)*
- (Tuỳ chọn) Qdrant (Vector DB) nếu bạn dùng các tính năng liên quan

## Cấu hình

Các cấu hình chính nằm ở:

- `src/HeThongChungCu.WebAPI/appsettings.json`
- `src/HeThongChungCu.WebAPI/appsettings.Development.json`

### Các key quan trọng

- `ConnectionStrings:DefaultConnection` (SQL Server) — **bắt buộc**
- `JwtSettings:Secret` — **bắt buộc** (không được rỗng)
- `FileStorageSettings:ConnectionString` (Azure Blob Storage) — **bắt buộc**
- `ApplicationInsights:ConnectionString` — tuỳ chọn (chủ yếu dùng khi non-Development)
- `Email:*` — tuỳ chọn (SMTP; bật/tắt bằng `Email:EnableSending`)
- `Qdrant:Host`, `Qdrant:Port` — tuỳ chọn (mặc định `localhost:6334` gRPC)

Ví dụ cấu hình tối thiểu (gợi ý) cho `appsettings.Development.json`:

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Server=localhost;Database=HeThongChungCu;Trusted_Connection=True;TrustServerCertificate=True;"
	},
	"JwtSettings": {
		"Secret": "<your-very-long-secret>",
		"Issuer": "HeThongChungCu",
		"Audience": "HeThongChungCuClients",
		"ExpiryMinutes": 60
	},
	"FileStorageSettings": {
		"ConnectionString": "UseDevelopmentStorage=true",
		"UserAvatarContainer": "anh-dai-dien-nguoi-dung",
		"BuildingContainer": "hinh-anh-toa-nha",
		"ApartmentContainer": "hinh-anh-can-ho",
		"DocumentContainer": "tai-lieu-cu-tru",
		"VehicleContainer": "tai-lieu-phuong-tien",
		"StaffDocumentContainer": "tai-lieu-nhan-vien",
		"PartnerDocumentContainer": "tai-lieu-doi-tac",
		"CleanupIntervalHours": 1,
		"UnusedFileThresholdHours": 1
	},
	"Qdrant": {
		"Host": "localhost",
		"Port": "6334"
	}
}
```

Ghi chú:

- `UseDevelopmentStorage=true` phù hợp khi bạn chạy **Azurite**.
- Nếu không dùng Email trong môi trường dev, có thể đặt `Email:EnableSending=false`.

## Database (EF Core migrations)

Dự án đã có migrations tại `src/HeThongChungCu.Infrastructure/Persistence/Migrations/`.

Lệnh apply migrations (gợi ý):

```bash
dotnet ef database update --project src/HeThongChungCu.Infrastructure --startup-project src/HeThongChungCu.WebAPI --context AppDbContext
```

Khi app khởi động, `ApplicationDbContextInitialiser` sẽ kiểm tra khả năng kết nối DB và log cảnh báo nếu chưa apply migrations.

## Chạy dự án

### Run (watch)

```bash
dotnet watch run --project src/HeThongChungCu.WebAPI/HeThongChungCu.WebAPI.csproj
```

Mặc định profile `Dev` chạy ở `http://localhost:5000` (xem `src/HeThongChungCu.WebAPI/Properties/launchSettings.json`).

### Swagger

- Swagger UI: `http://localhost:5000/swagger`
- API version: truyền qua query string `api-version`, ví dụ `?api-version=1.0`

### Health checks

- Live: `/health/live` (chỉ kiểm tra cơ bản)
- Ready: `/health/ready` (bao gồm DB, migrations, Qdrant)
- Tổng hợp: `/health`

### SignalR Notifications

- Hub endpoint: `/notifications`
- JWT cho SignalR có thể truyền qua query string `access_token` (đã cấu hình trong `JwtBearerEvents.OnMessageReceived`).

## Logging

- Serilog ghi log ra console và file rolling theo ngày tại `src/HeThongChungCu.WebAPI/Logs/` (giữ tối đa 7 ngày).
- Khi môi trường **không phải Development**, Serilog sẽ đẩy log sang Application Insights nếu được cấu hình.

## Build / Publish

```bash
dotnet build src/HeThongChungCu.WebAPI/HeThongChungCu.WebAPI.csproj
dotnet publish src/HeThongChungCu.WebAPI/HeThongChungCu.WebAPI.csproj -c Release
```

Trong VS Code workspace hiện có sẵn tasks: `build`, `watch`, `publish`.
