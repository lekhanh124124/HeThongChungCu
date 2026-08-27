export 'package:klks_app/features/shared/models/file_model.dart';
export 'package:klks_app/features/shared/models/paging_model.dart';
export 'package:klks_app/features/shared/models/selector_item_model.dart';
export 'package:klks_app/features/cu_tru/quan_he/models/quan_he_cu_tru_model.dart'
    show QuanHeCuTruModel;

import 'package:klks_app/features/shared/models/file_model.dart';

abstract class TrangThaiYeuCau {
  static const int pending = 1;
  static const int approved = 2;
  static const int rejected = 3;
  static const int saved = 4;
  static const int withdrawn = 5;
  static const int expired = 6;
  static const int completed = 7;
  static const int cancelled = 8;
  static const int returned = 9;

  static const Set<int> coTheChinhSua = {saved, returned};
  static const Set<int> coTheThuHoi = {saved, pending};
}

abstract class TrangThaiThiCong {
  static const int chuaThiCong = 1;
  static const int choThuTienCoc = 2;
  static const int daCapPhep = 3;
  static const int daHoanTat = 4;
}

class NhanSuThiCong {
  final int? id;
  final int? nhanVienId;
  final String hoTen;
  final String soCCCD;
  final String soDienThoai;
  final String vaiTro;
  final String ghiChu;
  final String lyDoXoa;

  const NhanSuThiCong({
    this.id,
    this.nhanVienId,
    required this.hoTen,
    required this.soCCCD,
    required this.soDienThoai,
    required this.vaiTro,
    this.ghiChu = '',
    this.lyDoXoa = '',
  });

  factory NhanSuThiCong.fromJson(Map<String, dynamic> json) => NhanSuThiCong(
    id: json['id'] as int?,
    nhanVienId: json['nhanVienId'] as int?,
    hoTen: json['hoTen'] as String? ?? '',
    soCCCD: json['soCCCD'] as String? ?? '',
    soDienThoai: json['soDienThoai'] as String? ?? '',
    vaiTro: json['vaiTro'] as String? ?? '',
    ghiChu: json['ghiChu'] as String? ?? '',
    lyDoXoa: json['lyDoXoa'] as String? ?? '',
  );

  Map<String, dynamic> toJson() => {
    if (id != null) 'id': id,
    'hoTen': hoTen,
    'soCCCD': soCCCD,
    'soDienThoai': soDienThoai,
    'vaiTro': vaiTro,
    'ghiChu': ghiChu,
  };

  NhanSuThiCong copyWith({
    int? id,
    int? nhanVienId,
    String? hoTen,
    String? soCCCD,
    String? soDienThoai,
    String? vaiTro,
    String? ghiChu,
    String? lyDoXoa,
  }) => NhanSuThiCong(
    id: id ?? this.id,
    nhanVienId: nhanVienId ?? this.nhanVienId,
    hoTen: hoTen ?? this.hoTen,
    soCCCD: soCCCD ?? this.soCCCD,
    soDienThoai: soDienThoai ?? this.soDienThoai,
    vaiTro: vaiTro ?? this.vaiTro,
    ghiChu: ghiChu ?? this.ghiChu,
    lyDoXoa: lyDoXoa ?? this.lyDoXoa,
  );
}

class YeuCauThiCongListItem {
  final int id;
  final int canHoId;
  final String tenCanHo;
  final String hangMucThiCong;
  final DateTime? duKienBatDau;
  final DateTime? duKienKetThuc;
  final String tenDonViThiCong;
  final int trangThaiYeuCauId;
  final String trangThaiYeuCauTen;
  final int trangThaiThiCongId;
  final String trangThaiThiCongTen;
  final DateTime? createdAt;
  final int createdBy;
  final String tenNguoiGui;

  const YeuCauThiCongListItem({
    required this.id,
    required this.canHoId,
    required this.tenCanHo,
    required this.hangMucThiCong,
    this.duKienBatDau,
    this.duKienKetThuc,
    required this.tenDonViThiCong,
    required this.trangThaiYeuCauId,
    required this.trangThaiYeuCauTen,
    required this.trangThaiThiCongId,
    required this.trangThaiThiCongTen,
    this.createdAt,
    required this.createdBy,
    required this.tenNguoiGui,
  });

  bool get isReturned => trangThaiYeuCauId == TrangThaiYeuCau.returned;
  bool get coTheChinhSua =>
      TrangThaiYeuCau.coTheChinhSua.contains(trangThaiYeuCauId);
  bool get coTheThuHoi =>
      TrangThaiYeuCau.coTheThuHoi.contains(trangThaiYeuCauId);

