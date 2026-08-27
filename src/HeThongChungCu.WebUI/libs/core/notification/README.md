# Thông báo (Notification) — frontend integration

This library provides a small Angular service `ThongBaoService` to call notification APIs from the backend.

APIs used
- `POST /api/thong-bao/da-doc` — body: `{ phanBoThongBaoId }` — marks a notification as read

Usage

1. Ensure your app provides `API_BASE` token (the codebase already uses this pattern). Example provider in `AppModule`:

```ts
providers: [
  { provide: 'API_BASE', useValue: environment.apiBaseUrl }
]
```

2. Inject and use `ThongBaoService` for mark-as-read operations (the frontend no longer fetches notification list via `/api/thong-bao/get-list`):

```ts
constructor(private thongBao: ThongBaoService) {}

// mark read when user opens a notification
this.thongBao.markRead(phanBoId).subscribe();
```

Integration with SignalR
- When receiving a realtime notification payload, attach the backend's notification distribution id (`phanBoThongBaoId`) to the item so you can later call `markRead(phanBoThongBaoId)` when the user opens or views it.

Notes
- `metadata` field from backend may be a JSON string — parse when needed.
- The service expects HTTP interceptor or global headers to attach Authorization token (Bearer) as your project does.
