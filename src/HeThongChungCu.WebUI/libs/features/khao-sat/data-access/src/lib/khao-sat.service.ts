import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '@features/dich-vu/data-access';

export interface KhaoSatResponse {
  id: number;
  tieuDe: string;
  moTa: string;
  loaiKhaoSatId: number;
  loaiKhaoSatTen: string;
  coCheTinhDiemId: number;
  coCheTinhDiemTen: string;
  trangThaiId: number;
  trangThaiTen: string;
  ngayBatDau: string;
  ngayKetThuc: string;
  tyleThamGiaToiThieu: number;
  tyLeDongYToiThieu: number;
  isAnDanh: boolean;
  isVoted: boolean;
  createdAt: string;
}

export interface KhaoSatDetailResponse extends KhaoSatResponse {
  cauHois: Array<{
    id: number;
    noiDungCauHoi: string;
    isBatBuoc: boolean;
    isMultiSelect: boolean;
    luaChons: Array<{
      id: number;
      noiDungLuaChon: string;
      isUngVienBQT: boolean;
      tieuSuUngVien?: string;
      ungVienId?: number;
    }>;
  }>;
}

export interface KetQuaKhaoSatResponse {
  khaoSatId: number;
  tieuDeKhaoSat: string;
  tongSoCanHo: number;
  soCanHoDaThamGia: number;
  tyLeThamGia: number;
  tyleThamGiaToiThieu: number;
  isHieuLuc: boolean;
  coCheTinhDiemId: number;
  coCheTinhDiemTen: string;
  ketQuaCauHois: Array<{
    cauHoiId: number;
    noiDungCauHoi: string;
    isMultiSelect: boolean;
    ketQuaLuaChons: Array<{
      luaChonId: number;
      noiDungLuaChon: string;
      isUngVienBQT: boolean;
      soLuongPhieuBau: number;
      tyLePhanTram: number;
    }>;
  }>;
}

@Injectable({ providedIn: 'root' })
export class KhaoSatService {
  private base: string;

  constructor(private http: HttpClient, @Inject('API_BASE') apiBase: string) {
    this.base = apiBase ? `${apiBase.replace(/\/$/, '')}/api` : '/api';
  }

  getList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/khao-sat/get-list`, query);
  }

  getById(id: number): Observable<ApiResponse<KhaoSatDetailResponse>> {
    return this.http.post<ApiResponse<KhaoSatDetailResponse>>(`${this.base}/khao-sat/get-by-id`, { id });
  }

  getKetQua(id: number): Observable<ApiResponse<KetQuaKhaoSatResponse>> {
    return this.http.post<ApiResponse<KetQuaKhaoSatResponse>>(`${this.base}/khao-sat/get-ket-qua`, { id });
  }

  create(payload: any): Observable<ApiResponse<KhaoSatResponse>> {
    return this.http.post<ApiResponse<KhaoSatResponse>>(`${this.base}/khao-sat`, payload);
  }

  update(payload: any): Observable<ApiResponse<KhaoSatResponse>> {
    return this.http.put<ApiResponse<KhaoSatResponse>>(`${this.base}/khao-sat`, payload);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.request<ApiResponse<boolean>>('delete', `${this.base}/khao-sat`, { body: { id } });
  }

  congBo(id: number): Observable<ApiResponse<KhaoSatResponse>> {
    return this.http.put<ApiResponse<KhaoSatResponse>>(`${this.base}/khao-sat/cong-bo`, { id });
  }

  getDanhSachThamGia(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/khao-sat/danh-sach-tham-gia`, query);
  }

  getLichSuBieuQuyet(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/khao-sat/lich-su-bieu-quyet`, query);
  }
}
