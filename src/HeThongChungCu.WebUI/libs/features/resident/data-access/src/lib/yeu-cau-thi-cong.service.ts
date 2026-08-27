import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedResult, TepDinhKem } from './chung-cu.model';
import { environment } from '@env/environment';

export interface YeuCauThiCongListItem {
  id: number;
  canHoId: number;
  tenCanHo: string;
  hangMucThiCong: string;
  duKienBatDau: string;
  duKienKetThuc: string;
  tenDonViThiCong: string;
  trangThaiYeuCauId: number;
  trangThaiYeuCauTen: string;
  trangThaiThiCongId: number;
  trangThaiThiCongTen: string;
  createdAt: string;
  createdBy: number;
  tenNguoiGui: string;
  _highlight?: boolean;
}

export interface NhanSuThiCong {
  id?: number;
  nhanVienId?: number;
  hoTen: string;
  soCCCD: string;
  soDienThoai: string;
  vaiTro: string;
  ghiChu?: string;
  lyDoXoa?: string;
}


export interface YeuCauThiCongDetail extends YeuCauThiCongListItem {
  noiDung: string;
  nguoiDaiDien: string;
  soDienThoaiDaiDien: string;
  tienDatCoc: number;
  isDaThuCoc: boolean;
  ghiChuThuCoc: string;
  tienKhauTru: number;
  lyDoKhauTru: string;
  isDaHoanCoc: boolean;
  nguoiXuLyId: number;
  tenNguoiXuLy: string;
  ngayXuLy: string;
  lyDo: string;
  lyDoHuy?: string;
  nhanSuThiCongs: NhanSuThiCong[];
  danhSachTep: TepDinhKem[];
}

@Injectable({ providedIn: 'root' })
export class YeuCauThiCongService {
  private base = (environment && environment.apiBaseUrl)
    ? `${environment.apiBaseUrl.replace(/\/$/, '')}/api/yeu-cau-thi-cong`
    : '/api/yeu-cau-thi-cong';

  constructor(private http: HttpClient) {}

  getList(query: any): Observable<ApiResponse<PagedResult<YeuCauThiCongListItem>>> {
    return this.http.post<ApiResponse<PagedResult<YeuCauThiCongListItem>>>(`${this.base}/get-list`, query || {});
  }

  getById(id: number): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.post<ApiResponse<YeuCauThiCongDetail>>(`${this.base}/get-by-id`, { id });
  }

  create(payload: any): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.post<ApiResponse<YeuCauThiCongDetail>>(`${this.base}`, payload);
  }

  update(payload: any): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.put<ApiResponse<YeuCauThiCongDetail>>(`${this.base}`, payload);
  }

  traLai(id: number, lyDo: string): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.put<ApiResponse<YeuCauThiCongDetail>>(`${this.base}/tra-lai`, { id, lyDo });
  }

  setTienCoc(id: number, soTien: number): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.put<ApiResponse<YeuCauThiCongDetail>>(`${this.base}/set-tien-coc`, { id, soTien });
  }

  approve(id: number): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.put<ApiResponse<YeuCauThiCongDetail>>(`${this.base}/approve`, { id });
  }

  thuCoc(id: number, ghiChu: string): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.put<ApiResponse<YeuCauThiCongDetail>>(`${this.base}/thu-coc`, { id, ghiChu });
  }

  nghiemThu(id: number): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.put<ApiResponse<YeuCauThiCongDetail>>(`${this.base}/nghiem-thu`, { id });
  }

  hoanCoc(id: number, tienKhauTru: number, lyDo: string): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.put<ApiResponse<YeuCauThiCongDetail>>(`${this.base}/hoan-coc`, { id, tienKhauTru, lyDo });
  }

  complete(id: number): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.put<ApiResponse<YeuCauThiCongDetail>>(`${this.base}/complete`, { id });
  }

  cancel(id: number, lyDo: string): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.request<ApiResponse<YeuCauThiCongDetail>>('delete', `${this.base}/cancel`, { body: { id, lyDo } });
  }

  addNhanSu(payload: { id: number; hoTen: string; soCCCD: string; soDienThoai: string; vaiTro: string; ghiChu?: string }): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.post<ApiResponse<YeuCauThiCongDetail>>(`${this.base}/nhan-su`, payload);
  }

  deleteNhanSu(payload: { id: number; nhanSuId: number; lyDo: string }): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.request<ApiResponse<YeuCauThiCongDetail>>('delete', `${this.base}/nhan-su`, { body: payload });
  }

  addTep(payload: { id: number; tepIds: number[] }): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.post<ApiResponse<YeuCauThiCongDetail>>(`${this.base}/tep`, payload);
  }

  deleteTep(payload: { id: number; tepId: number }): Observable<ApiResponse<YeuCauThiCongDetail>> {
    return this.http.request<ApiResponse<YeuCauThiCongDetail>>('delete', `${this.base}/tep`, { body: payload });
  }
}
