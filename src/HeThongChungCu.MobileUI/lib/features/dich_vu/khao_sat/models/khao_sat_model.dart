export 'package:klks_app/features/cu_tru/quan_he/models/quan_he_cu_tru_model.dart';

class KhaoSatTrangThai {
  static const int draft = 1;
  static const int published = 2;
  static const int paused = 3;
  static const int closed = 4;
}

class KhaoSatResponse {
  final int id;
  final String tieuDe;
  final String moTa;
  final int loaiKhaoSatId;
  final String loaiKhaoSatTen;
  final int coCheTinhDiemId;
  final String coCheTinhDiemTen;
  final int trangThaiId;
  final String trangThaiTen;
  final DateTime ngayBatDau;
  final DateTime ngayKetThuc;
  final double tyleThamGiaToiThieu;
  final double tyLeDongYToiThieu;
  final bool isAnDanh;
  final bool isVoted;
  final DateTime createdAt;

  const KhaoSatResponse({
    required this.id,
    required this.tieuDe,
    required this.moTa,
    required this.loaiKhaoSatId,
    required this.loaiKhaoSatTen,
    required this.coCheTinhDiemId,
    required this.coCheTinhDiemTen,
    required this.trangThaiId,
    required this.trangThaiTen,
    required this.ngayBatDau,
    required this.ngayKetThuc,
    required this.tyleThamGiaToiThieu,
    required this.tyLeDongYToiThieu,
    required this.isAnDanh,
    required this.isVoted,
    required this.createdAt,
  });

  bool get canVote => trangThaiId == KhaoSatTrangThai.published && !isVoted;

  bool get isDraft => trangThaiId == KhaoSatTrangThai.draft;
  bool get isPublished => trangThaiId == KhaoSatTrangThai.published;
  bool get isClosed => trangThaiId == KhaoSatTrangThai.closed;

  factory KhaoSatResponse.fromJson(
    Map<String, dynamic> json,
  ) => KhaoSatResponse(
    id: json['id'] as int? ?? 0,
    tieuDe: json['tieuDe'] as String? ?? '',
    moTa: json['moTa'] as String? ?? '',
    loaiKhaoSatId: json['loaiKhaoSatId'] as int? ?? 0,
    loaiKhaoSatTen: json['loaiKhaoSatTen'] as String? ?? '',
    coCheTinhDiemId: json['coCheTinhDiemId'] as int? ?? 0,
    coCheTinhDiemTen: json['coCheTinhDiemTen'] as String? ?? '',
    trangThaiId: json['trangThaiId'] as int? ?? 0,
    trangThaiTen: json['trangThaiTen'] as String? ?? '',
    ngayBatDau:
        DateTime.tryParse(json['ngayBatDau'] as String? ?? '') ??
        DateTime.now(),
    ngayKetThuc:
        DateTime.tryParse(json['ngayKetThuc'] as String? ?? '') ??
        DateTime.now(),
    tyleThamGiaToiThieu: (json['tyleThamGiaToiThieu'] as num?)?.toDouble() ?? 0,
    tyLeDongYToiThieu: (json['tyLeDongYToiThieu'] as num?)?.toDouble() ?? 0,
    isAnDanh: json['isAnDanh'] as bool? ?? false,
    isVoted: json['isVoted'] as bool? ?? false,
    createdAt:
        DateTime.tryParse(json['createdAt'] as String? ?? '') ?? DateTime.now(),
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'tieuDe': tieuDe,
    'moTa': moTa,
    'loaiKhaoSatId': loaiKhaoSatId,
    'loaiKhaoSatTen': loaiKhaoSatTen,
    'coCheTinhDiemId': coCheTinhDiemId,
    'coCheTinhDiemTen': coCheTinhDiemTen,
    'trangThaiId': trangThaiId,
    'trangThaiTen': trangThaiTen,
    'ngayBatDau': ngayBatDau.toIso8601String(),
    'ngayKetThuc': ngayKetThuc.toIso8601String(),
    'tyleThamGiaToiThieu': tyleThamGiaToiThieu,
    'tyLeDongYToiThieu': tyLeDongYToiThieu,
    'isAnDanh': isAnDanh,
    'isVoted': isVoted,
    'createdAt': createdAt.toIso8601String(),
  };

