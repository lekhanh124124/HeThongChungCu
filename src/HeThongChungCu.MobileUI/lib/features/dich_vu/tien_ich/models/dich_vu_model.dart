export 'package:klks_app/features/shared/models/paging_model.dart';
export 'package:klks_app/features/shared/models/selector_item_model.dart';
export 'package:klks_app/features/cu_tru/quan_he/models/quan_he_cu_tru_model.dart'
    show QuanHeCuTruModel;

class DichVuItem {
  final int id;
  final String maDichVu;
  final String tenDichVu;
  final int loaiDichVuId;
  final String loaiDichVuTen;
  final String donViTinh;
  final String? moTa;
  final bool isBatBuoc;
  final int? soLuongToiDa;
  final int trangThaiDichVuId;
  final String trangThaiDichVuTen;
  final String? iconUrl;

  const DichVuItem({
    required this.id,
    required this.maDichVu,
    required this.tenDichVu,
    required this.loaiDichVuId,
    required this.loaiDichVuTen,
    required this.donViTinh,
    this.moTa,
    required this.isBatBuoc,
    this.soLuongToiDa,
    required this.trangThaiDichVuId,
    required this.trangThaiDichVuTen,
    this.iconUrl,
  });

  bool get isHoatDong => trangThaiDichVuId == 1;
  String get displayName => '$maDichVu - $tenDichVu';

  factory DichVuItem.fromJson(Map<String, dynamic> json) => DichVuItem(
    id: json['id'] as int? ?? 0,
    maDichVu: json['maDichVu'] as String? ?? '',
    tenDichVu: json['tenDichVu'] as String? ?? '',
    loaiDichVuId: json['loaiDichVuId'] as int? ?? 0,
    loaiDichVuTen: json['loaiDichVuTen'] as String? ?? '',
    donViTinh: json['donViTinh'] as String? ?? '',
    moTa: json['moTa'] as String?,
    isBatBuoc: json['isBatBuoc'] as bool? ?? false,
    soLuongToiDa: json['soLuongToiDa'] as int?,
    trangThaiDichVuId: json['trangThaiDichVuId'] as int? ?? 0,
    trangThaiDichVuTen: json['trangThaiDichVuTen'] as String? ?? '',
    iconUrl: json['iconUrl'] as String?,
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'maDichVu': maDichVu,
    'tenDichVu': tenDichVu,
    'loaiDichVuId': loaiDichVuId,
    'loaiDichVuTen': loaiDichVuTen,
    'donViTinh': donViTinh,
    'moTa': moTa,
    'isBatBuoc': isBatBuoc,
    'soLuongToiDa': soLuongToiDa,
    'trangThaiDichVuId': trangThaiDichVuId,
    'trangThaiDichVuTen': trangThaiDichVuTen,
    'iconUrl': iconUrl,
  };

  DichVuItem copyWith({
    int? id,
    String? maDichVu,
    String? tenDichVu,
    int? loaiDichVuId,
    String? loaiDichVuTen,
    String? donViTinh,
    String? moTa,
    bool? isBatBuoc,
    int? soLuongToiDa,
    int? trangThaiDichVuId,
    String? trangThaiDichVuTen,
    String? iconUrl,
  }) => DichVuItem(
    id: id ?? this.id,
    maDichVu: maDichVu ?? this.maDichVu,
    tenDichVu: tenDichVu ?? this.tenDichVu,
    loaiDichVuId: loaiDichVuId ?? this.loaiDichVuId,
    loaiDichVuTen: loaiDichVuTen ?? this.loaiDichVuTen,
    donViTinh: donViTinh ?? this.donViTinh,
    moTa: moTa ?? this.moTa,
    isBatBuoc: isBatBuoc ?? this.isBatBuoc,
    soLuongToiDa: soLuongToiDa ?? this.soLuongToiDa,
    trangThaiDichVuId: trangThaiDichVuId ?? this.trangThaiDichVuId,
    trangThaiDichVuTen: trangThaiDichVuTen ?? this.trangThaiDichVuTen,
    iconUrl: iconUrl ?? this.iconUrl,
  );
}

