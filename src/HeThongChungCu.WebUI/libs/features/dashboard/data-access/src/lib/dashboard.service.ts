import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface TheCuDan {
  cuDanMoiTrongThang: number;
  chiSoBienDong: number;
  tongTamTru: number;
  tongTamVang: number;
}

export interface ThePhanAnh {
  soLuongChuaXuLy: number;
  soLuongDangXuLy: number;
  soLuongHoanThanh: number;
  coPhanAnhKhan: boolean;
}

export interface TheTaiChinh {
  tongNoPhi: number;
  tyLeThanhToan: number;
}

export interface ThePhuongTien {
  tongXeDangKy: number;
  soLuongXeMay: number;
  soLuongOTo: number;
  soLuongXeKhac: number;
}

export interface DoanhThuTheoLoai {
  tenLoaiPhi: string;
  soTien: number;
  tyLePhanTram: number;
}

export interface DangKyTienIch {
  tenTienIch: string;
  luotDungHomNay: number;
  luotDungTrongThang: number;
}

export interface TongQuanBaoTri {
  lichBaoTriSapToi: number;
  congViecKT_DangXuLy: number;
  suCoQuaHan: number;
}

export interface HoatDongGanDay {
  tenNguoiThucHien: string;
  anhDaiDienUrl: string;
  loaiHoatDong: string;
  moTa: string;
  thoiGianTao: string;
  thoiGianTuongDoi: string;
}

export interface DashboardOverview {
  theCuDan: TheCuDan;
  thePhanAnh: ThePhanAnh;
  theTaiChinh: TheTaiChinh;
  thePhuongTien: ThePhuongTien;
  doanhThuTheoLoai: DoanhThuTheoLoai[];
  dangKyTienIch: DangKyTienIch[];
  tongQuanBaoTri: TongQuanBaoTri;
  hoatDongGanDay: HoatDongGanDay[];
}

export interface ApiResponse<T> {
  result: T;
  warningMessages?: string[];
  errors?: { code: string; description: string }[];
  isOk: boolean;
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private base: string;

  constructor(private http: HttpClient, @Inject('API_BASE') apiBase: string) {
    this.base = apiBase ? `${apiBase.replace(/\/$/, '')}/api` : '/api';
  }

  getOverview(query?: { toaNhaId?: number; thang?: number; nam?: number; ngay?: number }): Observable<ApiResponse<DashboardOverview>> {
    return this.http.post<ApiResponse<DashboardOverview>>(`${this.base}/dashboard/overview`, query || {});
  }
}
