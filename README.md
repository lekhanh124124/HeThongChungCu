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


## Cấu trúc thư mục

```text
src/
	HeThongChungCu.Domain/         # Entities, ValueObjects, Enums, Domain Services, Errors/Exceptions
	HeThongChungCu.Application/    # Use cases (Features), CQRS, Behaviors, Validators, Interfaces
	HeThongChungCu.Infrastructure/ # Persistence (EF Core + Dapper), Auth, Email, FileStorage, Qdrant, Notifications, HealthChecks
	HeThongChungCu.WebAPI/         # API host, Controllers, Middlewares, Swagger, Static files
tests/
	HeThongChungCu.Application.UnitTests/         # Unit tests cho Application layer
	HeThongChungCu.Infrastructure.IntegrationTests/ # Integration tests cho Infrastructure layer
```

## Yêu cầu môi trường & Cấu hình

- .NET SDK 8.x
- SQL Server (LocalDB/Express/Developer đều được)
- Azure Blob Storage hoặc **Azurite** (local) *(bắt buộc, vì tầng Infrastructure sẽ throw nếu thiếu `FileStorageSettings:ConnectionString`)*
- (Tuỳ chọn) Qdrant (Vector DB) nếu bạn dùng các tính năng liên quan đến AI / Search

> **Lưu ý Cấu hình**: Trước khi khởi chạy ứng dụng, hãy kiểm tra và cấu hình các giá trị cần thiết trong `src/HeThongChungCu.WebAPI/appsettings.json` (hoặc `appsettings.Development.json`), đặc biệt là:
> - `ConnectionStrings:DefaultConnection`
> - `JwtSettings:Secret`
> - `FileStorageSettings:ConnectionString`


## Database (EF Core migrations)

Dự án đã có migrations tại `src/HeThongChungCu.Infrastructure/Persistence/Migrations/`.

Lệnh apply migrations (gợi ý):

```bash
dotnet ef database update --project src/HeThongChungCu.Infrastructure --startup-project src/HeThongChungCu.WebAPI --context AppDbContext
```

Khi app khởi động, `ApplicationDbContextInitialiser` sẽ kiểm tra khả năng kết nối DB và log cảnh báo nếu không kết nối được. Việc kiểm tra các migration còn thiếu sẽ do Health Check `/health/ready` đảm nhận.

## Chạy dự án & Kiểm thử

### Run (watch)

```bash
dotnet watch run --project src/HeThongChungCu.WebAPI/HeThongChungCu.WebAPI.csproj
```

Mặc định profile `Dev` chạy ở `http://localhost:5000` (xem `src/HeThongChungCu.WebAPI/Properties/launchSettings.json`).

### Running Tests

```bash
dotnet test
```

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


