import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Injectable()
export class ResponseNotifierInterceptor implements HttpInterceptor {
  constructor(private notification: NzNotificationService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const url = (req.url || '').toLowerCase();

    // Bỏ qua thông báo tự động cho các API xác thực,
    // vì các màn auth (login/forgot/reset...) đã tự xử lý hiển thị lỗi riêng.
    if (url.includes('/api/auth/')) {
      return next.handle(req);
    }

    return next.handle(req).pipe(
      tap({
        next: (ev) => {
          if (ev instanceof HttpResponse) {
            const body: any = ev.body;
            if (!body) return;

            if (body.warningMessages && Array.isArray(body.warningMessages) && body.warningMessages.length) {
              body.warningMessages.forEach((m: string) => {
                if (m) this.notification.warning('Cảnh báo', m);
              });
            }

            if (body.errors && Array.isArray(body.errors) && body.errors.length) {
              body.errors.forEach((e: any) => {
                const desc = (e && (e.description || e.message || e.code)) || JSON.stringify(e);
                if (desc) this.notification.error('Lỗi', desc);
              });
            }

            if (body.isOk === false && body.result && typeof body.result === 'string') {
              this.notification.error('Lỗi', body.result);
            }
          }
        },
        error: (err: any) => {
          if (err instanceof HttpErrorResponse) {
            const payload = err.error || {};

            if (payload.warningMessages && Array.isArray(payload.warningMessages)) {
              payload.warningMessages.forEach((m: string) => m && this.notification.warning('Cảnh báo', m));
            }

            if (payload.errors && Array.isArray(payload.errors) && payload.errors.length) {
              payload.errors.forEach((e: any) => {
                const desc = (e && (e.description || e.message || e.code)) || JSON.stringify(e);
                if (desc) this.notification.error('Lỗi', desc);
              });
              return;
            }

            if (payload.result && typeof payload.result === 'string') {
              this.notification.error('Lỗi', payload.result);
              return;
            }

            this.notification.error('Lỗi', err.message || 'Lỗi kết nối');
          }
        }
      })
    );
  }
}
