export interface HopDongDto {
  id?: number;
  soHopDong?: string;
  ngayKy?: string;
  ngayHetHan?: string;
  giaTri?: number;
  noiDung?: string;
  tepFileIds?: number[];
  maDichVu?: string;
  tenDichVu?: string;
}

export interface DoiTacDto {
  id?: number;
  tenDoiTac: string;
  tenCongTy?: string;
  nguoiDaiDien: string;
  soGiayPhepKD?: string;
  maSoThue: string;
  diaChi: string;
  soDienThoai: string;
  email?: string;
  ghiChu?: string;
  hopDongs?: HopDongDto[];
}
