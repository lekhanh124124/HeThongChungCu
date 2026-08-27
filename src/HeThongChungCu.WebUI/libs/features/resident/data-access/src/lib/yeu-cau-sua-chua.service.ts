import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult, TepDinhKem } from './chung-cu.model';
import { environment } from '@env/environment';

export interface YeuCauSuaChuaListItem {
  id: number;
  canHoId: number;
  tenCanHo: string;
  tenTang: string;
  tenToaNha: string;
  loaiYeuCauCuDanId: number;
  loaiYeuCauCuDanTen: string;
  trangThaiYeuCauId: number;
  trangThaiYeuCauTen: string;
  noiDung: string;
  loaiSuCoId: number;
  loaiSuCoTen: string;
  trangThaiSuaChuaId: number;
  trangThaiSuaChuaTen: string;
  createdAt: string;
  createdBy: number;
  tenNguoiGui: string;
  _highlight?: boolean;
}

export interface NhanSuSuaChua {
  id?: number;
  nhanVienId?: number;
  hoTen?: string;
  soCCCD?: string;
  soDienThoai?: string;
  vaiTro: string;
  ghiChu?: string;
}


export interface YeuCauSuaChuaDetail extends YeuCauSuaChuaListItem {
  lyDo?: string;
  nguoiXuLyId?: number;
  tenNguoiXuLy?: string;
  ngayXuLy?: string;
  phamViId: number;
  phamViTen: string;
  moTaViTri: string;
  henTu?: string;
  henDen?: string;
  chiPhiDuKien: number;
  chiPhiThucTe: number;
  isMienPhi: boolean;
  ghiChuBaoGia?: string;
  ketQuaXuLy?: string;
  lyDoHuy?: string;
  hopDongDoiTacId?: number;
  tenDoiTac?: string;
  nhanSuSuaChuas: NhanSuSuaChua[];
  danhSachTep: TepDinhKem[];
}

@Injectable({ providedIn: 'root' })
export class YeuCauSuaChuaService {
  private base = (environment && environment.apiBaseUrl)
    ? `${environment.apiBaseUrl.replace(/\/$/, '')}/api/yeu-cau-sua-chua`
    : '/api/yeu-cau-sua-chua';

  constructor(private http: HttpClient) {}

  getList(query: any): Observable<ApiResponse<PagedResult<YeuCauSuaChuaListItem>>> {
    return this.http.post<ApiResponse<PagedResult<YeuCauSuaChuaListItem>>>(`${this.base}/get-list`, query || {});
  }

  getById(id: number): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.post<ApiResponse<YeuCauSuaChuaDetail>>(`${this.base}/get-by-id`, { id });
  }

  pheDuyet(id: number): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.put<ApiResponse<YeuCauSuaChuaDetail>>(`${this.base}/phe-duyet`, { id });
  }

  tuChoi(id: number, lyDo: string): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.put<ApiResponse<YeuCauSuaChuaDetail>>(`${this.base}/tu-choi`, { id, lyDo });
  }

  traLai(id: number, lyDo: string): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.put<ApiResponse<YeuCauSuaChuaDetail>>(`${this.base}/tra-lai`, { id, lyDo });
  }

  dieuPhoiNhanSu(payload: { id: number; hopDongDoiTacId?: number | null; nhanSu: NhanSuSuaChua[] }): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.put<ApiResponse<YeuCauSuaChuaDetail>>(`${this.base}/dieu-phoi-nhan-su`, payload);
  }

  boSungNhanSu(payload: { id: number; nhanSu: NhanSuSuaChua[] }): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.put<ApiResponse<YeuCauSuaChuaDetail>>(`${this.base}/bo-sung-nhan-su`, payload);
  }

  xoaNhanSu(payload: { id: number; nhanSuId: number; lyDo: string }): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.request<ApiResponse<YeuCauSuaChuaDetail>>('delete', `${this.base}/xoa-nhan-su`, { body: payload });
  }

  nhapBaoGia(payload: { id: number; chiPhiDuKien: number; isMienPhi: boolean; ghiChuBaoGia: string }): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.put<ApiResponse<YeuCauSuaChuaDetail>>(`${this.base}/nhap-bao-gia`, payload);
  }

  henLich(payload: { id: number; henTu: string; henDen: string }): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.put<ApiResponse<YeuCauSuaChuaDetail>>(`${this.base}/hen-lich`, payload);
  }

  hoanTatXuLy(payload: { id: number; ketQuaXuLy: string; chiPhiThucTe: number }): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.put<ApiResponse<YeuCauSuaChuaDetail>>(`${this.base}/hoan-tat-xu-ly`, payload);
  }

  huy(id: number, lyDoHuy: string): Observable<ApiResponse<YeuCauSuaChuaDetail>> {
    return this.http.request<ApiResponse<YeuCauSuaChuaDetail>>('delete', `${this.base}/huy`, { body: { id, lyDoHuy } });
  }

  // Catalog selectors
  getLoaiSuCoForSelector(): Observable<ApiResponse<any[]>> {
    const apiRoot = (environment && environment.apiBaseUrl)
      ? `${environment.apiBaseUrl.replace(/\/$/, '')}/api`
      : '/api';
    return this.http.post<ApiResponse<any[]>>(`${apiRoot}/catalog/loai-su-co-ky-thuat-for-selector`, {});
  }

  getTrangThaiSuaChuaForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base.replace('yeu-cau-sua-chua', 'catalog')}/trang-thai-sua-chua-for-selector`, {});
  }

  getPhamViForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base.replace('yeu-cau-sua-chua', 'catalog')}/pham-vi-sua-chua-for-selector`, {});
  }
}