  KhaoSatResponse copyWith({
    int? id,
    String? tieuDe,
    String? moTa,
    int? loaiKhaoSatId,
    String? loaiKhaoSatTen,
    int? coCheTinhDiemId,
    String? coCheTinhDiemTen,
    int? trangThaiId,
    String? trangThaiTen,
    DateTime? ngayBatDau,
    DateTime? ngayKetThuc,
    double? tyleThamGiaToiThieu,
    double? tyLeDongYToiThieu,
    bool? isAnDanh,
    bool? isVoted,
    DateTime? createdAt,
  }) => KhaoSatResponse(
    id: id ?? this.id,
    tieuDe: tieuDe ?? this.tieuDe,
    moTa: moTa ?? this.moTa,
    loaiKhaoSatId: loaiKhaoSatId ?? this.loaiKhaoSatId,
    loaiKhaoSatTen: loaiKhaoSatTen ?? this.loaiKhaoSatTen,
    coCheTinhDiemId: coCheTinhDiemId ?? this.coCheTinhDiemId,
    coCheTinhDiemTen: coCheTinhDiemTen ?? this.coCheTinhDiemTen,
    trangThaiId: trangThaiId ?? this.trangThaiId,
    trangThaiTen: trangThaiTen ?? this.trangThaiTen,
    ngayBatDau: ngayBatDau ?? this.ngayBatDau,
    ngayKetThuc: ngayKetThuc ?? this.ngayKetThuc,
    tyleThamGiaToiThieu: tyleThamGiaToiThieu ?? this.tyleThamGiaToiThieu,
    tyLeDongYToiThieu: tyLeDongYToiThieu ?? this.tyLeDongYToiThieu,
    isAnDanh: isAnDanh ?? this.isAnDanh,
    isVoted: isVoted ?? this.isVoted,
    createdAt: createdAt ?? this.createdAt,
  );
}

class LuaChonModel {
  final int id;
  final String noiDungLuaChon;
  final bool isUngVienBQT;
  final String? tieuSuUngVien;
  final int? ungVienId;

  const LuaChonModel({
    required this.id,
    required this.noiDungLuaChon,
    required this.isUngVienBQT,
    this.tieuSuUngVien,
    this.ungVienId,
  });

  factory LuaChonModel.fromJson(Map<String, dynamic> json) => LuaChonModel(
    id: json['id'] as int? ?? 0,
    noiDungLuaChon: json['noiDungLuaChon'] as String? ?? '',
    isUngVienBQT: json['isUngVienBQT'] as bool? ?? false,
    tieuSuUngVien: json['tieuSuUngVien'] as String?,
    ungVienId: json['ungVienId'] as int?,
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'noiDungLuaChon': noiDungLuaChon,
    'isUngVienBQT': isUngVienBQT,
    'tieuSuUngVien': tieuSuUngVien,
    'ungVienId': ungVienId,
  };

  LuaChonModel copyWith({
    int? id,
    String? noiDungLuaChon,
    bool? isUngVienBQT,
    String? tieuSuUngVien,
    int? ungVienId,
  }) => LuaChonModel(
    id: id ?? this.id,
    noiDungLuaChon: noiDungLuaChon ?? this.noiDungLuaChon,
    isUngVienBQT: isUngVienBQT ?? this.isUngVienBQT,
    tieuSuUngVien: tieuSuUngVien ?? this.tieuSuUngVien,
    ungVienId: ungVienId ?? this.ungVienId,
  );
}

class CauHoiModel {
  final int id;
  final String noiDungCauHoi;
  final bool isBatBuoc;
  final bool isMultiSelect;
  final List<LuaChonModel> luaChons;