class KhungGioItem {
  final int id;
  final int dichVuId;
  final String gioBatDau;
  final String gioKetThuc;
  final String tenKhungGio;
  final int ngayTrongTuan;
  final bool isActive;

  const KhungGioItem({
    required this.id,
    required this.dichVuId,
    required this.gioBatDau,
    required this.gioKetThuc,
    required this.tenKhungGio,
    required this.ngayTrongTuan,
    required this.isActive,
  });

  String get thoiGian => '$gioBatDau - $gioKetThuc';

  factory KhungGioItem.fromJson(Map<String, dynamic> json) => KhungGioItem(
    id: json['id'] as int? ?? 0,
    dichVuId: json['dichVuId'] as int? ?? 0,
    gioBatDau: json['gioBatDau'] as String? ?? '',
    gioKetThuc: json['gioKetThuc'] as String? ?? '',
    tenKhungGio: json['tenKhungGio'] as String? ?? '',
    ngayTrongTuan: json['ngayTrongTuan'] as int? ?? 0,
    isActive: json['isActive'] as bool? ?? false,
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'dichVuId': dichVuId,
    'gioBatDau': gioBatDau,
    'gioKetThuc': gioKetThuc,
    'tenKhungGio': tenKhungGio,
    'ngayTrongTuan': ngayTrongTuan,
    'isActive': isActive,
  };

  KhungGioItem copyWith({
    int? id,
    int? dichVuId,
    String? gioBatDau,
    String? gioKetThuc,
    String? tenKhungGio,
    int? ngayTrongTuan,
    bool? isActive,
  }) => KhungGioItem(
    id: id ?? this.id,
    dichVuId: dichVuId ?? this.dichVuId,
    gioBatDau: gioBatDau ?? this.gioBatDau,
    gioKetThuc: gioKetThuc ?? this.gioKetThuc,
    tenKhungGio: tenKhungGio ?? this.tenKhungGio,
    ngayTrongTuan: ngayTrongTuan ?? this.ngayTrongTuan,
    isActive: isActive ?? this.isActive,
  );
}

class GiaLuyTien {
  final int id;
  final int bangGiaId;
  final double tuMuc;
  final double? denMuc;
  final double donGia;

  const GiaLuyTien({
    required this.id,
    required this.bangGiaId,
    required this.tuMuc,
    this.denMuc,
    required this.donGia,
  });

  factory GiaLuyTien.fromJson(Map<String, dynamic> json) => GiaLuyTien(
    id: json['id'] as int,
    bangGiaId: json['bangGiaId'] as int,
    tuMuc: (json['tuMuc'] as num).toDouble(),
    denMuc: (json['denMuc'] as num?)?.toDouble(),
    donGia: (json['donGia'] as num).toDouble(),
  );
}

class GiaKhungGio {
  final int id;
  final int bangGiaId;
  final int khungGioId;
  final String tenKhungGio;
  final double donGia;

  const GiaKhungGio({
    required this.id,
    required this.bangGiaId,
    required this.khungGioId,
    required this.tenKhungGio,
    required this.donGia,
  });

  factory GiaKhungGio.fromJson(Map<String, dynamic> json) => GiaKhungGio(
    id: json['id'] as int,
    bangGiaId: json['bangGiaId'] as int,
    khungGioId: json['khungGioId'] as int,
    tenKhungGio: json['tenKhungGio'] as String? ?? '',
    donGia: (json['donGia'] as num).toDouble(),
  );
}

class GiaLoaiCanHo {
  final int id;
  final int bangGiaId;
  final int loaiCanHoId;
  final String loaiCanHoTen;
  final double donGia;

