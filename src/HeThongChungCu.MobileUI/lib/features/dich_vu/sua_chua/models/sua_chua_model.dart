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

abstract class TrangThaiSuaChua {
  static const int daDieuPhoi = 1;
  static const int choBaoGia = 2;
  static const int daDuyetBaoGia = 3;
  static const int daHenLich = 4;
}

class NhanSuSuaChua {
  final int id;
  final int? nhanVienId;
  final String? hoTen;
  final String? soCCCD;
  final String? soDienThoai;
  final String vaiTro;
  final String? ghiChu;

  String get displayName =>
      hoTen?.isNotEmpty == true ? hoTen! : 'Nhân viên #$id';

  const NhanSuSuaChua({
    required this.id,
    this.nhanVienId,
    this.hoTen,
    this.soCCCD,
    this.soDienThoai,
    required this.vaiTro,
    this.ghiChu,
  });

  factory NhanSuSuaChua.fromJson(Map<String, dynamic> json) => NhanSuSuaChua(
    id: json['id'] as int,
    nhanVienId: json['nhanVienId'] as int?,
    hoTen: json['hoTen'] as String?,
    soCCCD: json['soCCCD'] as String?,
    soDienThoai: json['soDienThoai'] as String?,
    vaiTro: json['vaiTro'] as String? ?? '',
    ghiChu: json['ghiChu'] as String?,
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'nhanVienId': nhanVienId,
    'hoTen': hoTen,
    'soCCCD': soCCCD,
    'soDienThoai': soDienThoai,
    'vaiTro': vaiTro,
    'ghiChu': ghiChu,
  };
}

class YeuCauSuaChua {
  final int id;
  final int canHoId;
  final String? tenCanHo;
  final String? tenTang;
  final String? tenToaNha;
  final int? loaiYeuCauCuDanId;
  final String? loaiYeuCauCuDanTen;
  final int trangThaiYeuCauId;
  final String? trangThaiYeuCauTen;
  final String noiDung;
  final int? loaiSuCoId;
  final String? loaiSuCoTen;
  final int? trangThaiSuaChuaId;
  final String? trangThaiSuaChuaTen;
  final DateTime? createdAt;
  final int? createdBy;
  final String? tenNguoiGui;
  final String? lyDo;
  final int? nguoiXuLyId;
  final String? tenNguoiXuLy;
  final DateTime? ngayXuLy;
  final int? phamViId;
  final String? phamViTen;
  final String? moTaViTri;
  final DateTime? henTu;
  final DateTime? henDen;
  final double? chiPhiDuKien;
  final double? chiPhiThucTe;
  final bool? isMienPhi;
  final String? ghiChuBaoGia;
  final String? ketQuaXuLy;
  final String? lyDoHuy;
  final int? hopDongDoiTacId;
  final String? tenDoiTac;
  final List<NhanSuSuaChua> nhanSuSuaChuas;
  final List<FileAttachment> danhSachTep;

  const YeuCauSuaChua({
    required this.id,
    required this.canHoId,
    this.tenCanHo,
    this.tenTang,
    this.tenToaNha,
    this.loaiYeuCauCuDanId,
    this.loaiYeuCauCuDanTen,
    required this.trangThaiYeuCauId,
    this.trangThaiYeuCauTen,
    required this.noiDung,
    this.loaiSuCoId,
    this.loaiSuCoTen,
    this.trangThaiSuaChuaId,
    this.trangThaiSuaChuaTen,
    this.createdAt,
    this.createdBy,
    this.tenNguoiGui,
    this.lyDo,
    this.nguoiXuLyId,
    this.tenNguoiXuLy,
    this.ngayXuLy,
    this.phamViId,
    this.phamViTen,
    this.moTaViTri,
    this.henTu,
    this.henDen,
    this.chiPhiDuKien,
    this.chiPhiThucTe,
    this.isMienPhi,
    this.ghiChuBaoGia,
    this.ketQuaXuLy,
    this.lyDoHuy,
    this.hopDongDoiTacId,
    this.tenDoiTac,
    this.nhanSuSuaChuas = const [],
    this.danhSachTep = const [],
  });

