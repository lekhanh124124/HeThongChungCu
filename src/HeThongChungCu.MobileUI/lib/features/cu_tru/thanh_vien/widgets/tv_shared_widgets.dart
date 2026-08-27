import 'package:flutter/material.dart';

import 'package:klks_app/design/design.dart';

import '../models/thanh_vien_model.dart';

String tvFmtDate(DateTime d) =>
    '${d.day.toString().padLeft(2, '0')}/'
    '${d.month.toString().padLeft(2, '0')}/'
    '${d.year}';

extension TvDateTimeExt on DateTime {
  String get tvFormatted => tvFmtDate(this);
}

AppBadgeVariant tvTrangThaiVariant(int id) => switch (id) {
  4 => AppBadgeVariant.info,
  1 => AppBadgeVariant.warning,
  2 => AppBadgeVariant.success,
  3 => AppBadgeVariant.error,
  _ => AppBadgeVariant.info,
};

(Color bg, Color text) tvTrangThaiColor(int id) => switch (id) {
  4 => (AppColors.secondaryLight, AppColors.textSecondary),
  1 => (AppColors.warningLight, AppColors.warning),
  2 => (AppColors.successLight, AppColors.success),
  3 => (AppColors.errorLight, AppColors.error),
  _ => (AppColors.primaryLight, AppColors.primary),
};

IconData tvTrangThaiIcon(int id) => switch (id) {
  4 => Icons.save_outlined,
  1 => Icons.hourglass_top_outlined,
  2 => Icons.check_circle_outline,
  3 => Icons.cancel_outlined,
  _ => Icons.info_outline,
};

IconData tvLoaiYeuCauIcon(int id) => switch (id) {
  1 => Icons.person_add_outlined,
  2 => Icons.edit_outlined,
  3 => Icons.person_remove_outlined,
  _ => Icons.description_outlined,
};

class TvSectionCard extends StatelessWidget {
  final String title;
  final List<Widget> children;
  final EdgeInsetsGeometry? padding;

  const TvSectionCard({
    super.key,
    required this.title,
    required this.children,
    this.padding,
  });

  @override
  Widget build(BuildContext context) {
    return AppCard(
      padding: padding,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(title, style: AppTypography.subhead),
          const Divider(height: AppSpacing.lg),
          ...children,
        ],
      ),
    );
  }
}

class TvInfoRow extends StatelessWidget {
  final String label;
  final String value;
  final bool highlight;
  final double labelWidth;

  const TvInfoRow({
    super.key,
    required this.label,
    required this.value,
    this.highlight = false,
    this.labelWidth = 120,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: labelWidth,
            child: Text(label, style: AppTypography.captionSmall.secondary),
          ),
          Expanded(
            child: Text(
              value,
              style: highlight
                  ? AppTypography.bodyMedium.error
                  : AppTypography.bodyMedium,
            ),
          ),
        ],
      ),
    );
  }
}

class TvMemberAvatar extends StatelessWidget {
  final String? imageUrl;
  final String name;
  final double radius;
  final double? fontSize;

  const TvMemberAvatar({
    super.key,
    required this.imageUrl,
    required this.name,
    this.radius = 24,
    this.fontSize,
  });

  @override
  Widget build(BuildContext context) {
    final initial = name.isNotEmpty ? name[0].toUpperCase() : '?';
    return CircleAvatar(
      radius: radius,
      backgroundColor: AppColors.primaryLight,
      backgroundImage: imageUrl != null ? NetworkImage(imageUrl!) : null,
      child: imageUrl == null
          ? Text(
              initial,
              style: TextStyle(
                fontSize: fontSize ?? (radius * 0.75),
                color: AppColors.primary,
                fontWeight: FontWeight.w600,
              ),
            )
          : null,
    );
  }
}

class TvMemberReadonlyCard extends StatelessWidget {
  final ThanhVienCuTruModel thanhVien;
  final String diaChiCanHo;
  final String badgeLabel;
  final AppBadgeVariant badgeVariant;

  const TvMemberReadonlyCard({
    super.key,
    required this.thanhVien,
    required this.diaChiCanHo,
    required this.badgeLabel,
    this.badgeVariant = AppBadgeVariant.warning,
  });

  @override
  Widget build(BuildContext context) {
    return AppCard(
      color: AppColors.secondaryLight,
      child: Row(
        children: [
          TvMemberAvatar(
            imageUrl: thanhVien.anhDaiDienUrl,
            name: thanhVien.fullName,
            radius: 22,
          ),
          const SizedBox(width: AppSpacing.sm),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(thanhVien.fullName, style: AppTypography.subhead),
                Text(
                  '${thanhVien.loaiQuanHeTen} · $diaChiCanHo',
                  style: AppTypography.captionSmall.secondary,
                ),
              ],
            ),
          ),
          AppStatusBadge(label: badgeLabel, variant: badgeVariant),
        ],
      ),
    );
  }
}

class TvStatusBanner extends StatelessWidget {
  final int trangThaiId;
  final String tenTrangThai;

  const TvStatusBanner({
    super.key,
    required this.trangThaiId,
    required this.tenTrangThai,
  });

  @override
  Widget build(BuildContext context) {
    final (bg, fg) = tvTrangThaiColor(trangThaiId);
    final icon = tvTrangThaiIcon(trangThaiId);

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.md,
        vertical: AppSpacing.sm2,
      ),
      decoration: BoxDecoration(color: bg, borderRadius: AppRadius.card),
      child: Row(
        children: [
          Icon(icon, color: fg, size: 20),
          const SizedBox(width: AppSpacing.sm),
          Text(tenTrangThai, style: AppTypography.subhead.copyWith(color: fg)),
        ],
      ),
    );
  }
}