  const CauHoiModel({
    required this.id,
    required this.noiDungCauHoi,
    required this.isBatBuoc,
    required this.isMultiSelect,
    required this.luaChons,
  });

  factory CauHoiModel.fromJson(Map<String, dynamic> json) => CauHoiModel(
    id: json['id'] as int? ?? 0,
    noiDungCauHoi: json['noiDungCauHoi'] as String? ?? '',
    isBatBuoc: json['isBatBuoc'] as bool? ?? false,
    isMultiSelect: json['isMultiSelect'] as bool? ?? false,
    luaChons: (json['luaChons'] as List<dynamic>? ?? [])
        .map((e) => LuaChonModel.fromJson(e as Map<String, dynamic>))
        .toList(),
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'noiDungCauHoi': noiDungCauHoi,
    'isBatBuoc': isBatBuoc,
    'isMultiSelect': isMultiSelect,
    'luaChons': luaChons.map((e) => e.toJson()).toList(),
  };

  CauHoiModel copyWith({
    int? id,
    String? noiDungCauHoi,
    bool? isBatBuoc,
    bool? isMultiSelect,
    List<LuaChonModel>? luaChons,
  }) => CauHoiModel(
    id: id ?? this.id,
    noiDungCauHoi: noiDungCauHoi ?? this.noiDungCauHoi,
    isBatBuoc: isBatBuoc ?? this.isBatBuoc,
    isMultiSelect: isMultiSelect ?? this.isMultiSelect,
    luaChons: luaChons ?? this.luaChons,
  );
}

class KhaoSatDetailResponse {
  final int id;
  final String tieuDe;
  final String moTa;
  final int loaiKhaoSatId;
  final String loaiKhaoSatTen;
  final int coCheTinhDiemId;
  final String coCheTinhDiemTen;
  final int trangThaiId;
  final String trangThaiTen;
  final DateTime ngayBatDau;
  final DateTime ngayKetThuc;
  final double tyleThamGiaToiThieu;
  final double tyLeDongYToiThieu;
  final bool isAnDanh;
  final bool isVoted;
  final DateTime createdAt;
  final List<CauHoiModel> cauHois;

  const KhaoSatDetailResponse({
    required this.id,
    required this.tieuDe,
    required this.moTa,
    required this.loaiKhaoSatId,
    required this.loaiKhaoSatTen,
    required this.coCheTinhDiemId,
    required this.coCheTinhDiemTen,
    required this.trangThaiId,
    required this.trangThaiTen,
    required this.ngayBatDau,
    required this.ngayKetThuc,
    required this.tyleThamGiaToiThieu,
    required this.tyLeDongYToiThieu,
    required this.isAnDanh,
    required this.isVoted,
    required this.createdAt,
    required this.cauHois,
  });

  bool get canVote => trangThaiId == KhaoSatTrangThai.published && !isVoted;

  factory KhaoSatDetailResponse.fromJson(
    Map<String, dynamic> json,
  ) => KhaoSatDetailResponse(
    id: json['id'] as int? ?? 0,
    tieuDe: json['tieuDe'] as String? ?? '',
    moTa: json['moTa'] as String? ?? '',
    loaiKhaoSatId: json['loaiKhaoSatId'] as int? ?? 0,
    loaiKhaoSatTen: json['loaiKhaoSatTen'] as String? ?? '',
    coCheTinhDiemId: json['coCheTinhDiemId'] as int? ?? 0,
    coCheTinhDiemTen: json['coCheTinhDiemTen'] as String? ?? '',
    trangThaiId: json['trangThaiId'] as int? ?? 0,
    trangThaiTen: json['trangThaiTen'] as String? ?? '',
    ngayBatDau:
        DateTime.tryParse(json['ngayBatDau'] as String? ?? '') ??
        DateTime.now(),
    ngayKetThuc:
        DateTime.tryParse(json['ngayKetThuc'] as String? ?? '') ??
        DateTime.now(),
    tyleThamGiaToiThieu: (json['tyleThamGiaToiThieu'] as num?)?.toDouble() ?? 0,
    tyLeDongYToiThieu: (json['tyLeDongYToiThieu'] as num?)?.toDouble() ?? 0,
    isAnDanh: json['isAnDanh'] as bool? ?? false,
    isVoted: json['isVoted'] as bool? ?? false,
    createdAt:
        DateTime.tryParse(json['createdAt'] as String? ?? '') ?? DateTime.now(),
    cauHois: (json['cauHois'] as List<dynamic>? ?? [])
        .map((e) => CauHoiModel.fromJson(e as Map<String, dynamic>))
        .toList(),
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'tieuDe': tieuDe,
    'moTa': moTa,
    'trangThaiId': trangThaiId,
    'isVoted': isVoted,
    'cauHois': cauHois.map((e) => e.toJson()).toList(),
  };
}