  String get diaChiDayDu {
    final parts = [
      tenToaNha,
      tenTang,
      tenCanHo,
    ].where((s) => s != null && s.isNotEmpty).toList();
    return parts.isNotEmpty ? parts.join(' - ') : 'Căn hộ #$canHoId';
  }

  bool get coTheChinhSua =>
      TrangThaiYeuCau.coTheChinhSua.contains(trangThaiYeuCauId);
  bool get coTheThuHoi =>
      TrangThaiYeuCau.coTheThuHoi.contains(trangThaiYeuCauId);
  bool get daKetThuc => {
    TrangThaiYeuCau.completed,
    TrangThaiYeuCau.cancelled,
    TrangThaiYeuCau.withdrawn,
    TrangThaiYeuCau.rejected,
    TrangThaiYeuCau.expired,
  }.contains(trangThaiYeuCauId);

  factory YeuCauSuaChua.fromJson(Map<String, dynamic> json) => YeuCauSuaChua(
    id: json['id'] as int,
    canHoId: json['canHoId'] as int,
    tenCanHo: json['tenCanHo'] as String?,
    tenTang: json['tenTang'] as String?,
    tenToaNha: json['tenToaNha'] as String?,
    loaiYeuCauCuDanId: json['loaiYeuCauCuDanId'] as int?,
    loaiYeuCauCuDanTen: json['loaiYeuCauCuDanTen'] as String?,
    trangThaiYeuCauId: json['trangThaiYeuCauId'] as int,
    trangThaiYeuCauTen: json['trangThaiYeuCauTen'] as String?,
    noiDung: json['noiDung'] as String? ?? '',
    loaiSuCoId: json['loaiSuCoId'] as int?,
    loaiSuCoTen: json['loaiSuCoTen'] as String?,
    trangThaiSuaChuaId: json['trangThaiSuaChuaId'] as int?,
    trangThaiSuaChuaTen: json['trangThaiSuaChuaTen'] as String?,
    createdAt: json['createdAt'] != null
        ? DateTime.tryParse(json['createdAt'] as String)
        : null,
    createdBy: json['createdBy'] as int?,
    tenNguoiGui: json['tenNguoiGui'] as String?,
    lyDo: json['lyDo'] as String?,
    nguoiXuLyId: json['nguoiXuLyId'] as int?,
    tenNguoiXuLy: json['tenNguoiXuLy'] as String?,
    ngayXuLy: json['ngayXuLy'] != null
        ? DateTime.tryParse(json['ngayXuLy'] as String)
        : null,
    phamViId: json['phamViId'] as int?,
    phamViTen: json['phamViTen'] as String?,
    moTaViTri: json['moTaViTri'] as String?,
    henTu: json['henTu'] != null
        ? DateTime.tryParse(json['henTu'] as String)
        : null,
    henDen: json['henDen'] != null
        ? DateTime.tryParse(json['henDen'] as String)
        : null,
    chiPhiDuKien: (json['chiPhiDuKien'] as num?)?.toDouble(),
    chiPhiThucTe: (json['chiPhiThucTe'] as num?)?.toDouble(),
    isMienPhi: json['isMienPhi'] as bool?,
    ghiChuBaoGia: json['ghiChuBaoGia'] as String?,
    ketQuaXuLy: json['ketQuaXuLy'] as String?,
    lyDoHuy: json['lyDoHuy'] as String?,
    hopDongDoiTacId: json['hopDongDoiTacId'] as int?,
    tenDoiTac: json['tenDoiTac'] as String?,
    nhanSuSuaChuas: (json['nhanSuSuaChuas'] as List<dynamic>? ?? [])
        .map((e) => NhanSuSuaChua.fromJson(e as Map<String, dynamic>))
        .toList(),
    danhSachTep: (json['danhSachTep'] as List<dynamic>? ?? [])
        .map((e) => FileAttachment.fromJson(e as Map<String, dynamic>))
        .toList(),
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'canHoId': canHoId,
    'tenCanHo': tenCanHo,
    'tenTang': tenTang,
    'tenToaNha': tenToaNha,
    'trangThaiYeuCauId': trangThaiYeuCauId,
    'trangThaiYeuCauTen': trangThaiYeuCauTen,
    'noiDung': noiDung,
    'loaiSuCoId': loaiSuCoId,
    'loaiSuCoTen': loaiSuCoTen,
    'trangThaiSuaChuaId': trangThaiSuaChuaId,
    'trangThaiSuaChuaTen': trangThaiSuaChuaTen,
    'createdAt': createdAt?.toIso8601String(),
    'tenNguoiGui': tenNguoiGui,
    'phamViId': phamViId,
    'phamViTen': phamViTen,
    'moTaViTri': moTaViTri,
    'henTu': henTu?.toIso8601String(),
    'henDen': henDen?.toIso8601String(),
    'chiPhiDuKien': chiPhiDuKien,
    'chiPhiThucTe': chiPhiThucTe,
    'isMienPhi': isMienPhi,
    'ghiChuBaoGia': ghiChuBaoGia,
    'ketQuaXuLy': ketQuaXuLy,
    'lyDoHuy': lyDoHuy,
    'nhanSuSuaChuas': nhanSuSuaChuas.map((e) => e.toJson()).toList(),
    'danhSachTep': danhSachTep.map((e) => e.toJson()).toList(),
  };

