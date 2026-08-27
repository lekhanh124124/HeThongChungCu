export interface GiaoDichItem {
  id: number;
  maGiaoDich: string;
  loaiGiaoDichId: number;
  tenLoaiGiaoDich: string;
  tongSoTien: number;
  ngayGiaoDich: string;
  phuongThucThanhToanId: number;
  tenPhuongThucThanhToan: string;
  nguoiGiaoDich: string;
  chungTuGoc: string;
  chiTiets: ChiTietItem[];
}

export interface ChiTietItem {
  id: number;
  soTien: number;
  nhomThongKe: string;
  ghiChu: string;
  dichVuId?: number;
}

export interface TaoPhieuThuRequest {
  ngayGiaoDich: string;
  phuongThucThanhToanId: number;
  nguoiGiaoDich: string;
  chungTuGoc: string;
  chiTiets: PhieuThuChiTiet[];
}

export interface PhieuThuChiTiet {
  dichVuId: number;
  soTien: number;
  nhomThongKe: string;
  ghiChu: string;
}

export interface TaoPhieuChiRequest {
  ngayGiaoDich: string;
  phuongThucThanhToanId: number;
  nguoiGiaoDich: string;
  chungTuGoc: string;
  chiTiets: PhieuChiChiTiet[];
}

export interface PhieuChiChiTiet {
  nhomThongKe: string;
  soTien: number;
  ghiChu: string;
}

export interface BaoCaoThuChiResult {
  tuNgay: string;
  denNgay: string;
  soDuDauKy: number;
  tongThu: number;
  tongChi: number;
  dongTienThuan: number;
  soDuCuoiKy: number;
  danhSachKhoanThu: KhoanTienItem[];
  danhSachKhoanChi: KhoanTienItem[];
}

export interface KhoanTienItem {
  nhomThongKe: string;
  tongSoTien: number;
  soGiaoDich: number;
  tyLePhanTram: number;
}

export interface CongNoCanHoItem {
  canHoId: number;
  maCanHo: string;
  tenToaNha: string;
  tenTang: string;
  tenChuHo: string;
  noDauKy: number;
  phatSinhTrongKy: number;
  daThanhToanTrongKy: number;
  noCuoiKy: number;
}

export interface CongNoToaNhaItem {
  toaNhaId: number;
  tenToaNha: string;
  tongSoCanHo: number;
  soCanHoNoPhi: number;
  tongNoDauKy: number;
  tongPhatSinh: number;
  tongDaThu: number;
  tongNoConLai: number;
  tyLeThuHoi: number;
}
