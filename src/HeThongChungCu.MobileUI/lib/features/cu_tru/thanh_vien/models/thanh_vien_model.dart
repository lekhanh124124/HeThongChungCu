export 'package:klks_app/features/cu_tru/quan_he/models/quan_he_cu_tru_model.dart';

import 'package:klks_app/features/shared/models/file_model.dart';

class ThanhVienCuTruModel {
  final int quanHeCuTruId;
  final int userId;
  final int loaiQuanHeCuTruId;
  final String loaiQuanHeTen;
  final DateTime? ngayBatDau;
  final String fullName;
  final String? anhDaiDienUrl;

  const ThanhVienCuTruModel({
    required this.quanHeCuTruId,
    required this.userId,
    required this.loaiQuanHeCuTruId,
    required this.loaiQuanHeTen,
    this.ngayBatDau,
    required this.fullName,
    this.anhDaiDienUrl,
  });

  factory ThanhVienCuTruModel.fromJson(Map<String, dynamic> json) =>
      ThanhVienCuTruModel(
        quanHeCuTruId: json['quanHeCuTruId'] as int? ?? 0,
        userId: json['userId'] as int? ?? 0,
        loaiQuanHeCuTruId: json['loaiQuanHeCuTruId'] as int? ?? 0,
        loaiQuanHeTen: json['loaiQuanHeTen'] as String? ?? '',
        ngayBatDau: json['ngayBatDau'] != null
            ? DateTime.tryParse(json['ngayBatDau'] as String)
            : null,
        fullName: json['fullName'] as String? ?? '',
        anhDaiDienUrl: json['anhDaiDienUrl'] as String?,
      );

  Map<String, dynamic> toJson() => {
    'quanHeCuTruId': quanHeCuTruId,
    'userId': userId,
    'loaiQuanHeCuTruId': loaiQuanHeCuTruId,
    'loaiQuanHeTen': loaiQuanHeTen,
    'ngayBatDau': ngayBatDau?.toIso8601String(),
    'fullName': fullName,
    'anhDaiDienUrl': anhDaiDienUrl,
  };

  ThanhVienCuTruModel copyWith({
    int? quanHeCuTruId,
    int? userId,
    int? loaiQuanHeCuTruId,
    String? loaiQuanHeTen,
    DateTime? ngayBatDau,
    String? fullName,
    String? anhDaiDienUrl,
  }) => ThanhVienCuTruModel(
    quanHeCuTruId: quanHeCuTruId ?? this.quanHeCuTruId,
    userId: userId ?? this.userId,
    loaiQuanHeCuTruId: loaiQuanHeCuTruId ?? this.loaiQuanHeCuTruId,
    loaiQuanHeTen: loaiQuanHeTen ?? this.loaiQuanHeTen,
    ngayBatDau: ngayBatDau ?? this.ngayBatDau,
    fullName: fullName ?? this.fullName,
    anhDaiDienUrl: anhDaiDienUrl ?? this.anhDaiDienUrl,
  );
}

class TaiLieuCuTruModel {
  final int id;
  final int loaiGiayToId;
  final String tenLoaiGiayTo;
  final String soGiayTo;
  final DateTime? ngayPhatHanh;
  final int? targetTaiLieuCuTruId;

  final List<FileAttachment> files;

  const TaiLieuCuTruModel({
    required this.id,
    required this.loaiGiayToId,
    required this.tenLoaiGiayTo,
    required this.soGiayTo,
    this.ngayPhatHanh,
    this.targetTaiLieuCuTruId,
    required this.files,
  });

  factory TaiLieuCuTruModel.fromJson(Map<String, dynamic> json) =>
      TaiLieuCuTruModel(
        id: json['id'] as int? ?? 0,
        loaiGiayToId: json['loaiGiayToId'] as int? ?? 0,
        tenLoaiGiayTo: json['tenLoaiGiayTo'] as String? ?? '',
        soGiayTo: json['soGiayTo'] as String? ?? '',
        ngayPhatHanh: json['ngayPhatHanh'] != null
            ? DateTime.tryParse(json['ngayPhatHanh'] as String)
            : null,
        targetTaiLieuCuTruId: json['targetTaiLieuCuTruId'] as int?,
        files: (json['files'] as List<dynamic>? ?? [])
            .map((e) => FileAttachment.fromJson(e as Map<String, dynamic>))
            .toList(),
      );