  const GiaLoaiCanHo({
    required this.id,
    required this.bangGiaId,
    required this.loaiCanHoId,
    required this.loaiCanHoTen,
    required this.donGia,
  });

  factory GiaLoaiCanHo.fromJson(Map<String, dynamic> json) => GiaLoaiCanHo(
    id: json['id'] as int,
    bangGiaId: json['bangGiaId'] as int,
    loaiCanHoId: json['loaiCanHoId'] as int,
    loaiCanHoTen: json['loaiCanHoTen'] as String? ?? '',
    donGia: (json['donGia'] as num).toDouble(),
  );
}

class BangGia {
  final int id;
  final int dichVuId;
  final String tenBangGia;
  final DateTime? ngayApDung;
  final DateTime? ngayKetThuc;
  final int loaiDinhGiaId;
  final String loaiDinhGiaTen;
  final double donGia;
  final bool isActive;
  final List<GiaLuyTien> giaLuyTiens;
  final List<GiaKhungGio> giaKhungGios;
  final List<GiaLoaiCanHo> giaLoaiCanHos;

  const BangGia({
    required this.id,
    required this.dichVuId,
    required this.tenBangGia,
    this.ngayApDung,
    this.ngayKetThuc,
    required this.loaiDinhGiaId,
    required this.loaiDinhGiaTen,
    required this.donGia,
    required this.isActive,
    required this.giaLuyTiens,
    required this.giaKhungGios,
    required this.giaLoaiCanHos,
  });

  factory BangGia.fromJson(Map<String, dynamic> json) => BangGia(
    id: json['id'] as int,
    dichVuId: json['dichVuId'] as int,
    tenBangGia: json['tenBangGia'] as String? ?? '',
    ngayApDung: json['ngayApDung'] != null
        ? DateTime.tryParse(json['ngayApDung'] as String)
        : null,
    ngayKetThuc: json['ngayKetThuc'] != null
        ? DateTime.tryParse(json['ngayKetThuc'] as String)
        : null,
    loaiDinhGiaId: json['loaiDinhGiaId'] as int? ?? 0,
    loaiDinhGiaTen: json['loaiDinhGiaTen'] as String? ?? '',
    donGia: (json['donGia'] as num?)?.toDouble() ?? 0,
    isActive: json['isActive'] as bool? ?? false,
    giaLuyTiens: (json['giaLuyTiens'] as List<dynamic>? ?? [])
        .map((e) => GiaLuyTien.fromJson(e as Map<String, dynamic>))
        .toList(),
    giaKhungGios: (json['giaKhungGios'] as List<dynamic>? ?? [])
        .map((e) => GiaKhungGio.fromJson(e as Map<String, dynamic>))
        .toList(),
    giaLoaiCanHos: (json['giaLoaiCanHos'] as List<dynamic>? ?? [])
        .map((e) => GiaLoaiCanHo.fromJson(e as Map<String, dynamic>))
        .toList(),
  );
}

class DichVuDetail extends DichVuItem {
  final List<KhungGioItem> khungGioDichVu;
  final BangGia? bangGia;

  const DichVuDetail({
    required super.id,
    required super.maDichVu,
    required super.tenDichVu,
    required super.loaiDichVuId,
    required super.loaiDichVuTen,
    required super.donViTinh,
    super.moTa,
    required super.isBatBuoc,
    super.soLuongToiDa,
    required super.trangThaiDichVuId,
    required super.trangThaiDichVuTen,
    super.iconUrl,
    required this.khungGioDichVu,
    this.bangGia,
  });

