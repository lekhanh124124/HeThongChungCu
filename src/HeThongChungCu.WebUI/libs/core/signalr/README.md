# SignalR Service (Angular)

Đây là service mẫu dùng `@microsoft/signalr` để nhận notification realtime từ backend.

Quick start

1. Cài package:

```bash
npm install @microsoft/signalr
```

2. Sử dụng `SignalrService` (mẫu):

- Khởi tạo khi app start hoặc sau khi login để đảm bảo token đã sẵn sàng.

```ts
// AppComponent (ví dụ)
constructor(private signalr: SignalrService, private auth: AuthService) {}

ngOnInit(){
  this.signalr.init(environment.apiBaseUrl + '/notifications', () => localStorage.getItem('access_token') || '');

  // Lắng nghe notification chung
  this.signalr.notifications().subscribe(n => {
    if (!n) return;
    console.log('Received notification', n);
  });
}
```

3. Gợi ý: nếu access token có thể hết hạn, truyền một hàm trả `Promise<string|null>` để service có thể chờ refresh trước khi kết nối:

```ts
this.signalr.init(environment.apiBaseUrl + '/notifications', () => this.auth.ensureValidAccessToken());
```

4. Để test nhanh, bạn có thể dùng file `test-notifications.html` (đã có trong repo) hoặc tạo component test và subscribe `signalr.notifications()`.

Security notes
- Không in token ra console trong production.
- Backend của dự án đọc token từ `access_token` query string — `accessTokenFactory` đã xử lý điều này.

Environment & DI notes
- Provide the API base URL in your Angular environment files (example `environment.ts`):

```ts
export const environment = {
  production: false,
  apiBaseUrl: 'https://api.example.com'
};
```

- Ensure your `AppModule` or a core module provides `API_BASE` (used by other services in this repo):

```ts
providers: [
  { provide: 'API_BASE', useValue: environment.apiBaseUrl }
]
```

Retry / token refresh behavior
- `SignalrService.init()` accepts an access token getter that may return a `Promise<string|null>`; pass your `AuthService.ensureValidAccessToken()` so the service can wait for refresh before connecting. The service will attempt a few retries on start and call the getter again when it detects an authorization failure.
