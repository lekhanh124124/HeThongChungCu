import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

export 'package:klks_app/features/cu_tru/quan_he/models/quan_he_cu_tru_model.dart';

class HoaDon {
  final int id;
  final int canHoId;
  final String maHoaDon;
  final int thang;
  final int nam;
  final DateTime ngayLap;
  final DateTime ngayHanThanhToan;
  final double tongTien;
  final int trangThaiHoaDonId;
  final String trangThaiHoaDonTen;

  const HoaDon({
    required this.id,
    required this.canHoId,
    required this.maHoaDon,
    required this.thang,
    required this.nam,
    required this.ngayLap,
    required this.ngayHanThanhToan,
    required this.tongTien,
    required this.trangThaiHoaDonId,
    required this.trangThaiHoaDonTen,
  });

  factory HoaDon.fromJson(Map<String, dynamic> json) => HoaDon(
    id: json['id'] as int? ?? 0,
    canHoId: json['canHoId'] as int? ?? 0,
    maHoaDon: json['maHoaDon'] as String? ?? '',
    thang: json['thang'] as int? ?? 0,
    nam: json['nam'] as int? ?? 0,
    ngayLap:
        DateTime.tryParse(json['ngayLap'] as String? ?? '') ?? DateTime.now(),
    ngayHanThanhToan:
        DateTime.tryParse(json['ngayHanThanhToan'] as String? ?? '') ??
        DateTime.now(),
    tongTien: (json['tongTien'] as num? ?? 0).toDouble(),
    trangThaiHoaDonId: json['trangThaiHoaDonId'] as int? ?? 0,
    trangThaiHoaDonTen: json['trangThaiHoaDonTen'] as String? ?? '',
  );

  bool get laDaThanhToan => trangThaiHoaDonId == 3;
  bool get laQuaHan => trangThaiHoaDonId == 4;
  bool get laChuaThanhToan => trangThaiHoaDonId == 2;
  bool get laCoTheThanhToan => trangThaiHoaDonId == 2 || trangThaiHoaDonId == 5;

  bool get sapHetHan {
    final soNgayConLai = ngayHanThanhToan.difference(DateTime.now()).inDays;
    return soNgayConLai >= 0 && soNgayConLai <= 3;
  }

  String get kyThanhToan => 'Tháng $thang/$nam';
}

class ChiTietHoaDon {
  final int id;
  final int loaiChiTietHoaDonId;
  final String loaiChiTietHoaDonTen;
  final String tenMucPhi;
  final double soLuong;
  final double donGia;
  final double thanhTien;
  final int loaiDinhGiaId;
  final String loaiDinhGiaTen;
  final String ghiChu;

  const ChiTietHoaDon({
    required this.id,
    required this.loaiChiTietHoaDonId,
    required this.loaiChiTietHoaDonTen,
    required this.tenMucPhi,
    required this.soLuong,
    required this.donGia,
    required this.thanhTien,
    required this.loaiDinhGiaId,
    required this.loaiDinhGiaTen,
    required this.ghiChu,
  });

  factory ChiTietHoaDon.fromJson(Map<String, dynamic> json) => ChiTietHoaDon(
    id: json['id'] as int? ?? 0,
    loaiChiTietHoaDonId: json['loaiChiTietHoaDonId'] as int? ?? 0,
    loaiChiTietHoaDonTen: json['loaiChiTietHoaDonTen'] as String? ?? '',
    tenMucPhi: json['tenMucPhi'] as String? ?? '',
    soLuong: (json['soLuong'] as num? ?? 0).toDouble(),
    donGia: (json['donGia'] as num? ?? 0).toDouble(),
    thanhTien: (json['thanhTien'] as num? ?? 0).toDouble(),
    loaiDinhGiaId: json['loaiDinhGiaId'] as int? ?? 0,
    loaiDinhGiaTen: json['loaiDinhGiaTen'] as String? ?? '',
    ghiChu: json['ghiChu'] as String? ?? '',
  );