  factory DichVuDetail.fromJson(Map<String, dynamic> json) => DichVuDetail(
    id: json['id'] as int? ?? 0,
    maDichVu: json['maDichVu'] as String? ?? '',
    tenDichVu: json['tenDichVu'] as String? ?? '',
    loaiDichVuId: json['loaiDichVuId'] as int? ?? 0,
    loaiDichVuTen: json['loaiDichVuTen'] as String? ?? '',
    donViTinh: json['donViTinh'] as String? ?? '',
    moTa: json['moTa'] as String?,
    isBatBuoc: json['isBatBuoc'] as bool? ?? false,
    soLuongToiDa: json['soLuongToiDa'] as int?,
    trangThaiDichVuId: json['trangThaiDichVuId'] as int? ?? 0,
    trangThaiDichVuTen: json['trangThaiDichVuTen'] as String? ?? '',
    iconUrl: json['iconUrl'] as String?,
    khungGioDichVu: (json['khungGioDichVu'] as List<dynamic>? ?? [])
        .map((e) => KhungGioItem.fromJson(e as Map<String, dynamic>))
        .toList(),
    bangGia: json['bangGia'] != null
        ? BangGia.fromJson(json['bangGia'] as Map<String, dynamic>)
        : null,
  );
}

class DichVuDangKyItem {
  final int id;
  final int canHoId;
  final int dichVuId;
  final String maDichVu;
  final String tenDichVu;
  final int loaiDichVuId;
  final String loaiDichVuTen;
  final int soLuong;
  final DateTime? ngayBatDau;
  final DateTime? ngayKetThuc;
  final int trangThaiDangKyId;
  final String trangThaiDangKyTen;

  const DichVuDangKyItem({
    required this.id,
    required this.canHoId,
    required this.dichVuId,
    required this.maDichVu,
    required this.tenDichVu,
    required this.loaiDichVuId,
    required this.loaiDichVuTen,
    required this.soLuong,
    this.ngayBatDau,
    this.ngayKetThuc,
    required this.trangThaiDangKyId,
    required this.trangThaiDangKyTen,
  });

  bool get isActive {
    if (ngayKetThuc == null) return true;
    return ngayKetThuc!.isAfter(DateTime.now());
  }

  String get thoiGianHienThi {
    String fmt(DateTime? d) => d == null
        ? 'N/A'
        : '${d.day.toString().padLeft(2, '0')}/${d.month.toString().padLeft(2, '0')}/${d.year}';
    return '${fmt(ngayBatDau)} → ${fmt(ngayKetThuc)}';
  }

  factory DichVuDangKyItem.fromJson(Map<String, dynamic> json) =>
      DichVuDangKyItem(
        id: json['id'] as int? ?? 0,
        canHoId: json['canHoId'] as int? ?? 0,
        dichVuId: json['dichVuId'] as int? ?? 0,
        maDichVu: json['maDichVu'] as String? ?? '',
        tenDichVu: json['tenDichVu'] as String? ?? '',
        loaiDichVuId: json['loaiDichVuId'] as int? ?? 0,
        loaiDichVuTen: json['loaiDichVuTen'] as String? ?? '',
        soLuong: json['soLuong'] as int? ?? 0,
        ngayBatDau: json['ngayBatDau'] != null
            ? DateTime.tryParse(json['ngayBatDau'] as String)
            : null,
        ngayKetThuc: json['ngayKetThuc'] != null
            ? DateTime.tryParse(json['ngayKetThuc'] as String)
            : null,
        trangThaiDangKyId: json['trangThaiDangKyId'] as int? ?? 0,
        trangThaiDangKyTen: json['trangThaiDangKyTen'] as String? ?? '',
      );

  Map<String, dynamic> toJson() => {
    'id': id,
    'canHoId': canHoId,
    'dichVuId': dichVuId,
    'maDichVu': maDichVu,
    'tenDichVu': tenDichVu,
    'loaiDichVuId': loaiDichVuId,
    'loaiDichVuTen': loaiDichVuTen,
    'soLuong': soLuong,
    'ngayBatDau': ngayBatDau?.toIso8601String(),
    'ngayKetThuc': ngayKetThuc?.toIso8601String(),
    'trangThaiDangKyId': trangThaiDangKyId,
    'trangThaiDangKyTen': trangThaiDangKyTen,
  };

