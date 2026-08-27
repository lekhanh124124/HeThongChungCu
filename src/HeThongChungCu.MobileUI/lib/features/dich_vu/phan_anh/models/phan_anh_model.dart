export 'package:klks_app/features/shared/models/paging_model.dart';
export 'package:klks_app/features/shared/models/file_model.dart';
export '../../../cu_tru/quan_he/models/quan_he_cu_tru_model.dart'
    show QuanHeCuTruModel;

import 'package:klks_app/features/shared/models/file_model.dart';

class PhanAnhResponse {
  final int id;
  final int canHoId;
  final String tenCanHo;
  final String tieuDe;
  final int loaiPhanAnhId;
  final String loaiPhanAnhTen;
  final int trangThaiPhanAnhId;
  final String trangThaiPhanAnhTen;
  final int? nguoiXuLyId;
  final String? tenNguoiXuLy;
  final DateTime createdAt;
  final int createdBy;
  final String tenNguoiGui;

  const PhanAnhResponse({
    required this.id,
    required this.canHoId,
    required this.tenCanHo,
    required this.tieuDe,
    required this.loaiPhanAnhId,
    required this.loaiPhanAnhTen,
    required this.trangThaiPhanAnhId,
    required this.trangThaiPhanAnhTen,
    this.nguoiXuLyId,
    this.tenNguoiXuLy,
    required this.createdAt,
    required this.createdBy,
    required this.tenNguoiGui,
  });

  String get statusLabel => trangThaiPhanAnhTen;

  factory PhanAnhResponse.fromJson(Map<String, dynamic> json) =>
      PhanAnhResponse(
        id: json['id'] as int,
        canHoId: json['canHoId'] as int,
        tenCanHo: json['tenCanHo'] as String? ?? '',
        tieuDe: json['tieuDe'] as String? ?? '',
        loaiPhanAnhId: json['loaiPhanAnhId'] as int,
        loaiPhanAnhTen: json['loaiPhanAnhTen'] as String? ?? '',
        trangThaiPhanAnhId: json['trangThaiPhanAnhId'] as int,
        trangThaiPhanAnhTen: json['trangThaiPhanAnhTen'] as String? ?? '',
        nguoiXuLyId: json['nguoiXuLyId'] as int?,
        tenNguoiXuLy: json['tenNguoiXuLy'] as String?,
        createdAt: DateTime.parse(json['createdAt'] as String),
        createdBy: json['createdBy'] as int,
        tenNguoiGui: json['tenNguoiGui'] as String? ?? '',
      );

  Map<String, dynamic> toJson() => {
    'id': id,
    'canHoId': canHoId,
    'tenCanHo': tenCanHo,
    'tieuDe': tieuDe,
    'loaiPhanAnhId': loaiPhanAnhId,
    'loaiPhanAnhTen': loaiPhanAnhTen,
    'trangThaiPhanAnhId': trangThaiPhanAnhId,
    'trangThaiPhanAnhTen': trangThaiPhanAnhTen,
    'nguoiXuLyId': nguoiXuLyId,
    'tenNguoiXuLy': tenNguoiXuLy,
    'createdAt': createdAt.toIso8601String(),
    'createdBy': createdBy,
    'tenNguoiGui': tenNguoiGui,
  };

  PhanAnhResponse copyWith({
    int? id,
    int? canHoId,
    String? tenCanHo,
    String? tieuDe,
    int? loaiPhanAnhId,
    String? loaiPhanAnhTen,
    int? trangThaiPhanAnhId,
    String? trangThaiPhanAnhTen,
    int? nguoiXuLyId,
    String? tenNguoiXuLy,
    DateTime? createdAt,
    int? createdBy,
    String? tenNguoiGui,
  }) => PhanAnhResponse(
    id: id ?? this.id,
    canHoId: canHoId ?? this.canHoId,
    tenCanHo: tenCanHo ?? this.tenCanHo,
    tieuDe: tieuDe ?? this.tieuDe,
    loaiPhanAnhId: loaiPhanAnhId ?? this.loaiPhanAnhId,
    loaiPhanAnhTen: loaiPhanAnhTen ?? this.loaiPhanAnhTen,
    trangThaiPhanAnhId: trangThaiPhanAnhId ?? this.trangThaiPhanAnhId,
    trangThaiPhanAnhTen: trangThaiPhanAnhTen ?? this.trangThaiPhanAnhTen,
    nguoiXuLyId: nguoiXuLyId ?? this.nguoiXuLyId,
    tenNguoiXuLy: tenNguoiXuLy ?? this.tenNguoiXuLy,
    createdAt: createdAt ?? this.createdAt,
    createdBy: createdBy ?? this.createdBy,
    tenNguoiGui: tenNguoiGui ?? this.tenNguoiGui,
  );
}