  Map<String, dynamic> toJson() => {
    'id': id,
    'loaiGiayToId': loaiGiayToId,
    'tenLoaiGiayTo': tenLoaiGiayTo,
    'soGiayTo': soGiayTo,
    'ngayPhatHanh': ngayPhatHanh?.toIso8601String(),
    'targetTaiLieuCuTruId': targetTaiLieuCuTruId,
    'files': files.map((e) => e.toJson()).toList(),
  };
}

class ThongTinCuDanModel {
  final int userId;
  final String fullName;
  final String firstName;
  final String lastName;
  final int gioiTinhId;
  final String gioiTinhName;
  final DateTime? dob;
  final String? idCard;
  final String? phoneNumber;
  final String? diaChi;
  final String? anhDaiDienUrl;
  final int quanHeCuTruId;
  final int loaiQuanHeCuTruId;
  final String loaiQuanHeTen;
  final DateTime? ngayBatDau;
  final DateTime? ngayKetThuc;
  final int trangThaiCuTruId;
  final String trangThaiCuTruTen;
  final List<TaiLieuCuTruModel> taiLieuCuTrus;

  const ThongTinCuDanModel({
    required this.userId,
    required this.fullName,
    required this.firstName,
    required this.lastName,
    required this.gioiTinhId,
    required this.gioiTinhName,
    this.dob,
    this.idCard,
    this.phoneNumber,
    this.diaChi,
    this.anhDaiDienUrl,
    required this.quanHeCuTruId,
    required this.loaiQuanHeCuTruId,
    required this.loaiQuanHeTen,
    this.ngayBatDau,
    this.ngayKetThuc,
    required this.trangThaiCuTruId,
    required this.trangThaiCuTruTen,
    required this.taiLieuCuTrus,
  });

  factory ThongTinCuDanModel.fromJson(Map<String, dynamic> json) =>
      ThongTinCuDanModel(
        userId: json['userId'] as int? ?? 0,
        fullName: json['fullName'] as String? ?? '',
        firstName: json['firstName'] as String? ?? '',
        lastName: json['lastName'] as String? ?? '',
        gioiTinhId: json['gioiTinhId'] as int? ?? 0,
        gioiTinhName: json['gioiTinhName'] as String? ?? '',
        dob: json['dob'] != null
            ? DateTime.tryParse(json['dob'] as String)
            : null,
        idCard: json['idCard'] as String?,
        phoneNumber: json['phoneNumber'] as String?,
        diaChi: json['diaChi'] as String?,
        anhDaiDienUrl: json['anhDaiDienUrl'] as String?,
        quanHeCuTruId: json['quanHeCuTruId'] as int? ?? 0,
        loaiQuanHeCuTruId: json['loaiQuanHeCuTruId'] as int? ?? 0,
        loaiQuanHeTen: json['loaiQuanHeTen'] as String? ?? '',
        ngayBatDau: json['ngayBatDau'] != null
            ? DateTime.tryParse(json['ngayBatDau'] as String)
            : null,
        ngayKetThuc: json['ngayKetThuc'] != null
            ? DateTime.tryParse(json['ngayKetThuc'] as String)
            : null,
        trangThaiCuTruId: json['trangThaiCuTruId'] as int? ?? 0,
        trangThaiCuTruTen: json['trangThaiCuTruTen'] as String? ?? '',
        taiLieuCuTrus: (json['taiLieuCuTrus'] as List<dynamic>? ?? [])
            .map((e) => TaiLieuCuTruModel.fromJson(e as Map<String, dynamic>))
            .toList(),
      );

