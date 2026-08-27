import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '@features/dich-vu/data-access';

@Injectable({ providedIn: 'root' })
export class QuyThuChiService {
  private base: string;

  constructor(private http: HttpClient, @Inject('API_BASE') apiBase: string) {
    this.base = apiBase ? `${apiBase.replace(/\/$/, '')}/api/quy-thu-chi` : '/api/quy-thu-chi';
  }

  getList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/get-list`, query);
  }

  getById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/get-by-id`, { id });
  }

  taoPhieuThu(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/tao-phieu-thu`, payload);
  }

  taoPhieuChi(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/tao-phieu-chi`, payload);
  }

  exportExcel(query: any): Observable<Blob> {
    return this.http.post(`${this.base}/export`, query, { responseType: 'blob' });
  }

  baoCaoThuChi(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/bao-cao/thu-chi`, query);
  }

  baoCaoCongNoCanHo(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/bao-cao/cong-no-can-ho`, query);
  }

  baoCaoCongNoToaNha(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/bao-cao/cong-no-toa-nha`, query);
  }

  exportCongNoCanHo(query: any): Observable<Blob> {
    return this.http.post(`${this.base}/bao-cao/cong-no-can-ho/export`, query, { responseType: 'blob' });
  }

  exportCongNoToaNha(query: any): Observable<Blob> {
    return this.http.post(`${this.base}/bao-cao/cong-no-toa-nha/export`, query, { responseType: 'blob' });
  }

  getLoaiThuChiForSelector(): Observable<ApiResponse<any[]>> {
    const url = this.base.replace('/quy-thu-chi', '/catalog') + '/loai-thu-chi-for-selector';
    return this.http.post<ApiResponse<any[]>>(url, {});
  }
}