  bool get laLuyTien => loaiDinhGiaId == 2;
  bool get laCoDinh => loaiDinhGiaId == 1;
  bool get laDienTich => loaiDinhGiaId == 3;
  bool get laKhungGio => loaiDinhGiaId == 4;

  bool get coChiTietDrillDown =>
      laLuyTien || laCoDinh || laDienTich || laKhungGio;
}

class HoaDonDetail {
  final int id;
  final int canHoId;
  final String maHoaDon;
  final int thang;
  final int nam;
  final DateTime ngayLap;
  final DateTime ngayHanThanhToan;
  final double tongTien;
  final int trangThaiHoaDonId;
  final String trangThaiHoaDonTen;
  final String ghiChu;
  final List<ChiTietHoaDon> chiTietHoaDons;

  const HoaDonDetail({
    required this.id,
    required this.canHoId,
    required this.maHoaDon,
    required this.thang,
    required this.nam,
    required this.ngayLap,
    required this.ngayHanThanhToan,
    required this.tongTien,
    required this.trangThaiHoaDonId,
    required this.trangThaiHoaDonTen,
    required this.ghiChu,
    required this.chiTietHoaDons,
  });

  factory HoaDonDetail.fromJson(Map<String, dynamic> json) => HoaDonDetail(
    id: json['id'] as int? ?? 0,
    canHoId: json['canHoId'] as int? ?? 0,
    maHoaDon: json['maHoaDon'] as String? ?? '',
    thang: json['thang'] as int? ?? 0,
    nam: json['nam'] as int? ?? 0,
    ngayLap:
        DateTime.tryParse(json['ngayLap'] as String? ?? '') ?? DateTime.now(),
    ngayHanThanhToan:
        DateTime.tryParse(json['ngayHanThanhToan'] as String? ?? '') ??
        DateTime.now(),
    tongTien: (json['tongTien'] as num? ?? 0).toDouble(),
    trangThaiHoaDonId: json['trangThaiHoaDonId'] as int? ?? 0,
    trangThaiHoaDonTen: json['trangThaiHoaDonTen'] as String? ?? '',
    ghiChu: json['ghiChu'] as String? ?? '',
    chiTietHoaDons: (json['chiTietHoaDons'] as List<dynamic>? ?? [])
        .map((e) => ChiTietHoaDon.fromJson(e as Map<String, dynamic>))
        .toList(),
  );

  bool get laCoTheThanhToan => trangThaiHoaDonId == 2 || trangThaiHoaDonId == 5;
  bool get laDaThanhToan => trangThaiHoaDonId == 3;
  String get kyThanhToan => 'Tháng $thang/$nam';
  List<int> get chiTietHoaDonIds => chiTietHoaDons.map((e) => e.id).toList();
}

class ChiTietCoDinh {
  final int id;
  final String tenMucPhi;
  final double soLuong;
  final double donGia;
  final double thanhTien;
  final String ghiChu;

  const ChiTietCoDinh({
    required this.id,
    required this.tenMucPhi,
    required this.soLuong,
    required this.donGia,
    required this.thanhTien,
    required this.ghiChu,
  });

  factory ChiTietCoDinh.fromJson(Map<String, dynamic> json) => ChiTietCoDinh(
    id: json['id'] as int? ?? 0,
    tenMucPhi: json['tenMucPhi'] as String? ?? '',
    soLuong: (json['soLuong'] as num? ?? 0).toDouble(),
    donGia: (json['donGia'] as num? ?? 0).toDouble(),
    thanhTien: (json['thanhTien'] as num? ?? 0).toDouble(),
    ghiChu: json['ghiChu'] as String? ?? '',
  );
}

class BacThang {
  final String tenBac;
  final double tuSo;
  final double denSo;
  final double soLuong;
  final double donGia;
  final double thanhTien;

  const BacThang({
    required this.tenBac,
    required this.tuSo,
    required this.denSo,
    required this.soLuong,
    required this.donGia,
    required this.thanhTien,
  });

