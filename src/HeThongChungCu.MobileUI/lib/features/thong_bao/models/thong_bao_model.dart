export 'package:klks_app/features/shared/models/paging_model.dart';

class ThongBaoItem {
  final int id;
  final int thongBaoId;
  final String tieuDe;
  final String noiDung;
  final int loaiThongBaoId;
  final String tenLoaiThongBao;
  final String? referenceId;
  final String? metadata;
  final bool isRead;
  final DateTime createdAt;
  final DateTime? readAt;

  const ThongBaoItem({
    required this.id,
    required this.thongBaoId,
    required this.tieuDe,
    required this.noiDung,
    required this.loaiThongBaoId,
    required this.tenLoaiThongBao,
    this.referenceId,
    this.metadata,
    required this.isRead,
    required this.createdAt,
    this.readAt,
  });

  bool get isMoi => DateTime.now().difference(createdAt).inHours < 24;

  String get thoiGianHienThi {
    final diff = DateTime.now().difference(createdAt);
    if (diff.inMinutes < 1) return 'Vừa xong';
    if (diff.inMinutes < 60) return '${diff.inMinutes} phút trước';
    if (diff.inHours < 24) return '${diff.inHours} giờ trước';
    return '${diff.inDays} ngày trước';
  }

  factory ThongBaoItem.fromJson(Map<String, dynamic> json) => ThongBaoItem(
    id: json['id'] as int,
    thongBaoId: json['thongBaoId'] as int,
    tieuDe: json['tieuDe'] as String? ?? '',
    noiDung: json['noiDung'] as String? ?? '',
    loaiThongBaoId: json['loaiThongBaoId'] as int? ?? 0,
    tenLoaiThongBao: json['tenLoaiThongBao'] as String? ?? '',
    referenceId: json['referenceId'] as String?,
    metadata: json['metadata'] as String?,
    isRead: json['isRead'] as bool? ?? false,
    createdAt: DateTime.parse(json['createdAt'] as String),
    readAt: json['readAt'] != null
        ? DateTime.parse(json['readAt'] as String)
        : null,
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'thongBaoId': thongBaoId,
    'tieuDe': tieuDe,
    'noiDung': noiDung,
    'loaiThongBaoId': loaiThongBaoId,
    'tenLoaiThongBao': tenLoaiThongBao,
    'referenceId': referenceId,
    'metadata': metadata,
    'isRead': isRead,
    'createdAt': createdAt.toIso8601String(),
    'readAt': readAt?.toIso8601String(),
  };

  ThongBaoItem copyWith({
    int? id,
    int? thongBaoId,
    String? tieuDe,
    String? noiDung,
    int? loaiThongBaoId,
    String? tenLoaiThongBao,
    String? referenceId,
    String? metadata,
    bool? isRead,
    DateTime? createdAt,
    DateTime? readAt,
  }) => ThongBaoItem(
    id: id ?? this.id,
    thongBaoId: thongBaoId ?? this.thongBaoId,
    tieuDe: tieuDe ?? this.tieuDe,
    noiDung: noiDung ?? this.noiDung,
    loaiThongBaoId: loaiThongBaoId ?? this.loaiThongBaoId,
    tenLoaiThongBao: tenLoaiThongBao ?? this.tenLoaiThongBao,
    referenceId: referenceId ?? this.referenceId,
    metadata: metadata ?? this.metadata,
    isRead: isRead ?? this.isRead,
    createdAt: createdAt ?? this.createdAt,
    readAt: readAt ?? this.readAt,
  );
}
