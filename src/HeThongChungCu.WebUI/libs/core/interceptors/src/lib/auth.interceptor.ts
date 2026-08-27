import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject, of } from 'rxjs';
import { catchError, filter, finalize, switchMap, take } from 'rxjs/operators';
import { AuthService } from '@core/auth/data-access';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(private auth: AuthService, private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const url = (req.url || '').toLowerCase();
    if (url.includes('/api/auth/login') || url.includes('/api/auth/forgot-password') || url.includes('/api/auth/reset-password') || url.includes('/api/auth/refresh-token')) {
      return next.handle(req);
    }

    const token = this.auth.getAccessToken();
    const authReq = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;

    return next.handle(authReq).pipe(
      catchError(err => {
        if (err instanceof HttpErrorResponse && err.status === 401) {
          return this.handle401Error(authReq, next);
        }
        return throwError(() => err);
      })
    );
  }

  private handle401Error(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const url = (req.url || '').toLowerCase();
    if (url.includes('/api/auth/login') || url.includes('/api/auth/refresh-token') || url.includes('/api/auth/logout')) {
      this.auth.clearTokens();
      this.router.navigate(['/auth/login']);
      return throwError(() => new Error('Unauthorized'));
    }

    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      const refresh$ = this.auth.refreshToken();
      return refresh$.pipe(
        switchMap((res: any) => {
          const newAccess = this.auth.getAccessToken();
          if (newAccess) {
            this.refreshTokenSubject.next(newAccess);
            const cloned = req.clone({ setHeaders: { Authorization: `Bearer ${newAccess}` } });
            return next.handle(cloned);
          }
          this.auth.clearTokens();
          this.router.navigate(['/auth/login']);
          return throwError(() => new Error('Unable to refresh token'));
        }),
        catchError(err => {
          this.auth.clearTokens();
          this.router.navigate(['/auth/login']);
          return throwError(() => err);
        }),
        finalize(() => {
          this.isRefreshing = false;
        })
      );
    } else {
      return this.refreshTokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap((token) => {
          const cloned = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
          return next.handle(cloned as HttpRequest<any>);
        })
      );
    }
  }
}