  Map<String, dynamic> toJson() => {
    'userId': userId,
    'fullName': fullName,
    'firstName': firstName,
    'lastName': lastName,
    'gioiTinhId': gioiTinhId,
    'gioiTinhName': gioiTinhName,
    'dob': dob?.toIso8601String(),
    'idCard': idCard,
    'phoneNumber': phoneNumber,
    'diaChi': diaChi,
    'anhDaiDienUrl': anhDaiDienUrl,
    'quanHeCuTruId': quanHeCuTruId,
    'loaiQuanHeCuTruId': loaiQuanHeCuTruId,
    'loaiQuanHeTen': loaiQuanHeTen,
    'ngayBatDau': ngayBatDau?.toIso8601String(),
    'ngayKetThuc': ngayKetThuc?.toIso8601String(),
    'trangThaiCuTruId': trangThaiCuTruId,
    'trangThaiCuTruTen': trangThaiCuTruTen,
    'taiLieuCuTrus': taiLieuCuTrus.map((e) => e.toJson()).toList(),
  };

  ThongTinCuDanModel copyWith({
    int? userId,
    String? fullName,
    String? firstName,
    String? lastName,
    int? gioiTinhId,
    String? gioiTinhName,
    DateTime? dob,
    String? idCard,
    String? phoneNumber,
    String? diaChi,
    String? anhDaiDienUrl,
    int? quanHeCuTruId,
    int? loaiQuanHeCuTruId,
    String? loaiQuanHeTen,
    DateTime? ngayBatDau,
    DateTime? ngayKetThuc,
    int? trangThaiCuTruId,
    String? trangThaiCuTruTen,
    List<TaiLieuCuTruModel>? taiLieuCuTrus,
  }) => ThongTinCuDanModel(
    userId: userId ?? this.userId,
    fullName: fullName ?? this.fullName,
    firstName: firstName ?? this.firstName,
    lastName: lastName ?? this.lastName,
    gioiTinhId: gioiTinhId ?? this.gioiTinhId,
    gioiTinhName: gioiTinhName ?? this.gioiTinhName,
    dob: dob ?? this.dob,
    idCard: idCard ?? this.idCard,
    phoneNumber: phoneNumber ?? this.phoneNumber,
    diaChi: diaChi ?? this.diaChi,
    anhDaiDienUrl: anhDaiDienUrl ?? this.anhDaiDienUrl,
    quanHeCuTruId: quanHeCuTruId ?? this.quanHeCuTruId,
    loaiQuanHeCuTruId: loaiQuanHeCuTruId ?? this.loaiQuanHeCuTruId,
    loaiQuanHeTen: loaiQuanHeTen ?? this.loaiQuanHeTen,
    ngayBatDau: ngayBatDau ?? this.ngayBatDau,
    ngayKetThuc: ngayKetThuc ?? this.ngayKetThuc,
    trangThaiCuTruId: trangThaiCuTruId ?? this.trangThaiCuTruId,
    trangThaiCuTruTen: trangThaiCuTruTen ?? this.trangThaiCuTruTen,
    taiLieuCuTrus: taiLieuCuTrus ?? this.taiLieuCuTrus,
  );
}

class YeuCauCuTruModel {
  final int id;
  final int createdBy;
  final String tenNguoiGui;
  final DateTime? createdAt;
  final int canHoId;
  final String tenCanHo;
  final String tenTang;
  final String tenToaNha;
  final int? nguoiXuLyId;
  final String? tenNguoiXuLy;
  final DateTime? ngayXuLy;
  final int loaiYeuCauId;
  final String tenLoaiYeuCau;
  final int? targetQuanHeCuTruId;
  final String? yeuCauTen;
  final String? yeuCauHo;
  final DateTime? yeuCauNgaySinh;
  final int? yeuCauGioiTinhId;
  final String? yeuCauGioiTinhTen;
  final String? yeuCauSoDienThoai;
  final String? yeuCauCCCD;
  final String? yeuCauDiaChi;
  final int? yeuCauLoaiQuanHeId;
  final String? yeuCauLoaiQuanHeTen;
  final String? noiDung;
  final String? lyDo;
  final int trangThaiId;
  final String tenTrangThai;
  final List<TaiLieuCuTruModel> documents;