  factory BacThang.fromJson(Map<String, dynamic> json) => BacThang(
    tenBac: json['tenBac'] as String? ?? '',
    tuSo: (json['tuSo'] as num? ?? 0).toDouble(),
    denSo: (json['denSo'] as num? ?? 0).toDouble(),
    soLuong: (json['soLuong'] as num? ?? 0).toDouble(),
    donGia: (json['donGia'] as num? ?? 0).toDouble(),
    thanhTien: (json['thanhTien'] as num? ?? 0).toDouble(),
  );
}

class ChiTietLuyTien {
  final int id;
  final String tenMucPhi;
  final double chiSoCu;
  final double chiSoMoi;
  final double soLuongTieuThu;
  final double thanhTien;
  final String anhDongHoUrl;
  final List<BacThang> bacThang;

  const ChiTietLuyTien({
    required this.id,
    required this.tenMucPhi,
    required this.chiSoCu,
    required this.chiSoMoi,
    required this.soLuongTieuThu,
    required this.thanhTien,
    required this.anhDongHoUrl,
    required this.bacThang,
  });

  factory ChiTietLuyTien.fromJson(Map<String, dynamic> json) => ChiTietLuyTien(
    id: json['id'] as int? ?? 0,
    tenMucPhi: json['tenMucPhi'] as String? ?? '',
    chiSoCu: (json['chiSoCu'] as num? ?? 0).toDouble(),
    chiSoMoi: (json['chiSoMoi'] as num? ?? 0).toDouble(),
    soLuongTieuThu: (json['soLuongTieuThu'] as num? ?? 0).toDouble(),
    thanhTien: (json['thanhTien'] as num? ?? 0).toDouble(),
    anhDongHoUrl: json['anhDongHoUrl'] as String? ?? '',
    bacThang: (json['bacThang'] as List<dynamic>? ?? [])
        .map((e) => BacThang.fromJson(e as Map<String, dynamic>))
        .toList(),
  );
}

class ChiTietDienTich {
  final int id;
  final String tenMucPhi;
  final String tenLoaiCanHo;
  final double dienTich;
  final double donGia;
  final double thanhTien;

  const ChiTietDienTich({
    required this.id,
    required this.tenMucPhi,
    required this.tenLoaiCanHo,
    required this.dienTich,
    required this.donGia,
    required this.thanhTien,
  });

  factory ChiTietDienTich.fromJson(Map<String, dynamic> json) =>
      ChiTietDienTich(
        id: json['id'] as int? ?? 0,
        tenMucPhi: json['tenMucPhi'] as String? ?? '',
        tenLoaiCanHo: json['tenLoaiCanHo'] as String? ?? '',
        dienTich: (json['dienTich'] as num? ?? 0).toDouble(),
        donGia: (json['donGia'] as num? ?? 0).toDouble(),
        thanhTien: (json['thanhTien'] as num? ?? 0).toDouble(),
      );
}

class KhungGioHoaDon {
  final String tenKhungGio;
  final String gioBatDau;
  final String gioKetThuc;
  final double donGia;

  const KhungGioHoaDon({
    required this.tenKhungGio,
    required this.gioBatDau,
    required this.gioKetThuc,
    required this.donGia,
  });

  factory KhungGioHoaDon.fromJson(Map<String, dynamic> json) => KhungGioHoaDon(
    tenKhungGio: json['tenKhungGio'] as String? ?? '',
    gioBatDau: json['gioBatDau'] as String? ?? '',
    gioKetThuc: json['gioKetThuc'] as String? ?? '',
    donGia: (json['donGia'] as num? ?? 0).toDouble(),
  );
}

class ChiTietKhungGio {
  final int id;
  final String tenMucPhi;
  final double thanhTien;
  final List<KhungGioHoaDon> khungGios;

  const ChiTietKhungGio({
    required this.id,
    required this.tenMucPhi,
    required this.thanhTien,
    required this.khungGios,
  });

