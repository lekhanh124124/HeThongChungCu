import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '@core/auth/data-access';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(): boolean | Observable<boolean> {
    if (this.auth.isAuthenticated()) return true;
    const refresh = this.auth.getRefreshToken();
    if (refresh) {
      return this.auth.refreshToken().pipe(
        map(() => {
          if (this.auth.isAuthenticated()) return true;
          this.router.navigate(['/auth/login']);
          return false;
        }),
        catchError(() => {
          this.router.navigate(['/auth/login']);
          return of(false);
        })
      );
    }
    this.router.navigate(['/auth/login']);
    return false;
  }
}