class KetQuaLuaChonModel {
  final int luaChonId;
  final String noiDungLuaChon;
  final bool isUngVienBQT;
  final double soLuongPhieuBau;
  final double tyLePhanTram;

  const KetQuaLuaChonModel({
    required this.luaChonId,
    required this.noiDungLuaChon,
    required this.isUngVienBQT,
    required this.soLuongPhieuBau,
    required this.tyLePhanTram,
  });

  factory KetQuaLuaChonModel.fromJson(Map<String, dynamic> json) =>
      KetQuaLuaChonModel(
        luaChonId: json['luaChonId'] as int? ?? 0,
        noiDungLuaChon: json['noiDungLuaChon'] as String? ?? '',
        isUngVienBQT: json['isUngVienBQT'] as bool? ?? false,
        soLuongPhieuBau: (json['soLuongPhieuBau'] as num?)?.toDouble() ?? 0,
        tyLePhanTram: (json['tyLePhanTram'] as num?)?.toDouble() ?? 0,
      );

  KetQuaLuaChonModel copyWith({
    int? luaChonId,
    String? noiDungLuaChon,
    bool? isUngVienBQT,
    double? soLuongPhieuBau,
    double? tyLePhanTram,
  }) => KetQuaLuaChonModel(
    luaChonId: luaChonId ?? this.luaChonId,
    noiDungLuaChon: noiDungLuaChon ?? this.noiDungLuaChon,
    isUngVienBQT: isUngVienBQT ?? this.isUngVienBQT,
    soLuongPhieuBau: soLuongPhieuBau ?? this.soLuongPhieuBau,
    tyLePhanTram: tyLePhanTram ?? this.tyLePhanTram,
  );
}

class KetQuaCauHoiModel {
  final int cauHoiId;
  final String noiDungCauHoi;
  final bool isMultiSelect;
  final List<KetQuaLuaChonModel> ketQuaLuaChons;

  const KetQuaCauHoiModel({
    required this.cauHoiId,
    required this.noiDungCauHoi,
    required this.isMultiSelect,
    required this.ketQuaLuaChons,
  });

  factory KetQuaCauHoiModel.fromJson(Map<String, dynamic> json) =>
      KetQuaCauHoiModel(
        cauHoiId: json['cauHoiId'] as int? ?? 0,
        noiDungCauHoi: json['noiDungCauHoi'] as String? ?? '',
        isMultiSelect: json['isMultiSelect'] as bool? ?? false,
        ketQuaLuaChons: (json['ketQuaLuaChons'] as List<dynamic>? ?? [])
            .map((e) => KetQuaLuaChonModel.fromJson(e as Map<String, dynamic>))
            .toList(),
      );
}

class KetQuaKhaoSatResponse {
  final int khaoSatId;
  final String tieuDeKhaoSat;
  final int tongSoCanHo;
  final int soCanHoDaThamGia;
  final double tyLeThamGia;
  final double tyleThamGiaToiThieu;
  final bool isHieuLuc;
  final int coCheTinhDiemId;
  final String coCheTinhDiemTen;
  final List<KetQuaCauHoiModel> ketQuaCauHois;