  factory YeuCauThiCongListItem.fromJson(Map<String, dynamic> json) =>
      YeuCauThiCongListItem(
        id: json['id'] as int? ?? 0,
        canHoId: json['canHoId'] as int? ?? 0,
        tenCanHo: json['tenCanHo'] as String? ?? '',
        hangMucThiCong: json['hangMucThiCong'] as String? ?? '',
        duKienBatDau: json['duKienBatDau'] != null
            ? DateTime.tryParse(json['duKienBatDau'] as String)
            : null,
        duKienKetThuc: json['duKienKetThuc'] != null
            ? DateTime.tryParse(json['duKienKetThuc'] as String)
            : null,
        tenDonViThiCong: json['tenDonViThiCong'] as String? ?? '',
        trangThaiYeuCauId: json['trangThaiYeuCauId'] as int? ?? 0,
        trangThaiYeuCauTen: json['trangThaiYeuCauTen'] as String? ?? '',
        trangThaiThiCongId: json['trangThaiThiCongId'] as int? ?? 0,
        trangThaiThiCongTen: json['trangThaiThiCongTen'] as String? ?? '',
        createdAt: json['createdAt'] != null
            ? DateTime.tryParse(json['createdAt'] as String)
            : null,
        createdBy: json['createdBy'] as int? ?? 0,
        tenNguoiGui: json['tenNguoiGui'] as String? ?? '',
      );

  Map<String, dynamic> toJson() => {
    'id': id,
    'canHoId': canHoId,
    'tenCanHo': tenCanHo,
    'hangMucThiCong': hangMucThiCong,
    'duKienBatDau': duKienBatDau?.toIso8601String(),
    'duKienKetThuc': duKienKetThuc?.toIso8601String(),
    'tenDonViThiCong': tenDonViThiCong,
    'trangThaiYeuCauId': trangThaiYeuCauId,
    'trangThaiYeuCauTen': trangThaiYeuCauTen,
    'trangThaiThiCongId': trangThaiThiCongId,
    'trangThaiThiCongTen': trangThaiThiCongTen,
    'createdAt': createdAt?.toIso8601String(),
    'createdBy': createdBy,
    'tenNguoiGui': tenNguoiGui,
  };

  YeuCauThiCongListItem copyWith({
    int? id,
    int? canHoId,
    String? tenCanHo,
    String? hangMucThiCong,
    DateTime? duKienBatDau,
    DateTime? duKienKetThuc,
    String? tenDonViThiCong,
    int? trangThaiYeuCauId,
    String? trangThaiYeuCauTen,
    int? trangThaiThiCongId,
    String? trangThaiThiCongTen,
    DateTime? createdAt,
    int? createdBy,
    String? tenNguoiGui,
  }) => YeuCauThiCongListItem(
    id: id ?? this.id,
    canHoId: canHoId ?? this.canHoId,
    tenCanHo: tenCanHo ?? this.tenCanHo,
    hangMucThiCong: hangMucThiCong ?? this.hangMucThiCong,
    duKienBatDau: duKienBatDau ?? this.duKienBatDau,
    duKienKetThuc: duKienKetThuc ?? this.duKienKetThuc,
    tenDonViThiCong: tenDonViThiCong ?? this.tenDonViThiCong,
    trangThaiYeuCauId: trangThaiYeuCauId ?? this.trangThaiYeuCauId,
    trangThaiYeuCauTen: trangThaiYeuCauTen ?? this.trangThaiYeuCauTen,
    trangThaiThiCongId: trangThaiThiCongId ?? this.trangThaiThiCongId,
    trangThaiThiCongTen: trangThaiThiCongTen ?? this.trangThaiThiCongTen,
    createdAt: createdAt ?? this.createdAt,
    createdBy: createdBy ?? this.createdBy,
    tenNguoiGui: tenNguoiGui ?? this.tenNguoiGui,
  );
}

class YeuCauThiCongDetail {
  final int id;
  final int canHoId;
  final String tenCanHo;
  final String hangMucThiCong;
  final DateTime? duKienBatDau;
  final DateTime? duKienKetThuc;
  final String tenDonViThiCong;
  final int trangThaiYeuCauId;
  final String trangThaiYeuCauTen;
  final int trangThaiThiCongId;
  final String trangThaiThiCongTen;
  final DateTime? createdAt;
  final int createdBy;
  final String tenNguoiGui;
  final String noiDung;
  final String nguoiDaiDien;
  final String soDienThoaiDaiDien;
  final double tienDatCoc;
  final bool isDaThuCoc;
  final String ghiChuThuCoc;
  final double tienKhauTru;
  final String lyDoKhauTru;
  final bool isDaHoanCoc;
  final int? nguoiXuLyId;
  final String tenNguoiXuLy;
  final DateTime? ngayXuLy;
  final String lyDo;
  final List<NhanSuThiCong> nhanSuThiCongs;

  final List<FileAttachment> danhSachTep;