  const YeuCauCuTruModel({
    required this.id,
    required this.createdBy,
    required this.tenNguoiGui,
    this.createdAt,
    required this.canHoId,
    required this.tenCanHo,
    required this.tenTang,
    required this.tenToaNha,
    this.nguoiXuLyId,
    this.tenNguoiXuLy,
    this.ngayXuLy,
    required this.loaiYeuCauId,
    required this.tenLoaiYeuCau,
    this.targetQuanHeCuTruId,
    this.yeuCauTen,
    this.yeuCauHo,
    this.yeuCauNgaySinh,
    this.yeuCauGioiTinhId,
    this.yeuCauGioiTinhTen,
    this.yeuCauSoDienThoai,
    this.yeuCauCCCD,
    this.yeuCauDiaChi,
    this.yeuCauLoaiQuanHeId,
    this.yeuCauLoaiQuanHeTen,
    this.noiDung,
    this.lyDo,
    required this.trangThaiId,
    required this.tenTrangThai,
    this.documents = const [],
  });

  String? get hoTenDayDu {
    if (yeuCauHo == null && yeuCauTen == null) return null;
    return '${yeuCauHo ?? ''} ${yeuCauTen ?? ''}'.trim();
  }

  String get diaChiCanHo => '$tenToaNha - $tenTang - $tenCanHo';

  factory YeuCauCuTruModel.fromJson(Map<String, dynamic> json) =>
      YeuCauCuTruModel(
        id: json['id'] as int? ?? 0,
        createdBy: json['createdBy'] as int? ?? 0,
        tenNguoiGui: json['tenNguoiGui'] as String? ?? '',
        createdAt: json['createdAt'] != null
            ? DateTime.tryParse(json['createdAt'] as String)
            : null,
        canHoId: json['canHoId'] as int? ?? 0,
        tenCanHo: json['tenCanHo'] as String? ?? '',
        tenTang: json['tenTang'] as String? ?? '',
        tenToaNha: json['tenToaNha'] as String? ?? '',
        nguoiXuLyId: json['nguoiXuLyId'] as int?,
        tenNguoiXuLy: json['tenNguoiXuLy'] as String?,
        ngayXuLy: json['ngayXuLy'] != null
            ? DateTime.tryParse(json['ngayXuLy'] as String)
            : null,
        loaiYeuCauId: json['loaiYeuCauId'] as int? ?? 0,
        tenLoaiYeuCau: json['tenLoaiYeuCau'] as String? ?? '',
        targetQuanHeCuTruId: json['targetQuanHeCuTruId'] as int?,
        yeuCauTen: json['yeuCauTen'] as String?,
        yeuCauHo: json['yeuCauHo'] as String?,
        yeuCauNgaySinh: json['yeuCauNgaySinh'] != null
            ? DateTime.tryParse(json['yeuCauNgaySinh'] as String)
            : null,
        yeuCauGioiTinhId: json['yeuCauGioiTinhId'] as int?,
        yeuCauGioiTinhTen: json['yeuCauGioiTinhTen'] as String?,
        yeuCauSoDienThoai: json['yeuCauSoDienThoai'] as String?,
        yeuCauCCCD: json['yeuCauCCCD'] as String?,
        yeuCauDiaChi: json['yeuCauDiaChi'] as String?,
        yeuCauLoaiQuanHeId: json['yeuCauLoaiQuanHeId'] as int?,
        yeuCauLoaiQuanHeTen: json['yeuCauLoaiQuanHeTen'] as String?,
        noiDung: json['noiDung'] as String?,
        lyDo: json['lyDo'] as String?,
        trangThaiId: json['trangThaiId'] as int? ?? 0,
        tenTrangThai: json['tenTrangThai'] as String? ?? '',
        documents: (json['documents'] as List<dynamic>? ?? [])
            .map((e) => TaiLieuCuTruModel.fromJson(e as Map<String, dynamic>))
            .toList(),
      );