  const KetQuaKhaoSatResponse({
    required this.khaoSatId,
    required this.tieuDeKhaoSat,
    required this.tongSoCanHo,
    required this.soCanHoDaThamGia,
    required this.tyLeThamGia,
    required this.tyleThamGiaToiThieu,
    required this.isHieuLuc,
    required this.coCheTinhDiemId,
    required this.coCheTinhDiemTen,
    required this.ketQuaCauHois,
  });

  bool get isDuNguong => tyLeThamGia >= tyleThamGiaToiThieu;

  factory KetQuaKhaoSatResponse.fromJson(Map<String, dynamic> json) =>
      KetQuaKhaoSatResponse(
        khaoSatId: json['khaoSatId'] as int? ?? 0,
        tieuDeKhaoSat: json['tieuDeKhaoSat'] as String? ?? '',
        tongSoCanHo: json['tongSoCanHo'] as int? ?? 0,
        soCanHoDaThamGia: json['soCanHoDaThamGia'] as int? ?? 0,
        tyLeThamGia: (json['tyLeThamGia'] as num?)?.toDouble() ?? 0,
        tyleThamGiaToiThieu:
            (json['tyleThamGiaToiThieu'] as num?)?.toDouble() ?? 0,
        isHieuLuc: json['isHieuLuc'] as bool? ?? false,
        coCheTinhDiemId: json['coCheTinhDiemId'] as int? ?? 0,
        coCheTinhDiemTen: json['coCheTinhDiemTen'] as String? ?? '',
        ketQuaCauHois: (json['ketQuaCauHois'] as List<dynamic>? ?? [])
            .map((e) => KetQuaCauHoiModel.fromJson(e as Map<String, dynamic>))
            .toList(),
      );

  KetQuaKhaoSatResponse copyWith({
    int? khaoSatId,
    String? tieuDeKhaoSat,
    int? tongSoCanHo,
    int? soCanHoDaThamGia,
    double? tyLeThamGia,
    double? tyleThamGiaToiThieu,
    bool? isHieuLuc,
    int? coCheTinhDiemId,
    String? coCheTinhDiemTen,
    List<KetQuaCauHoiModel>? ketQuaCauHois,
  }) => KetQuaKhaoSatResponse(
    khaoSatId: khaoSatId ?? this.khaoSatId,
    tieuDeKhaoSat: tieuDeKhaoSat ?? this.tieuDeKhaoSat,
    tongSoCanHo: tongSoCanHo ?? this.tongSoCanHo,
    soCanHoDaThamGia: soCanHoDaThamGia ?? this.soCanHoDaThamGia,
    tyLeThamGia: tyLeThamGia ?? this.tyLeThamGia,
    tyleThamGiaToiThieu: tyleThamGiaToiThieu ?? this.tyleThamGiaToiThieu,
    isHieuLuc: isHieuLuc ?? this.isHieuLuc,
    coCheTinhDiemId: coCheTinhDiemId ?? this.coCheTinhDiemId,
    coCheTinhDiemTen: coCheTinhDiemTen ?? this.coCheTinhDiemTen,
    ketQuaCauHois: ketQuaCauHois ?? this.ketQuaCauHois,
  );
}

class TraLoiRequest {
  final int luaChonId;
  final String? noiDungTraLoiTuDo;

  const TraLoiRequest({required this.luaChonId, this.noiDungTraLoiTuDo});

  Map<String, dynamic> toJson() => {
    'luaChonId': luaChonId,
    'noiDungTraLoiTuDo': noiDungTraLoiTuDo,
  };
}

class XacNhanBieuQuyetRequest {
  final int khaoSatId;
  final int canHoId;
  final String otpCode;
  final List<TraLoiRequest> traLois;

  const XacNhanBieuQuyetRequest({
    required this.khaoSatId,
    required this.canHoId,
    required this.otpCode,
    required this.traLois,
  });

  Map<String, dynamic> toJson() => {
    'khaoSatId': khaoSatId,
    'canHoId': canHoId,
    'otpCode': otpCode,
    'traLois': traLois.map((e) => e.toJson()).toList(),
  };
}