  const YeuCauThiCongDetail({
    required this.id,
    required this.canHoId,
    required this.tenCanHo,
    required this.hangMucThiCong,
    this.duKienBatDau,
    this.duKienKetThuc,
    required this.tenDonViThiCong,
    required this.trangThaiYeuCauId,
    required this.trangThaiYeuCauTen,
    required this.trangThaiThiCongId,
    required this.trangThaiThiCongTen,
    this.createdAt,
    required this.createdBy,
    required this.tenNguoiGui,
    required this.noiDung,
    required this.nguoiDaiDien,
    required this.soDienThoaiDaiDien,
    required this.tienDatCoc,
    required this.isDaThuCoc,
    required this.ghiChuThuCoc,
    required this.tienKhauTru,
    required this.lyDoKhauTru,
    required this.isDaHoanCoc,
    this.nguoiXuLyId,
    required this.tenNguoiXuLy,
    this.ngayXuLy,
    required this.lyDo,
    required this.nhanSuThiCongs,
    required this.danhSachTep,
  });

  bool get coTheChinhSua =>
      TrangThaiYeuCau.coTheChinhSua.contains(trangThaiYeuCauId);
  bool get isReturned => trangThaiYeuCauId == TrangThaiYeuCau.returned;
  bool get coTheThuHoi =>
      TrangThaiYeuCau.coTheThuHoi.contains(trangThaiYeuCauId);

  String get tienDatCocFormatted =>
      '${tienDatCoc.toStringAsFixed(0).replaceAllMapped(RegExp(r'(\d{1,3})(?=(\d{3})+(?!\d))'), (m) => '${m[1]}.')} đ';

  factory YeuCauThiCongDetail.fromJson(Map<String, dynamic> json) =>
      YeuCauThiCongDetail(
        id: json['id'] as int? ?? 0,
        canHoId: json['canHoId'] as int? ?? 0,
        tenCanHo: json['tenCanHo'] as String? ?? '',
        hangMucThiCong: json['hangMucThiCong'] as String? ?? '',
        duKienBatDau: json['duKienBatDau'] != null
            ? DateTime.tryParse(json['duKienBatDau'] as String)
            : null,
        duKienKetThuc: json['duKienKetThuc'] != null
            ? DateTime.tryParse(json['duKienKetThuc'] as String)
            : null,
        tenDonViThiCong: json['tenDonViThiCong'] as String? ?? '',
        trangThaiYeuCauId: json['trangThaiYeuCauId'] as int? ?? 0,
        trangThaiYeuCauTen: json['trangThaiYeuCauTen'] as String? ?? '',
        trangThaiThiCongId: json['trangThaiThiCongId'] as int? ?? 0,
        trangThaiThiCongTen: json['trangThaiThiCongTen'] as String? ?? '',
        createdAt: json['createdAt'] != null
            ? DateTime.tryParse(json['createdAt'] as String)
            : null,
        createdBy: json['createdBy'] as int? ?? 0,
        tenNguoiGui: json['tenNguoiGui'] as String? ?? '',
        noiDung: json['noiDung'] as String? ?? '',
        nguoiDaiDien: json['nguoiDaiDien'] as String? ?? '',
        soDienThoaiDaiDien: json['soDienThoaiDaiDien'] as String? ?? '',
        tienDatCoc: (json['tienDatCoc'] as num?)?.toDouble() ?? 0,
        isDaThuCoc: json['isDaThuCoc'] as bool? ?? false,
        ghiChuThuCoc: json['ghiChuThuCoc'] as String? ?? '',
        tienKhauTru: (json['tienKhauTru'] as num?)?.toDouble() ?? 0,
        lyDoKhauTru: json['lyDoKhauTru'] as String? ?? '',
        isDaHoanCoc: json['isDaHoanCoc'] as bool? ?? false,
        nguoiXuLyId: json['nguoiXuLyId'] as int?,
        tenNguoiXuLy: json['tenNguoiXuLy'] as String? ?? '',
        ngayXuLy: json['ngayXuLy'] != null
            ? DateTime.tryParse(json['ngayXuLy'] as String)
            : null,
        lyDo: json['lyDo'] as String? ?? '',
        nhanSuThiCongs: (json['nhanSuThiCongs'] as List<dynamic>? ?? [])
            .map((e) => NhanSuThiCong.fromJson(e as Map<String, dynamic>))
            .toList(),
        danhSachTep: (json['danhSachTep'] as List<dynamic>? ?? [])
            .map((e) => FileAttachment.fromJson(e as Map<String, dynamic>))
            .toList(),
      );

