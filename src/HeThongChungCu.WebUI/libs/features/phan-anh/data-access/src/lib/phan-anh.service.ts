import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '@features/dich-vu/data-access';

export interface PhanAnhResponse {
  id: number;
  canHoId: number;
  tenCanHo: string;
  tieuDe: string;
  loaiPhanAnhId: number;
  loaiPhanAnhTen: string;
  trangThaiPhanAnhId: number;
  trangThaiPhanAnhTen: string;
  nguoiXuLyId?: number;
  tenNguoiXuLy?: string;
  createdAt: string;
  createdBy: number;
  tenNguoiGui: string;
}

export interface PhanAnhDetailResponse extends PhanAnhResponse {
  noiDung: string;
  diemDanhGia?: number;
  nhanXetDanhGia?: string;
  ngayDanhGia?: string;
  traLoiPhanAnhs: Array<{
    id: number;
    noiDung: string;
    isNhanVien: boolean;
    createdBy: number;
    tenNguoiGui: string;
    createdAt: string;
  }>;
  danhSachTep: Array<{
    id: number;
    fileName: string;
    fileUrl: string;
    size?: number;
    contentType?: string;
  }>;
}

@Injectable({ providedIn: 'root' })
export class PhanAnhService {
  private base: string;

  constructor(private http: HttpClient, @Inject('API_BASE') apiBase: string) {
    this.base = apiBase ? `${apiBase.replace(/\/$/, '')}/api` : '/api';
  }

  getList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phan-anh/get-list`, query);
  }

  getById(id: number): Observable<ApiResponse<PhanAnhDetailResponse>> {
    return this.http.post<ApiResponse<PhanAnhDetailResponse>>(`${this.base}/phan-anh/get-by-id`, { id });
  }

  tiepNhan(phanAnhId: number): Observable<ApiResponse<PhanAnhResponse>> {
    return this.http.put<ApiResponse<PhanAnhResponse>>(`${this.base}/phan-anh/tiep-nhan`, { phanAnhId });
  }

  submitTraLoi(phanAnhId: number, noiDung: string): Observable<ApiResponse<PhanAnhResponse>> {
    return this.http.post<ApiResponse<PhanAnhResponse>>(`${this.base}/phan-anh/submit-tra-loi`, { phanAnhId, noiDung });
  }

  xacNhanHoanThanh(phanAnhId: number, ketQua: string): Observable<ApiResponse<PhanAnhResponse>> {
    return this.http.put<ApiResponse<PhanAnhResponse>>(`${this.base}/phan-anh/xac-nhan-hoan-thanh`, { phanAnhId, ketQua });
  }

  huy(phanAnhId: number, lyDoHuy: string): Observable<ApiResponse<PhanAnhResponse>> {
    return this.http.put<ApiResponse<PhanAnhResponse>>(`${this.base}/phan-anh/huy`, { phanAnhId, lyDoHuy });
  }
}
