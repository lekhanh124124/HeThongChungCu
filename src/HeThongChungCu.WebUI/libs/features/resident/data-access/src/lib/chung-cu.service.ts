import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ApiResponse, PagedResult, ToaNha, Tang, CanHo, CauTrucToaNha } from './chung-cu.model';
import { environment } from '@env/environment';

@Injectable({ providedIn: 'root' })
export class ChungCuService {
  private refreshSubject = new Subject<void>();
  refresh$ = this.refreshSubject.asObservable();
  private base = (environment && environment.apiBaseUrl)
    ? `${environment.apiBaseUrl.replace(/\/$/, '')}/api`
    : '/api';

  constructor(private http: HttpClient) {}

  // Toa nha
  createToaNha(payload: ToaNha): Observable<ApiResponse<ToaNha>> {
    return this.http.post<ApiResponse<ToaNha>>(`${this.base}/toa-nha`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  updateToaNha(payload: ToaNha): Observable<ApiResponse<ToaNha>> {
    return this.http.put<ApiResponse<ToaNha>>(`${this.base}/toa-nha`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  deleteToaNha(ids: number[]): Observable<ApiResponse<ToaNha[]>> {
    return this.http.request<ApiResponse<ToaNha[]>>('delete', `${this.base}/toa-nha`, { body: { ids } })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  getToaNhaList(query: any): Observable<ApiResponse<PagedResult<ToaNha>>> {
    return this.http.post<ApiResponse<PagedResult<ToaNha>>>(`${this.base}/toa-nha/get-list`, query || {});
  }

  getToaNhaById(id: number): Observable<ApiResponse<ToaNha>> {
    return this.http.post<ApiResponse<ToaNha>>(`${this.base}/toa-nha/get-by-id`, { id });
  }

  // Gợi ý mã tòa nhà
  goiYMaToaNha(): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/toa-nha/goi-y-ma-toa-nha`, {});
  }

  // Tang
  createTang(payload: Tang): Observable<ApiResponse<Tang>> {
    return this.http.post<ApiResponse<Tang>>(`${this.base}/tang`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  updateTang(payload: Tang): Observable<ApiResponse<Tang>> {
    return this.http.put<ApiResponse<Tang>>(`${this.base}/tang`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  deleteTang(ids: number[]): Observable<ApiResponse<Tang[]>> {
    return this.http.request<ApiResponse<Tang[]>>('delete', `${this.base}/tang`, { body: { ids } })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  getTangList(query: any): Observable<ApiResponse<PagedResult<Tang>>> {
    return this.http.post<ApiResponse<PagedResult<Tang>>>(`${this.base}/tang/get-list`, query || {});
  }

  getTangById(id: number): Observable<ApiResponse<Tang>> {
    return this.http.post<ApiResponse<Tang>>(`${this.base}/tang/get-by-id`, { id });
  }

  // Catalog
  getTrangThaiToaNhaForSelector(): Observable<ApiResponse<any[]>> {
	return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/trang-thai-toa-nha-for-selector`, {});
  }

  getLoaiTangForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/loai-tang-for-selector`, {});
  }

  getLoaiCanHoForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/loai-can-ho-for-selector`, {});
  }

  getTinhTrangCanHoForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/tinh-trang-can-ho-for-selector`, {});
  }

  getLoaiQuanHeCuTruForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/loai-quan-he-cu-tru-for-selector`, {});
  }

  getLoaiYeuCauForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/loai-yeu-cau-for-selector`, {});
  }

  getTrangThaiYeuCauForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/trang-thai-yeu-cau-for-selector`, {});
  }

  getTrangThaiThiCongForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/trang-thai-thi-cong-for-selector`, {});
  }

  getLoaiGiayToForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/loai-giay-to-for-selector`, {});
  }

  getTrangThaiCuTruForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/trang-thai-cu-tru-for-selector`, {});
  }

  getCauTrucChungCu(keyword?: string): Observable<ApiResponse<CauTrucToaNha[]>> {
	const body: any = {};
	if (keyword) body.keyword = keyword;
	return this.http.post<ApiResponse<CauTrucToaNha[]>>(`${this.base}/catalog/cau-truc-chung-cu`, body);
  }

  // Catalog - phuong tien
  getLoaiPhuongTienForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/loai-phuong-tien-for-selector`, {});
  }

  getTrangThaiPhuongTienForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/trang-thai-phuong-tien-for-selector`, {});
  }

  // Can ho
  createCanHo(payload: CanHo): Observable<ApiResponse<CanHo>> {
    return this.http.post<ApiResponse<CanHo>>(`${this.base}/can-ho`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  updateCanHo(payload: CanHo): Observable<ApiResponse<CanHo>> {
    return this.http.put<ApiResponse<CanHo>>(`${this.base}/can-ho`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  deleteCanHo(ids: number[]): Observable<ApiResponse<CanHo[]>> {
    return this.http.request<ApiResponse<CanHo[]>>('delete', `${this.base}/can-ho`, { body: { ids } })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  getCanHoList(query: any): Observable<ApiResponse<PagedResult<CanHo>>> {
    return this.http.post<ApiResponse<PagedResult<CanHo>>>(`${this.base}/can-ho/get-list`, query || {});
  }

  getCanHoById(id: number): Observable<ApiResponse<CanHo>> {
    return this.http.post<ApiResponse<CanHo>>(`${this.base}/can-ho/get-by-id`, { id });
  }

  // Gợi ý mã căn hộ (tangId)
  goiYMaCanHo(tangId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/can-ho/goi-y-ma-can-ho`, { tangId });
  }

  // Gợi ý mã tầng (toaNhaId, loaiTangId)
  goiYMaTang(toaNhaId: number, loaiTangId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/tang/goi-y-ma-tang`, { toaNhaId, loaiTangId });
  }

  getResidentsByCanHo(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/quan-he-cu-tru/cu-dan`, query || {});
  }

  getCuDanThongTin(quanHeCuTruId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/cu-dan/thong-tin`, { quanHeCuTruId });
  }

  // Upload media files
  uploadMedia(files: FileList | any[], target?: string): Observable<ApiResponse<any>> {

  const formData = new FormData();

    if (files instanceof FileList) {
      for (let i = 0; i < files.length; i++) {
        const f = files.item(i);
        if (f) formData.append('files', f);
      }
    } else if (Array.isArray(files)) {
      files.forEach(f => {
        if (!f) { return; }
        const real: any = (f as any).originFileObj || (f as any).file || (f as any).raw || f;
        if (real) {
          formData.append('files', real as Blob);
        }
      });
    }

    if (target) {
      const aliases = ['target', 'category', 'targetContainer', 'targetCategory', 'container', 'containerName', 'purpose'];
      for (const key of aliases) {
        try { formData.append(key, target); } catch (e) { }
      }
    }

    return this.http.post<ApiResponse<any>>(`${this.base}/upload-media`, formData);
  }

  // Quan hệ cư trú - tìm hồ sơ cư dân theo CCCD (dùng tại quầy tiếp tân)
  searchUserForQuanHeCuTru(idCard: string): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/quan-he-cu-tru/search-user`, { idCard });
  }

  // Quan hệ cư trú - tạo hồ sơ cư dân mới khi chưa tồn tại trong hệ thống
  taoHoSoCuDan(payload: any): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(`${this.base}/quan-he-cu-tru/ho-so`, payload);
  }

  // Quan hệ cư trú - thiết lập cư trú (thêm cư dân vào căn hộ)
  createQuanHeCuTru(payload: { canHoId: number | undefined; userId: number; loaiQuanHeCuTruId: number; ngayBatDau?: string }): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/quan-he-cu-tru`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  // Quan hệ cư trú - kết thúc cư trú
  ketThucQuanHeCuTru(quanHeCuTruId: number | string): Observable<ApiResponse<boolean>> {
    return this.http.request<ApiResponse<boolean>>('delete', `${this.base}/quan-he-cu-tru`, { body: { quanHeCuTruId } })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  // Quan hệ cư trú - cập nhật loại quan hệ cư trú
  updateQuanHeCuTru(payload: { quanHeCuTruId: number | string; loaiQuanHeCuTruId?: number; ngayBatDau?: string; ngayKetThuc?: string }): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.base}/quan-he-cu-tru`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  deleteQuanHeCuTru(ids: Array<number | string>): Observable<ApiResponse<any[]>> {
    return this.http.request<ApiResponse<any[]>>('delete', `${this.base}/quan-he-cu-tru`, { body: { ids } })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  // Quan hệ cư trú - bổ sung tài liệu cư dân
  boSungTaiLieuCuDan(payload: any): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.base}/quan-he-cu-tru/tai-lieu`, payload);
  }

  // Quan hệ cư trú - chỉnh sửa hồ sơ cư dân
  updateHoSoCuDan(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/quan-he-cu-tru/ho-so`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  // Quan hệ cư trú - tạo mã định danh (token)
  taoMaDinhDanh(payload: { quanHeCuTruId: number; email: string }): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.base}/quan-he-cu-tru/tao-ma-dinh-danh`, payload);
  }

  // Quan hệ cư trú - liên kết tài khoản trực tiếp (dành cho BQL/quầy tiếp tân)
  lienKetTaiKhoanCuDan(payload: { userId: number; email: string }): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/quan-he-cu-tru/lien-ket-tai-khoan`, payload);
  }

  // Quan hệ cư trú - yêu cầu cư trú (dành cho BQL quản lý)
  getYeuCauCuTruList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/quan-he-cu-tru/yeu-cau/get-list`, query || {});
  }

  getYeuCauCuTruById(requestId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/quan-he-cu-tru/yeu-cau/get-by-id`, { requestId });
  }


  pheDuyetYeuCauCuTru(yeuCauCuTruId: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.base}/quan-he-cu-tru/yeu-cau/phe-duyet`, { yeuCauCuTruId })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  tuChoiYeuCauCuTru(yeuCauCuTruId: number, lyDo: string): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.base}/quan-he-cu-tru/yeu-cau/tu-choi`, { yeuCauCuTruId, lyDo })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  deleteYeuCau(ids: number[] | Array<number | string>): Observable<ApiResponse<boolean>> {
    return this.http.request<ApiResponse<boolean>>('delete', `${this.base}/quan-he-cu-tru/yeu-cau`, { body: { ids } })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  // Phuong tien
  createPhuongTien(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phuong-tien`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  updatePhuongTien(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/phuong-tien`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  // deletePhuongTien(ids: number[]): Observable<ApiResponse<any>> {
  //   return this.http.request<ApiResponse<any>>('delete', `${this.base}/phuong-tien`, { body: { ids } })
  //     .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  // }

  kichHoatPhuongTien(phuongTienIds: number[]): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/phuong-tien/kich-hoat`, { phuongTienIds })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  huyPhuongTien(phuongTienIds: number[]): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/phuong-tien/huy`, { phuongTienIds })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  khoaPhuongTien(phuongTienIds: number[]): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/phuong-tien/khoa`, { phuongTienIds })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  updateTrangThaiPhuongTien(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/phuong-tien/trang-thai`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  taoThePhuongTien(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phuong-tien/the-phuong-tien`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  goiYMaThe(phuongTienId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phuong-tien/the-phuong-tien/goi-y-ma-the`, { phuongTienId });
  }

  getPhuongTienList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phuong-tien/get-list`, query || {});
  }

  getPhuongTienById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phuong-tien/get-by-id`, { id });
  }

  khoaThePhuongTien(theIds: number[]): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/phuong-tien/the-phuong-tien/khoa`, { theIds })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  // Yeu cau phuong tien (danh cho BQL quan ly yeu cau tu cu dan)
  getYeuCauPhuongTienList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phuong-tien/yeu-cau/get-list`, query || {});
  }

  getYeuCauPhuongTienById(requestId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/phuong-tien/yeu-cau/get-by-id`, { requestId });
  }

  pheDuyetYeuCauPhuongTien(yeuCauPhuongTienId: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.base}/phuong-tien/yeu-cau/phe-duyet`, { yeuCauPhuongTienId })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  tuChoiYeuCauPhuongTien(yeuCauPhuongTienId: number, lyDo: string): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.base}/phuong-tien/yeu-cau/tu-choi`, { yeuCauPhuongTienId, lyDo })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  deleteYeuCauPhuongTien(ids: number[] | Array<number | string>): Observable<ApiResponse<boolean>> {
    return this.http.request<ApiResponse<boolean>>('delete', `${this.base}/phuong-tien/yeu-cau`, { body: { ids } })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  // Nhan vien
  private toPascalKeys(obj: any): any {
    if (obj === null || obj === undefined) return obj;
    if (Array.isArray(obj)) return obj.map(v => this.toPascalKeys(v));
    if (typeof obj !== 'object') return obj;
    const out: any = {};
    Object.keys(obj).forEach(k => {
      out[k.charAt(0).toUpperCase() + k.slice(1)] = this.toPascalKeys(obj[k]);
    });
    return out;
  }

  createNhanVien(payload: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/nhan-vien`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  updateNhanVien(payload: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.base}/nhan-vien`, payload)
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  deleteNhanVien(ids: number[] | Array<number | string>): Observable<ApiResponse<any>> {
    return this.http.request<ApiResponse<any>>('delete', `${this.base}/nhan-vien`, { body: { ids } })
      .pipe(tap((res: any) => { if (res && res.isOk) this.refreshSubject.next(); }));
  }

  getNhanVienList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/nhan-vien/get-list`, query || {});
  }

  getNhanVienById(id: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/nhan-vien/get-by-id`, { id });
  }

  // Thông báo
  getThongBaoList(query: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/thong-bao/get-list`, query || {});
  }

  // Catalog - nhan vien
  getLoaiNhanVienForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/loai-nhan-vien-for-selector`, {});
  }

  getTrangThaiNhanVienForSelector(): Observable<ApiResponse<any[]>> {
    return this.http.post<ApiResponse<any[]>>(`${this.base}/catalog/trang-thai-nhan-vien-for-selector`, {});
  }
}