  Map<String, dynamic> toJson() => {
    'id': id,
    'canHoId': canHoId,
    'tenCanHo': tenCanHo,
    'hangMucThiCong': hangMucThiCong,
    'duKienBatDau': duKienBatDau?.toIso8601String(),
    'duKienKetThuc': duKienKetThuc?.toIso8601String(),
    'tenDonViThiCong': tenDonViThiCong,
    'trangThaiYeuCauId': trangThaiYeuCauId,
    'trangThaiYeuCauTen': trangThaiYeuCauTen,
    'trangThaiThiCongId': trangThaiThiCongId,
    'trangThaiThiCongTen': trangThaiThiCongTen,
    'createdAt': createdAt?.toIso8601String(),
    'createdBy': createdBy,
    'tenNguoiGui': tenNguoiGui,
    'noiDung': noiDung,
    'nguoiDaiDien': nguoiDaiDien,
    'soDienThoaiDaiDien': soDienThoaiDaiDien,
    'tienDatCoc': tienDatCoc,
    'isDaThuCoc': isDaThuCoc,
    'ghiChuThuCoc': ghiChuThuCoc,
    'tienKhauTru': tienKhauTru,
    'lyDoKhauTru': lyDoKhauTru,
    'isDaHoanCoc': isDaHoanCoc,
    'nguoiXuLyId': nguoiXuLyId,
    'tenNguoiXuLy': tenNguoiXuLy,
    'ngayXuLy': ngayXuLy?.toIso8601String(),
    'lyDo': lyDo,
    'nhanSuThiCongs': nhanSuThiCongs.map((e) => e.toJson()).toList(),
    'danhSachTep': danhSachTep.map((e) => e.toJson()).toList(),
  };

  YeuCauThiCongDetail copyWith({
    int? id,
    int? canHoId,
    String? tenCanHo,
    String? hangMucThiCong,
    DateTime? duKienBatDau,
    DateTime? duKienKetThuc,
    String? tenDonViThiCong,
    int? trangThaiYeuCauId,
    String? trangThaiYeuCauTen,
    int? trangThaiThiCongId,
    String? trangThaiThiCongTen,
    DateTime? createdAt,
    int? createdBy,
    String? tenNguoiGui,
    String? noiDung,
    String? nguoiDaiDien,
    String? soDienThoaiDaiDien,
    double? tienDatCoc,
    bool? isDaThuCoc,
    String? ghiChuThuCoc,
    double? tienKhauTru,
    String? lyDoKhauTru,
    bool? isDaHoanCoc,
    int? nguoiXuLyId,
    String? tenNguoiXuLy,
    DateTime? ngayXuLy,
    String? lyDo,
    List<NhanSuThiCong>? nhanSuThiCongs,
    List<FileAttachment>? danhSachTep,
  }) => YeuCauThiCongDetail(
    id: id ?? this.id,
    canHoId: canHoId ?? this.canHoId,
    tenCanHo: tenCanHo ?? this.tenCanHo,
    hangMucThiCong: hangMucThiCong ?? this.hangMucThiCong,
    duKienBatDau: duKienBatDau ?? this.duKienBatDau,
    duKienKetThuc: duKienKetThuc ?? this.duKienKetThuc,
    tenDonViThiCong: tenDonViThiCong ?? this.tenDonViThiCong,
    trangThaiYeuCauId: trangThaiYeuCauId ?? this.trangThaiYeuCauId,
    trangThaiYeuCauTen: trangThaiYeuCauTen ?? this.trangThaiYeuCauTen,
    trangThaiThiCongId: trangThaiThiCongId ?? this.trangThaiThiCongId,
    trangThaiThiCongTen: trangThaiThiCongTen ?? this.trangThaiThiCongTen,
    createdAt: createdAt ?? this.createdAt,
    createdBy: createdBy ?? this.createdBy,
    tenNguoiGui: tenNguoiGui ?? this.tenNguoiGui,
    noiDung: noiDung ?? this.noiDung,
    nguoiDaiDien: nguoiDaiDien ?? this.nguoiDaiDien,
    soDienThoaiDaiDien: soDienThoaiDaiDien ?? this.soDienThoaiDaiDien,
    tienDatCoc: tienDatCoc ?? this.tienDatCoc,
    isDaThuCoc: isDaThuCoc ?? this.isDaThuCoc,
    ghiChuThuCoc: ghiChuThuCoc ?? this.ghiChuThuCoc,
    tienKhauTru: tienKhauTru ?? this.tienKhauTru,
    lyDoKhauTru: lyDoKhauTru ?? this.lyDoKhauTru,
    isDaHoanCoc: isDaHoanCoc ?? this.isDaHoanCoc,
    nguoiXuLyId: nguoiXuLyId ?? this.nguoiXuLyId,
    tenNguoiXuLy: tenNguoiXuLy ?? this.tenNguoiXuLy,
    ngayXuLy: ngayXuLy ?? this.ngayXuLy,
    lyDo: lyDo ?? this.lyDo,
    nhanSuThiCongs: nhanSuThiCongs ?? this.nhanSuThiCongs,
    danhSachTep: danhSachTep ?? this.danhSachTep,
  );
}
