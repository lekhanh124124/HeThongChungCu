import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface NotificationItem {
  id: number;
  thongBaoId?: number;
  tieuDe?: string;
  noiDung?: string;
  loaiThongBaoId?: number;
  tenLoaiThongBao?: string;
  referenceId?: string;
  metadata?: any;
  isRead?: boolean;
  createdAt?: string;
  readAt?: string | null;
}

export interface PagedResult<T> { items: T[]; pagingInfo?: { pageSize:number; pageNumber:number; totalItems:number } }
export interface ApiResponse<T> { result?: T; isOk: boolean; errors?: any[]; warningMessages?: string[] }

@Injectable({ providedIn: 'root' })
export class ThongBaoService {
  constructor(private http: HttpClient, @Inject('API_BASE') private apiBase: string) {}

  markRead(phanBoThongBaoId: number): Observable<ApiResponse<boolean>> {
    const url = `${this.apiBase}/api/thong-bao/da-doc`;
     return this.http.put<ApiResponse<boolean>>(url, { phanBoThongBaoId });
  }
}
