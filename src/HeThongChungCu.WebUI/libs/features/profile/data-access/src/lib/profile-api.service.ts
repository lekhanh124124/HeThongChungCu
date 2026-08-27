import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

@Injectable({ providedIn: 'root' })
export class ProfileApiService {
  constructor(private http: HttpClient) {}

  getProfile(): Observable<any> {
    return this.http.post<any>(`${environment.apiBaseUrl}/api/Profile/get-profile`, {});
  }

  updateProfile(payload: any): Observable<any> {
    return this.http.put<any>(`${environment.apiBaseUrl}/api/Profile`, payload);
  }

  changeAvatar(formData: FormData): Observable<any> {
    return this.http.post<any>(`${environment.apiBaseUrl}/api/Profile/change-avatar`, formData);
  }

  changePassword(payload: any): Observable<any> {
    return this.http.post<any>(`${environment.apiBaseUrl}/api/Profile/change-password`, payload);
  }

  getGioiTinhForSelector(): Observable<any> {
    return this.http.post<any>(`${environment.apiBaseUrl}/api/catalog/gioi-tinh-for-selector`, {});
  }
}