  YeuCauSuaChua copyWith({
    int? id,
    int? canHoId,
    String? tenCanHo,
    String? tenTang,
    String? tenToaNha,
    int? loaiYeuCauCuDanId,
    String? loaiYeuCauCuDanTen,
    int? trangThaiYeuCauId,
    String? trangThaiYeuCauTen,
    String? noiDung,
    int? loaiSuCoId,
    String? loaiSuCoTen,
    int? trangThaiSuaChuaId,
    String? trangThaiSuaChuaTen,
    DateTime? createdAt,
    int? createdBy,
    String? tenNguoiGui,
    String? lyDo,
    int? nguoiXuLyId,
    String? tenNguoiXuLy,
    DateTime? ngayXuLy,
    int? phamViId,
    String? phamViTen,
    String? moTaViTri,
    DateTime? henTu,
    DateTime? henDen,
    double? chiPhiDuKien,
    double? chiPhiThucTe,
    bool? isMienPhi,
    String? ghiChuBaoGia,
    String? ketQuaXuLy,
    String? lyDoHuy,
    int? hopDongDoiTacId,
    String? tenDoiTac,
    List<NhanSuSuaChua>? nhanSuSuaChuas,
    List<FileAttachment>? danhSachTep,
  }) => YeuCauSuaChua(
    id: id ?? this.id,
    canHoId: canHoId ?? this.canHoId,
    tenCanHo: tenCanHo ?? this.tenCanHo,
    tenTang: tenTang ?? this.tenTang,
    tenToaNha: tenToaNha ?? this.tenToaNha,
    loaiYeuCauCuDanId: loaiYeuCauCuDanId ?? this.loaiYeuCauCuDanId,
    loaiYeuCauCuDanTen: loaiYeuCauCuDanTen ?? this.loaiYeuCauCuDanTen,
    trangThaiYeuCauId: trangThaiYeuCauId ?? this.trangThaiYeuCauId,
    trangThaiYeuCauTen: trangThaiYeuCauTen ?? this.trangThaiYeuCauTen,
    noiDung: noiDung ?? this.noiDung,
    loaiSuCoId: loaiSuCoId ?? this.loaiSuCoId,
    loaiSuCoTen: loaiSuCoTen ?? this.loaiSuCoTen,
    trangThaiSuaChuaId: trangThaiSuaChuaId ?? this.trangThaiSuaChuaId,
    trangThaiSuaChuaTen: trangThaiSuaChuaTen ?? this.trangThaiSuaChuaTen,
    createdAt: createdAt ?? this.createdAt,
    createdBy: createdBy ?? this.createdBy,
    tenNguoiGui: tenNguoiGui ?? this.tenNguoiGui,
    lyDo: lyDo ?? this.lyDo,
    nguoiXuLyId: nguoiXuLyId ?? this.nguoiXuLyId,
    tenNguoiXuLy: tenNguoiXuLy ?? this.tenNguoiXuLy,
    ngayXuLy: ngayXuLy ?? this.ngayXuLy,
    phamViId: phamViId ?? this.phamViId,
    phamViTen: phamViTen ?? this.phamViTen,
    moTaViTri: moTaViTri ?? this.moTaViTri,
    henTu: henTu ?? this.henTu,
    henDen: henDen ?? this.henDen,
    chiPhiDuKien: chiPhiDuKien ?? this.chiPhiDuKien,
    chiPhiThucTe: chiPhiThucTe ?? this.chiPhiThucTe,
    isMienPhi: isMienPhi ?? this.isMienPhi,
    ghiChuBaoGia: ghiChuBaoGia ?? this.ghiChuBaoGia,
    ketQuaXuLy: ketQuaXuLy ?? this.ketQuaXuLy,
    lyDoHuy: lyDoHuy ?? this.lyDoHuy,
    hopDongDoiTacId: hopDongDoiTacId ?? this.hopDongDoiTacId,
    tenDoiTac: tenDoiTac ?? this.tenDoiTac,
    nhanSuSuaChuas: nhanSuSuaChuas ?? this.nhanSuSuaChuas,
    danhSachTep: danhSachTep ?? this.danhSachTep,
  );
}