  DichVuDangKyItem copyWith({
    int? id,
    int? canHoId,
    int? dichVuId,
    String? maDichVu,
    String? tenDichVu,
    int? loaiDichVuId,
    String? loaiDichVuTen,
    int? soLuong,
    DateTime? ngayBatDau,
    DateTime? ngayKetThuc,
    int? trangThaiDangKyId,
    String? trangThaiDangKyTen,
  }) => DichVuDangKyItem(
    id: id ?? this.id,
    canHoId: canHoId ?? this.canHoId,
    dichVuId: dichVuId ?? this.dichVuId,
    maDichVu: maDichVu ?? this.maDichVu,
    tenDichVu: tenDichVu ?? this.tenDichVu,
    loaiDichVuId: loaiDichVuId ?? this.loaiDichVuId,
    loaiDichVuTen: loaiDichVuTen ?? this.loaiDichVuTen,
    soLuong: soLuong ?? this.soLuong,
    ngayBatDau: ngayBatDau ?? this.ngayBatDau,
    ngayKetThuc: ngayKetThuc ?? this.ngayKetThuc,
    trangThaiDangKyId: trangThaiDangKyId ?? this.trangThaiDangKyId,
    trangThaiDangKyTen: trangThaiDangKyTen ?? this.trangThaiDangKyTen,
  );
}

class DichVuDangKyRequest {
  final int? loaiDichVuId;
  final int? dichVuId;
  final int? trangThaiDangKyId;
  final DateTime? tuNgay;
  final DateTime? denNgay;
  final String? keyword;
  final int pageNumber;
  final int pageSize;
  final String sortCol;
  final bool isAsc;

  const DichVuDangKyRequest({
    this.loaiDichVuId,
    this.dichVuId,
    this.trangThaiDangKyId,
    this.tuNgay,
    this.denNgay,
    this.keyword,
    this.pageNumber = 1,
    this.pageSize = 20,
    this.sortCol = 'id',
    this.isAsc = false,
  });

  factory DichVuDangKyRequest.tienIch({
    int pageNumber = 1,
    int pageSize = 20,
  }) => DichVuDangKyRequest(
    loaiDichVuId: 3,
    pageNumber: pageNumber,
    pageSize: pageSize,
  );

  Map<String, dynamic> toJson() => {
    if (loaiDichVuId != null) 'loaiDichVuId': loaiDichVuId,
    if (dichVuId != null) 'dichVuId': dichVuId,
    if (trangThaiDangKyId != null) 'trangThaiDangKyId': trangThaiDangKyId,
    if (tuNgay != null) 'tuNgay': tuNgay!.toIso8601String(),
    if (denNgay != null) 'denNgay': denNgay!.toIso8601String(),
    if (keyword?.isNotEmpty == true) 'keyword': keyword,
    'pageNumber': pageNumber,
    'pageSize': pageSize,
    'sortCol': sortCol,
    'isAsc': isAsc,
  };

  DichVuDangKyRequest copyWith({
    int? loaiDichVuId,
    int? dichVuId,
    int? trangThaiDangKyId,
    DateTime? tuNgay,
    DateTime? denNgay,
    String? keyword,
    int? pageNumber,
    int? pageSize,
    String? sortCol,
    bool? isAsc,
  }) => DichVuDangKyRequest(
    loaiDichVuId: loaiDichVuId ?? this.loaiDichVuId,
    dichVuId: dichVuId ?? this.dichVuId,
    trangThaiDangKyId: trangThaiDangKyId ?? this.trangThaiDangKyId,
    tuNgay: tuNgay ?? this.tuNgay,
    denNgay: denNgay ?? this.denNgay,
    keyword: keyword ?? this.keyword,
    pageNumber: pageNumber ?? this.pageNumber,
    pageSize: pageSize ?? this.pageSize,
    sortCol: sortCol ?? this.sortCol,
    isAsc: isAsc ?? this.isAsc,
  );
}