  Map<String, dynamic> toJson() => {
    'id': id,
    'createdBy': createdBy,
    'tenNguoiGui': tenNguoiGui,
    'createdAt': createdAt?.toIso8601String(),
    'canHoId': canHoId,
    'tenCanHo': tenCanHo,
    'tenTang': tenTang,
    'tenToaNha': tenToaNha,
    'nguoiXuLyId': nguoiXuLyId,
    'tenNguoiXuLy': tenNguoiXuLy,
    'ngayXuLy': ngayXuLy?.toIso8601String(),
    'loaiYeuCauId': loaiYeuCauId,
    'tenLoaiYeuCau': tenLoaiYeuCau,
    'targetQuanHeCuTruId': targetQuanHeCuTruId,
    'yeuCauTen': yeuCauTen,
    'yeuCauHo': yeuCauHo,
    'yeuCauNgaySinh': yeuCauNgaySinh?.toIso8601String(),
    'yeuCauGioiTinhId': yeuCauGioiTinhId,
    'yeuCauGioiTinhTen': yeuCauGioiTinhTen,
    'yeuCauSoDienThoai': yeuCauSoDienThoai,
    'yeuCauCCCD': yeuCauCCCD,
    'yeuCauDiaChi': yeuCauDiaChi,
    'yeuCauLoaiQuanHeId': yeuCauLoaiQuanHeId,
    'yeuCauLoaiQuanHeTen': yeuCauLoaiQuanHeTen,
    'noiDung': noiDung,
    'lyDo': lyDo,
    'trangThaiId': trangThaiId,
    'tenTrangThai': tenTrangThai,
    'documents': documents.map((e) => e.toJson()).toList(),
  };
}

class TaiLieuCuTruRequest {
  final int taiLieuCuTruId;
  final int? loaiGiayToId;
  final String soGiayTo;
  final DateTime? ngayPhatHanh;
  final List<int> fileIds;

  const TaiLieuCuTruRequest({
    this.taiLieuCuTruId = 0,
    this.loaiGiayToId,
    this.soGiayTo = '',
    this.ngayPhatHanh,
    required this.fileIds,
  });

  Map<String, dynamic> toJson() => {
    'soGiayTo': soGiayTo,
    'fileIds': fileIds,
    if (taiLieuCuTruId != 0) 'taiLieuCuTruId': taiLieuCuTruId,
    if (loaiGiayToId != null && loaiGiayToId != 0) 'loaiGiayToId': loaiGiayToId,
    if (ngayPhatHanh != null) 'ngayPhatHanh': ngayPhatHanh!.toIso8601String(),
  };
}

class GetListYeuCauCuTruRequest {
  final int pageNumber;
  final int pageSize;
  final int? toaNhaId;
  final int? tangId;
  final int? canHoId;
  final int? loaiYeuCauId;
  final int? trangThaiId;
  final String? keyword;
  final String sortCol;
  final bool isAsc;

  const GetListYeuCauCuTruRequest({
    required this.pageNumber,
    required this.pageSize,
    this.toaNhaId,
    this.tangId,
    this.canHoId,
    this.loaiYeuCauId,
    this.trangThaiId,
    this.keyword,
    this.sortCol = 'createdAt',
    this.isAsc = false,
  });

  Map<String, dynamic> toJson() => {
    'pageNumber': pageNumber,
    'pageSize': pageSize,
    'isAsc': isAsc,
    'sortCol': sortCol,
    if (toaNhaId != null) 'toaNhaId': toaNhaId,
    if (tangId != null) 'tangId': tangId,
    if (canHoId != null) 'canHoId': canHoId,
    if (loaiYeuCauId != null) 'loaiYeuCauId': loaiYeuCauId,
    if (trangThaiId != null) 'trangThaiId': trangThaiId,
    if (keyword != null) 'keyword': keyword,
  };
}

class TaoYeuCauCuTruRequest {
  final int canHoId;
  final int loaiYeuCauId;
  final bool isSubmit;
  final int? targetQuanHeCuTruId;
  final String? firstName;
  final String? lastName;
  final int? gioiTinhId;
  final DateTime? dob;
  final String? cccd;
  final String? phoneNumber;
  final String? diaChi;
  final int? loaiQuanHeId;
  final String? noiDung;
  final List<TaiLieuCuTruRequest>? taiLieuCuTrus;