class GetListYeuCauRequest {
  final int pageNumber;
  final int pageSize;
  final String sortCol;
  final bool isAsc;
  final int? canHoId;
  final int? trangThaiYeuCauId;
  final int? trangThaiSuaChuaId;
  final int? loaiSuCoId;
  final DateTime? ngayTaoTu;
  final DateTime? ngayTaoDen;

  const GetListYeuCauRequest({
    this.pageNumber = 1,
    this.pageSize = 10,
    this.sortCol = 'CreatedAt',
    this.isAsc = false,
    this.canHoId,
    this.trangThaiYeuCauId,
    this.trangThaiSuaChuaId,
    this.loaiSuCoId,
    this.ngayTaoTu,
    this.ngayTaoDen,
  });

  Map<String, dynamic> toJson() => {
    'pageNumber': pageNumber,
    'pageSize': pageSize,
    'sortCol': sortCol,
    'isAsc': isAsc,
    if (canHoId != null) 'canHoId': canHoId,
    if (trangThaiYeuCauId != null) 'trangThaiYeuCauId': trangThaiYeuCauId,
    if (trangThaiSuaChuaId != null) 'trangThaiSuaChuaId': trangThaiSuaChuaId,
    if (loaiSuCoId != null) 'loaiSuCoId': loaiSuCoId,
    if (ngayTaoTu != null) 'ngayTaoTu': ngayTaoTu!.toUtc().toIso8601String(),
    if (ngayTaoDen != null) 'ngayTaoDen': ngayTaoDen!.toUtc().toIso8601String(),
  };
}

class TaoYeuCauRequest {
  final int canHoId;
  final int phamViId;
  final int loaiSuCoId;
  final String noiDung;
  final String? moTaViTri;
  final List<int> danhSachTepIds;
  final bool isSubmit;

  const TaoYeuCauRequest({
    required this.canHoId,
    required this.phamViId,
    required this.loaiSuCoId,
    required this.noiDung,
    this.moTaViTri,
    this.danhSachTepIds = const [],
    required this.isSubmit,
  });

  Map<String, dynamic> toJson() => {
    'canHoId': canHoId,
    'phamViId': phamViId,
    'loaiSuCoId': loaiSuCoId,
    'noiDung': noiDung,
    'moTaViTri': moTaViTri,
    'danhSachTepIds': danhSachTepIds,
    'isSubmit': isSubmit,
  };
}

class CapNhatYeuCauRequest {
  final int id;
  final int phamViId;
  final int loaiSuCoId;
  final String noiDung;
  final String? moTaViTri;
  final List<int> danhSachTepIds;
  final bool isSubmit;
  final bool isWithdraw;

  const CapNhatYeuCauRequest({
    required this.id,
    required this.phamViId,
    required this.loaiSuCoId,
    required this.noiDung,
    this.moTaViTri,
    this.danhSachTepIds = const [],
    required this.isSubmit,
    this.isWithdraw = false,
  });

  Map<String, dynamic> toJson() => {
    'id': id,
    'phamViId': phamViId,
    'loaiSuCoId': loaiSuCoId,
    'noiDung': noiDung,
    'moTaViTri': moTaViTri,
    'danhSachTepIds': danhSachTepIds,
    'isSubmit': isSubmit,
    'isWithdraw': isWithdraw,
  };
}