  factory ChiTietKhungGio.fromJson(Map<String, dynamic> json) =>
      ChiTietKhungGio(
        id: json['id'] as int? ?? 0,
        tenMucPhi: json['tenMucPhi'] as String? ?? '',
        thanhTien: (json['thanhTien'] as num? ?? 0).toDouble(),
        khungGios: (json['khungGios'] as List<dynamic>? ?? [])
            .map((e) => KhungGioHoaDon.fromJson(e as Map<String, dynamic>))
            .toList(),
      );
}

class PhienThanhToan {
  final String maThanhToan;
  final double soTien;
  final String vietQrUrl;

  const PhienThanhToan({
    required this.maThanhToan,
    required this.soTien,
    required this.vietQrUrl,
  });

  factory PhienThanhToan.fromJson(Map<String, dynamic> json) => PhienThanhToan(
    maThanhToan: json['maThanhToan'] as String? ?? '',
    soTien: (json['soTien'] as num? ?? 0).toDouble(),
    vietQrUrl: json['vietQrUrl'] as String? ?? '',
  );
}

final _currencyFmt = NumberFormat('#,###', 'vi_VN');
final _dateFmt = DateFormat('dd/MM/yyyy');

String formatTien(double amount) => '${_currencyFmt.format(amount)} đ';
String formatNgay(DateTime dt) => _dateFmt.format(dt);
String formatSoThap(double v) =>
    v == v.truncateToDouble() ? v.toInt().toString() : v.toString();

class TrangThaiHoaDonConfig {
  final String ten;
  final Color mau;
  final Color mauNen;
  final IconData icon;

  const TrangThaiHoaDonConfig({
    required this.ten,
    required this.mau,
    required this.mauNen,
    required this.icon,
  });
}

TrangThaiHoaDonConfig getTrangThaiConfig(int id) => switch (id) {
  1 => const TrangThaiHoaDonConfig(
    ten: 'Chờ duyệt',
    mau: Color(0xFFF59E0B),
    mauNen: Color(0xFFFEF3C7),
    icon: Icons.hourglass_empty_rounded,
  ),
  2 => const TrangThaiHoaDonConfig(
    ten: 'Chưa thanh toán',
    mau: Color(0xFFF97316),
    mauNen: Color(0xFFFFF7ED),
    icon: Icons.payment_rounded,
  ),
  3 => const TrangThaiHoaDonConfig(
    ten: 'Đã thanh toán',
    mau: Color(0xFF16A34A),
    mauNen: Color(0xFFF0FDF4),
    icon: Icons.check_circle_rounded,
  ),
  4 => const TrangThaiHoaDonConfig(
    ten: 'Quá hạn',
    mau: Color(0xFFDC2626),
    mauNen: Color(0xFFFEF2F2),
    icon: Icons.warning_rounded,
  ),
  5 => const TrangThaiHoaDonConfig(
    ten: 'Thanh toán một phần',
    mau: Color(0xFF0891B2),
    mauNen: Color(0xFFECFEFF),
    icon: Icons.incomplete_circle_rounded,
  ),
  6 => const TrangThaiHoaDonConfig(
    ten: 'Đã hủy',
    mau: Color(0xFF6B7280),
    mauNen: Color(0xFFF9FAFB),
    icon: Icons.cancel_rounded,
  ),
  _ => const TrangThaiHoaDonConfig(
    ten: 'Không xác định',
    mau: Color(0xFF6B7280),
    mauNen: Color(0xFFF9FAFB),
    icon: Icons.help_outline_rounded,
  ),
};

IconData getLoaiDinhGiaIcon(int loaiDinhGiaId) => switch (loaiDinhGiaId) {
  1 => Icons.receipt_long_rounded,
  2 => Icons.show_chart_rounded,
  3 => Icons.square_foot_rounded,
  4 => Icons.access_time_rounded,
  _ => Icons.attach_money_rounded,
};

String getLoaiDinhGiaLabel(int loaiDinhGiaId, String fallback) =>
    switch (loaiDinhGiaId) {
      1 => 'Cố định',
      2 => 'Lũy tiến',
      3 => 'Diện tích',
      4 => 'Khung giờ',
      _ => fallback,
    };
