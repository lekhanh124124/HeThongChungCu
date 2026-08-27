import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ApiResponse<T> { result?: T; isOk: boolean; errors?: any[]; warningMessages?: string[] }

@Injectable({ providedIn: 'root' })
export class DichVuService {
  private base: string;
  constructor(private http: HttpClient, @Inject('API_BASE') apiBase: string) {
    this.base = apiBase ? `${apiBase.replace(/\/$/, '')}/api` : '/api';
  }

  createDichVu(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dich-vu`, payload);
  }

  updateDichVu(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/dich-vu`, payload);
  }

  deleteDichVu(ids: number[] | Array<number | string>): Observable<ApiResponse<boolean>> {
    return this.http.request<ApiResponse<boolean>>('delete', `${this.base}/dich-vu`, { body: { ids } });
  }

  getDichVuList(query?: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dich-vu/get-list`, query || {});
  }

  getDichVuById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dich-vu/get-by-id`, { id });
  }

  // Get service types for selector from catalog endpoint
  getLoaiDichVuForSelector(query?: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/catalog/loai-dich-vu-for-selector`, query || {});
  }

  // Get service status options for selector (catalog)
  getTrangThaiDichVuForSelector(query?: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/catalog/trang-thai-dich-vu-for-selector`, query || {});
  }

  activate(ids: number[]): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.base}/dich-vu/activate`, { ids });
  }

  revoke(ids: number[]): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.base}/dich-vu/revoke`, { ids });
  }

  // Khung giờ (time slots)
  createKhungGio(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dich-vu/khung-gio`, payload);
  }

  updateKhungGio(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/dich-vu/khung-gio`, payload);
  }

  deleteKhungGio(payload: { dichVuId: number; ids: number[] } | any): Observable<ApiResponse<boolean>> {
    return this.http.request<ApiResponse<boolean>>('delete', `${this.base}/dich-vu/khung-gio`, { body: payload });
  }

  revokeKhungGio(payload: { dichVuId: number; ids: number[] } | any): Observable<ApiResponse<boolean>> {
    return this.http.request<ApiResponse<boolean>>('delete', `${this.base}/dich-vu/khung-gio/revoke`, { body: payload });
  }

  getKhungGioList(query?: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dich-vu/khung-gio/get-list`, query || {});
  }

  getKhungGioById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dich-vu/khung-gio/get-by-id`, { id });
  }

  activateKhungGio(payload: { dichVuId: number; ids: number[] } | any): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.base}/dich-vu/khung-gio/activate`, payload);
  }

  // Bảng giá (pricing)
  createBangGia(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dich-vu/bang-gia`, payload);
  }

  updateBangGia(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/dich-vu/bang-gia`, payload);
  }

  deleteBangGia(payload: { dichVuId: number; ids: number[] } | any): Observable<ApiResponse<boolean>> {
    return this.http.request<ApiResponse<boolean>>('delete', `${this.base}/dich-vu/bang-gia`, { body: payload });
  }

  revokeBangGia(payload: { dichVuId: number; ids: number[] } | any): Observable<ApiResponse<boolean>> {
    return this.http.request<ApiResponse<boolean>>('delete', `${this.base}/dich-vu/bang-gia/revoke`, { body: payload });
  }

  getBangGiaList(query?: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dich-vu/bang-gia/get-list`, query || {});
  }

  getBangGiaById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dich-vu/bang-gia/get-by-id`, { id });
  }

  getLoaiDinhGiaForSelector(query?: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/catalog/loai-dinh-gia-for-selector`, query || {});
  }

  getLoaiCanHoForSelector(query?: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/catalog/loai-can-ho-for-selector`, query || {});
  }

  getNgayTrongTuanForSelector(query?: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/catalog/ngay-trong-tuan-for-selector`, query || {});
  }

  activateBangGia(payload: { dichVuId: number; ids: number[] } | any): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.base}/dich-vu/bang-gia/activate`, payload);
  }

  // Đăng ký sử dụng dịch vụ
  registerDichVu(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/dich-vu/dang-ky`, payload);
  }
}
