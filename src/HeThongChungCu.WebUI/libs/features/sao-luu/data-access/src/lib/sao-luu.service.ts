import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '@features/dich-vu/data-access';

@Injectable({ providedIn: 'root' })
export class SaoLuuService {
  private base: string;

  constructor(private http: HttpClient, @Inject('API_BASE') apiBase: string) {
    this.base = apiBase ? `${apiBase.replace(/\/$/, '')}/api/backup` : '/api/backup';
  }

  getList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/get-list`, query);
  }

  create(): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}`, {});
  }

  delete(fileId: number): Observable<ApiResponse<any>> {
    return this.http.request<ApiResponse<any>>('delete', `${this.base}`, { body: { fileId } });
  }

  restore(fileId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/restore`, { fileId });
  }
}
