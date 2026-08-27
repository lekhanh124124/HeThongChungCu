import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, BehaviorSubject } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '@env/environment';
import { AuthApiService } from '@features/auth/data-access';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private accessKey = 'access_token';
  private refreshKey = 'refresh_token';
  private avatarKey = 'avatar_url';
  private avatarSubject = new BehaviorSubject<string | null>(localStorage.getItem('avatar_url'));
  avatar$ = this.avatarSubject.asObservable();
  private userKey = 'auth_user';
  private userSubject = new BehaviorSubject<any>(
    (() => {
      try { return JSON.parse(localStorage.getItem('auth_user') || 'null'); } catch { return null; }
    })()
  );
  currentUser$ = this.userSubject.asObservable();

  constructor(private http: HttpClient, private api: AuthApiService) {}

  private refreshInProgress: Promise<string | null> | null = null;

  /**
   * Ensure access token is valid. Returns access token or null if cannot refresh.
   * Uses single-flight Promise to avoid concurrent refresh requests.
   */
  ensureValidAccessToken(): Promise<string | null> {
    const access = this.getAccessToken();
    if (access && !this.isTokenExpired(access)) return Promise.resolve(access);

    if (this.refreshInProgress) return this.refreshInProgress;

    this.refreshInProgress = (async () => {
      try {
        const res: any = await this.refreshToken().toPromise();
        const newAccess = res?.result?.accessToken || null;
        const newRefresh = res?.result?.refreshToken || null;
        if (newAccess) this.setTokens(newAccess, newRefresh);
        return newAccess;
      } catch (e) {
        this.clearTokens();
        return null;
      } finally {
        this.refreshInProgress = null;
      }
    })();

    return this.refreshInProgress;
  }

  private isTokenExpired(token: string | null): boolean {
    if (!token) return true;
    try {
      const parts = token.split('.');
      if (parts.length < 2) return true;
      const payload = JSON.parse(atob(parts[1]));
      const exp = (payload.exp || 0) * 1000;
      return Date.now() > (exp - 5000);
    } catch {
      return true;
    }
  }

  login(username: string, password: string): Observable<any> {
    this.clearTokens();
    const payload = { username, password };
    return this.api.login(payload).pipe(
      tap((res: any) => {
        const access = res?.result?.accessToken;
        const refresh = res?.result?.refreshToken;
        if (access) this.setTokens(access, refresh);
        const user = res?.result || null;
        if (user) {
          try { localStorage.setItem(this.userKey, JSON.stringify(user)); } catch {}
          this.userSubject.next(user);
          if (user.anhDaiDienUrl) this.setAvatar(user.anhDaiDienUrl);
        }
      })
    );
  }

  logout(): Observable<any> {
    const refresh = this.getRefreshToken();
    return this.api.logout({ refreshToken: refresh }).pipe(
      tap(() => this.clearTokens())
    );
  }

  refreshToken(): Observable<any> {
    const token = this.getRefreshToken();
    if (!token) return of(null);
    return this.api.refreshToken({ refreshToken: token }).pipe(
      tap((res: any) => {
        const access = res?.result?.accessToken;
        const refresh = res?.result?.refreshToken;
        if (access) this.setTokens(access, refresh);
      }),
      catchError(() => of(null))
    );
  }

  forgotPassword(username: string): Observable<any> {
    return this.api.forgotPassword({ username });
  }

  resetPassword(username: string, resetCode: string, newPassword: string, confirmPassword?: string): Observable<any> {
    const payload = {
      username,
      resetCode,
      newPassword,
      confirmPassword
    };
    return this.api.resetPassword(payload as any);
  }

  setTokens(access: string, refresh?: string | null) {
    if (access) localStorage.setItem(this.accessKey, access);
    if (refresh) localStorage.setItem(this.refreshKey, refresh);
  }

  setAvatar(url: string | null) {
    if (url) localStorage.setItem(this.avatarKey, url);
    else localStorage.removeItem(this.avatarKey);
    this.avatarSubject.next(url);
  }

  getCurrentUser(): any {
    try { return JSON.parse(localStorage.getItem(this.userKey) || 'null'); } catch { return null; }
  }

  getAvatar(): string | null {
    return localStorage.getItem(this.avatarKey);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.accessKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshKey);
  }

  clearTokens() {
    localStorage.removeItem(this.accessKey);
    localStorage.removeItem(this.refreshKey);
    localStorage.removeItem(this.userKey);
    this.userSubject.next(null);
  }

  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }
}
