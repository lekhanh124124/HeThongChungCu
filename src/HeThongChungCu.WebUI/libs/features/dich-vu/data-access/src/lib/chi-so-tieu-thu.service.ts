import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from './dich-vu.service';

@Injectable({ providedIn: 'root' })
export class ChiSoTieuThuService {
  private base: string;

  constructor(private http: HttpClient, @Inject('API_BASE') apiBase: string) {
    this.base = apiBase ? `${apiBase.replace(/\/$/, '')}/api` : '/api';
  }

  getList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/chi-so-tieu-thu/get-list`, query);
  }

  getListDichVuTieuThu(apiVersion?: string): Observable<ApiResponse<any>> {
    let params = new HttpParams();
    if (apiVersion) {
      params = params.set('api-version', apiVersion);
    }
    return this.http.post<ApiResponse<any>>(`${this.base}/chi-so-tieu-thu/get-list-dich-vu-tieu-thu`, {}, { params });
  }

  getById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/chi-so-tieu-thu/get-by-id`, { id });
  }

  exportExcel(payload: { toaNhaId?: number; tangId?: number; dichVuId?: number; thang: number; nam: number }): Observable<any> {
    return this.http.post(`${this.base}/chi-so-tieu-thu/export`, payload, { responseType: 'blob' });
  }

  importExcel(file: File, thang: number, nam: number, ngayChot: string): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('thang', thang.toString());
    formData.append('nam', nam.toString());
    formData.append('ngayChot', ngayChot);
    return this.http.post<ApiResponse<any>>(`${this.base}/chi-so-tieu-thu/import`, formData);
  }

  bulkCreate(payload: { items: any[]; thang: number; nam: number; ngayGhiNhan: string }): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/chi-so-tieu-thu`, payload);
  }

  update(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/chi-so-tieu-thu`, payload);
  }

  delete(ids: number[]): Observable<ApiResponse<any>> {
    return this.http.request<ApiResponse<any>>('delete', `${this.base}/chi-so-tieu-thu`, { body: { ids } });
  }

  confirm(payload: { thang: number; nam: number; dichVuId: number }): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/chi-so-tieu-thu/xac-nhan`, payload);
  }

  importImages(zipFile: File): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('zipFile', zipFile);
    return this.http.post<ApiResponse<any>>(`${this.base}/chi-so-tieu-thu/import-images`, formData);
  }

  // --- Catalog Selectors ---
  getTrangThaiChiSoForSelector(): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/catalog/trang-thai-chi-so-for-selector`, {});
  }
}
