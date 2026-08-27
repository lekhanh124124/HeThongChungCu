import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '@features/dich-vu/data-access';

@Injectable({ providedIn: 'root' })
export class BaoTriService {
  private base: string;

  constructor(private http: HttpClient, @Inject('API_BASE') apiBase: string) {
    this.base = apiBase ? `${apiBase.replace(/\/$/, '')}/api/bao-tri-ha-tang` : '/api/bao-tri-ha-tang';
  }

  // --- Thiết bị hạ tầng ---
  getThietBiList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/thiet-bi/get-list`, query);
  }

  getThietBiById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/thiet-bi/get-by-id`, { id });
  }

  createThietBi(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/thiet-bi`, payload);
  }

  updateThietBi(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/thiet-bi`, payload);
  }

  deleteThietBi(id: number): Observable<ApiResponse<any>> {
    return this.http.request<ApiResponse<any>>('delete', `${this.base}/thiet-bi`, { body: { id } });
  }

  // --- Lịch bảo trì ---
  getLichBaoTriList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/lich-bao-tri/get-list`, query);
  }

  getLichBaoTriById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/lich-bao-tri/get-by-id`, { id });
  }

  createLichBaoTri(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/lich-bao-tri`, payload);
  }

  updateLichBaoTri(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/lich-bao-tri`, payload);
  }

  quetLichBaoTri(): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/lich-bao-tri/quet-lich`, {});
  }

  // --- Phiếu bảo trì ---
  getPhieuBaoTriList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phieu-bao-tri/get-list`, query);
  }

  getPhieuBaoTriById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phieu-bao-tri/get-by-id`, { id });
  }

  createPhieuBaoTri(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phieu-bao-tri`, payload);
  }

  phanCongPhieuBaoTri(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phieu-bao-tri/phan-cong`, payload);
  }

  capNhatTienDoPhieuBaoTri(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/phieu-bao-tri/cap-nhat-tien-do`, payload);
  }

  kiemDuyetPhieuBaoTri(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/phieu-bao-tri/kiem-duyet`, payload);
  }

  huyPhieuBaoTri(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/phieu-bao-tri/huy`, payload);
  }

  exportPhieuBaoTri(id: number): Observable<Blob> {
    return this.http.post(`${this.base}/phieu-bao-tri/export`, { id }, { responseType: 'blob' });
  }

  // --- Hạng mục bảo trì ---
  getHangMucList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hang-muc/get-list`, query);
  }

  getHangMucById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hang-muc/get-by-id`, { id });
  }

  createHangMuc(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hang-muc`, payload);
  }

  updateHangMuc(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/hang-muc`, payload);
  }

  deleteHangMuc(id: number): Observable<ApiResponse<any>> {
    return this.http.request<ApiResponse<any>>('delete', `${this.base}/hang-muc`, { body: { id } });
  }

  // --- Catalog Selectors ---
  getTanSuatBaoTriForSelector(): Observable<ApiResponse<any>> {
    const url = this.base.replace('/bao-tri-ha-tang', '/catalog') + '/tan-suat-bao-tri-for-selector';
    return this.http.post<ApiResponse<any>>(url, {});
  }

  getTrangThaiPhieuBaoTriForSelector(): Observable<ApiResponse<any>> {
    const url = this.base.replace('/bao-tri-ha-tang', '/catalog') + '/trang-thai-phieu-bao-tri-for-selector';
    return this.http.post<ApiResponse<any>>(url, {});
  }

  getTrangThaiThietBiForSelector(): Observable<ApiResponse<any>> {
    const url = this.base.replace('/bao-tri-ha-tang', '/catalog') + '/trang-thai-thiet-bi-for-selector';
    return this.http.post<ApiResponse<any>>(url, {});
  }
}
