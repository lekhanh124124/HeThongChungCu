import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { ForgotPasswordRequest, LoginRequest, ResetPasswordRequest, ChangePasswordRequest, RefreshTokenRequest, ApiResponse, AuthUserResult, RegisterResidentRequest } from './models';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  constructor(private http: HttpClient) {}

  login(payload: LoginRequest): Observable<unknown> {
    return this.http.post(`${environment.apiBaseUrl}/api/auth/login`, payload);
  }

  forgotPassword(payload: ForgotPasswordRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${environment.apiBaseUrl}/api/auth/forgot-password`, payload);
  }

  resetPassword(payload: ResetPasswordRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${environment.apiBaseUrl}/api/auth/reset-password`, payload);
  }

  changePassword(payload: ChangePasswordRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${environment.apiBaseUrl}/api/auth/change-password`, payload);
  }

  registerResident(payload: RegisterResidentRequest): Observable<ApiResponse<AuthUserResult>> {
    return this.http.post<ApiResponse<AuthUserResult>>(`${environment.apiBaseUrl}/api/auth/register`, payload);
  }

  refreshToken(payload: RefreshTokenRequest): Observable<ApiResponse<AuthUserResult>> {
    return this.http.post<ApiResponse<AuthUserResult>>(`${environment.apiBaseUrl}/api/auth/refresh-token`, payload);
  }

  logout(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${environment.apiBaseUrl}/api/auth/logout`, payload);
  }
}
