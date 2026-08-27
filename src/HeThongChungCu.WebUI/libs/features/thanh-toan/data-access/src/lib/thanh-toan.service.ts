import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '@features/dich-vu/data-access'; // Reusing the same interface

@Injectable({ providedIn: 'root' })
export class ThanhToanService {
  private base: string;

  constructor(private http: HttpClient, @Inject('API_BASE') apiBase: string) {
    this.base = apiBase ? `${apiBase.replace(/\/$/, '')}/api` : '/api';
  }

  // --- DotThanhToan ---
  getDotThanhToanList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dot-thanh-toan/get-list`, query);
  }

  getDotThanhToanById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dot-thanh-toan/get-by-id`, { id });
  }

  createDotThanhToan(payload: { thang: number; nam: number }): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dot-thanh-toan`, payload);
  }

  updateDotThanhToan(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/dot-thanh-toan`, payload);
  }

  deleteDotThanhToan(ids: number[]): Observable<ApiResponse<any>> {
    return this.http.request<ApiResponse<any>>('delete', `${this.base}/dot-thanh-toan`, { body: { ids } });
  }

  approveDotThanhToan(ids: number[]): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/dot-thanh-toan/duyet`, { ids });
  }

  lapHoaDonDuThao(dotThanhToanId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dot-thanh-toan/lap-hoa-don-du-thao`, { dotThanhToanId });
  }

  dongDotThanhToan(dotThanhToanId: number): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/dot-thanh-toan/dong`, { dotThanhToanId });
  }

  // --- HoaDon ---
  getHoaDonList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hoa-don/get-list`, query);
  }

  getHoaDonById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hoa-don/get-by-id`, { id });
  }

  phatHanhHoaDon(payload: { dotThanhToanId: number; hoaDonIds: number[] }): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/hoa-don/phat-hanh`, payload);
  }

  huyHoaDon(payload: { hoaDonId: number; lyDo: string }): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/hoa-don/huy`, payload);
  }

  getChiTietCoDinh(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hoa-don/get-chi-tiet-co-dinh`, { id });
  }

  getChiTietLuyTien(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hoa-don/get-chi-tiet-luy-tien`, { id });
  }

  getChiTietDienTich(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hoa-don/get-chi-tiet-dien-tich`, { id });
  }

  getChiTietKhungGio(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hoa-don/get-chi-tiet-khung-gio`, { id });
  }

  // --- GiaoDichThanhToan ---
  ghiNhanThanhToan(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/giao-dich-thanh-toan/ghi-nhan`, payload);
  }

  getGiaoDichByHoaDon(hoaDonId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/giao-dich-thanh-toan/get-by-hoa-don`, { hoaDonId });
  }

  taoPhienThanhToan(payload: { hoaDonId: number; chiTietHoaDonIds: number[] }): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/giao-dich-thanh-toan/tao-phien`, payload);
  }

  mockXacNhanThanhToan(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/giao-dich-thanh-toan/mock-xac-nhan`, payload);
  }

  // --- HoaDonDoiTac ---
  getHoaDonDoiTacList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hoa-don-doi-tac/get-list`, query);
  }

  getHoaDonDoiTacById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hoa-don-doi-tac/get-by-id`, { id });
  }

  createHoaDonDoiTac(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/hoa-don-doi-tac`, payload);
  }

  updateHoaDonDoiTac(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/hoa-don-doi-tac`, payload);
  }

  deleteHoaDonDoiTac(id: number): Observable<ApiResponse<any>> {
    return this.http.request<ApiResponse<any>>('delete', `${this.base}/hoa-don-doi-tac`, { body: { id } });
  }

  xacNhanThanhToanDoiTac(id: number): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/hoa-don-doi-tac/xac-nhan-thanh-toan`, { id });
  }

  // --- Catalog Selectors ---
  getLoaiChiTietHoaDonForSelector(): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/catalog/loai-chi-tiet-hoa-don-for-selector`, {});
  }

  getTrangThaiHoaDonForSelector(): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/catalog/trang-thai-hoa-don-for-selector`, {});
  }

  getTrangThaiDotThanhToanForSelector(): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/catalog/trang-thai-dot-thanh-toan-for-selector`, {});
  }

  getTrangThaiThanhToanDoiTacForSelector(): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/catalog/trang-thai-thanh-toan-doi-tac-for-selector`, {});
  }
}