class TraLoiPhanAnh {
  final int id;
  final String noiDung;
  final bool isNhanVien;
  final int createdBy;
  final String tenNguoiGui;
  final DateTime createdAt;

  const TraLoiPhanAnh({
    required this.id,
    required this.noiDung,
    required this.isNhanVien,
    required this.createdBy,
    required this.tenNguoiGui,
    required this.createdAt,
  });

  factory TraLoiPhanAnh.fromJson(Map<String, dynamic> json) => TraLoiPhanAnh(
    id: json['id'] as int,
    noiDung: json['noiDung'] as String? ?? '',
    isNhanVien: json['isNhanVien'] as bool? ?? false,
    createdBy: json['createdBy'] as int,
    tenNguoiGui: json['tenNguoiGui'] as String? ?? '',
    createdAt: DateTime.parse(json['createdAt'] as String),
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'noiDung': noiDung,
    'isNhanVien': isNhanVien,
    'createdBy': createdBy,
    'tenNguoiGui': tenNguoiGui,
    'createdAt': createdAt.toIso8601String(),
  };
}

class PhanAnhDetailResponse extends PhanAnhResponse {
  final String? noiDung;
  final int? diemDanhGia;
  final String? nhanXetDanhGia;
  final DateTime? ngayDanhGia;
  final List<TraLoiPhanAnh> traLoiPhanAnhs;

  final List<FileAttachment> danhSachTep;

  const PhanAnhDetailResponse({
    required super.id,
    required super.canHoId,
    required super.tenCanHo,
    required super.tieuDe,
    required super.loaiPhanAnhId,
    required super.loaiPhanAnhTen,
    required super.trangThaiPhanAnhId,
    required super.trangThaiPhanAnhTen,
    super.nguoiXuLyId,
    super.tenNguoiXuLy,
    required super.createdAt,
    required super.createdBy,
    required super.tenNguoiGui,
    this.noiDung,
    this.diemDanhGia,
    this.nhanXetDanhGia,
    this.ngayDanhGia,
    required this.traLoiPhanAnhs,
    required this.danhSachTep,
  });

  factory PhanAnhDetailResponse.fromJson(Map<String, dynamic> json) =>
      PhanAnhDetailResponse(
        id: json['id'] as int,
        canHoId: json['canHoId'] as int,
        tenCanHo: json['tenCanHo'] as String? ?? '',
        tieuDe: json['tieuDe'] as String? ?? '',
        loaiPhanAnhId: json['loaiPhanAnhId'] as int,
        loaiPhanAnhTen: json['loaiPhanAnhTen'] as String? ?? '',
        trangThaiPhanAnhId: json['trangThaiPhanAnhId'] as int,
        trangThaiPhanAnhTen: json['trangThaiPhanAnhTen'] as String? ?? '',
        nguoiXuLyId: json['nguoiXuLyId'] as int?,
        tenNguoiXuLy: json['tenNguoiXuLy'] as String?,
        createdAt: DateTime.parse(json['createdAt'] as String),
        createdBy: json['createdBy'] as int,
        tenNguoiGui: json['tenNguoiGui'] as String? ?? '',
        noiDung: json['noiDung'] as String?,
        diemDanhGia: json['diemDanhGia'] as int?,
        nhanXetDanhGia: json['nhanXetDanhGia'] as String?,
        ngayDanhGia: json['ngayDanhGia'] != null
            ? DateTime.parse(json['ngayDanhGia'] as String)
            : null,
        traLoiPhanAnhs: (json['traLoiPhanAnhs'] as List<dynamic>? ?? [])
            .map((e) => TraLoiPhanAnh.fromJson(e as Map<String, dynamic>))
            .toList(),
        danhSachTep: (json['danhSachTep'] as List<dynamic>? ?? [])
            .map((e) => FileAttachment.fromJson(e as Map<String, dynamic>))
            .toList(),
      );
}