  const TaoYeuCauCuTruRequest({
    required this.canHoId,
    required this.loaiYeuCauId,
    this.isSubmit = false,
    this.targetQuanHeCuTruId,
    this.firstName,
    this.lastName,
    this.gioiTinhId,
    this.dob,
    this.cccd,
    this.phoneNumber,
    this.diaChi,
    this.loaiQuanHeId,
    this.noiDung,
    this.taiLieuCuTrus,
  });

  Map<String, dynamic> toJson() {
    final map = <String, dynamic>{
      'canHoId': canHoId,
      'loaiYeuCauId': loaiYeuCauId,
      'isSubmit': isSubmit,
      if (targetQuanHeCuTruId != null)
        'targetQuanHeCuTruId': targetQuanHeCuTruId,
      if (firstName?.isNotEmpty == true) 'firstName': firstName,
      if (lastName?.isNotEmpty == true) 'lastName': lastName,
      if (gioiTinhId != null) 'gioiTinhId': gioiTinhId,
      if (dob != null) 'dob': dob!.toIso8601String(),
      if (cccd?.isNotEmpty == true) 'cccd': cccd,
      if (phoneNumber?.isNotEmpty == true) 'phoneNumber': phoneNumber,
      if (diaChi?.isNotEmpty == true) 'diaChi': diaChi,
      if (loaiQuanHeId != null) 'loaiQuanHeId': loaiQuanHeId,
      if (noiDung?.isNotEmpty == true) 'noiDung': noiDung,
    };
    _attachTaiLieu(map, taiLieuCuTrus);
    return map;
  }
}

class CapNhatYeuCauCuTruRequest {
  final int id;
  final bool isSubmit;
  final bool isWithdraw;
  final String? firstName;
  final String? lastName;
  final String? phoneNumber;
  final DateTime? dob;
  final int? gioiTinhId;
  final String? cccd;
  final String? diaChi;
  final int? loaiQuanHeId;
  final String? noiDung;
  final List<TaiLieuCuTruRequest>? taiLieuCuTrus;

  const CapNhatYeuCauCuTruRequest({
    required this.id,
    this.isSubmit = false,
    this.isWithdraw = false,
    this.firstName,
    this.lastName,
    this.phoneNumber,
    this.dob,
    this.gioiTinhId,
    this.cccd,
    this.diaChi,
    this.loaiQuanHeId,
    this.noiDung,
    this.taiLieuCuTrus,
  });

  Map<String, dynamic> toJson() {
    final map = <String, dynamic>{
      'id': id,
      'isSubmit': isSubmit,
      'isWithdraw': isWithdraw,
      if (firstName?.isNotEmpty == true) 'firstName': firstName,
      if (lastName?.isNotEmpty == true) 'lastName': lastName,
      if (phoneNumber?.isNotEmpty == true) 'phoneNumber': phoneNumber,
      if (dob != null) 'dob': dob!.toIso8601String(),
      if (gioiTinhId != null) 'gioiTinhId': gioiTinhId,
      if (cccd?.isNotEmpty == true) 'cccd': cccd,
      if (diaChi?.isNotEmpty == true) 'diaChi': diaChi,
      if (loaiQuanHeId != null) 'loaiQuanHeId': loaiQuanHeId,
      if (noiDung?.isNotEmpty == true) 'noiDung': noiDung,
    };
    _attachTaiLieu(map, taiLieuCuTrus);
    return map;
  }
}

void _attachTaiLieu(
  Map<String, dynamic> map,
  List<TaiLieuCuTruRequest>? taiLieuCuTrus,
) {
  if (taiLieuCuTrus == null || taiLieuCuTrus.isEmpty) return;
  final valid = taiLieuCuTrus
      .where((t) => t.fileIds.isNotEmpty)
      .map((t) => t.toJson())
      .toList();
  if (valid.isNotEmpty) map['taiLieuCuTrus'] = valid;
}
