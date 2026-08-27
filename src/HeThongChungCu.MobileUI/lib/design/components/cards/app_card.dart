import 'package:flutter/material.dart';

import 'package:klks_app/design/tokens/colors.dart';
import 'package:klks_app/design/tokens/radius.dart';
import 'package:klks_app/design/tokens/elevation.dart';
import 'package:klks_app/design/tokens/spacing.dart';
import 'package:klks_app/design/tokens/typography.dart';

class AppCard extends StatelessWidget {
  const AppCard({
    super.key,
    required this.child,
    this.onTap,
    this.padding,
    this.color,
    this.margin,
  });

  final Widget child;
  final VoidCallback? onTap;
  final EdgeInsetsGeometry? padding;
  final Color? color;
  final EdgeInsetsGeometry? margin;

  @override
  Widget build(BuildContext context) {
    final effectiveColor = color ?? AppColors.surface;

    return Container(
      margin: margin,
      decoration: BoxDecoration(
        color: effectiveColor,
        borderRadius: AppRadius.card,
        boxShadow: AppElevation.level1,
      ),
      clipBehavior: Clip.antiAlias,
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          onTap: onTap,
          splashColor: (color ?? AppColors.primary).withAlpha(20),
          highlightColor: (color ?? AppColors.primary).withAlpha(10),
          child: Padding(
            padding: padding ?? AppSpacing.cardPadding,
            child: child,
          ),
        ),
      ),
    );
  }
}

class AppServiceCard extends StatelessWidget {
  const AppServiceCard({
    super.key,
    required this.title,
    this.subtitle,
    this.leading,
    this.icon,
    this.onTap,
    this.trailing,
    this.margin,
  });

  final String title;
  final String? subtitle;
  final Widget? leading;
  final IconData? icon;
  final VoidCallback? onTap;
  final Widget? trailing;
  final EdgeInsetsGeometry? margin;

  @override
  Widget build(BuildContext context) {
    return AppCard(
      onTap: onTap,
      margin: margin,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      child: Row(
        children: [
          if (leading != null) ...[
            leading!,
            const SizedBox(width: 12),
          ] else if (icon != null) ...[
            Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                color: AppColors.primaryLight,
                borderRadius: AppRadius.buttonSmall,
              ),
              child: Icon(icon, color: AppColors.primary, size: 20),
            ),
            const SizedBox(width: 12),
          ],

          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: AppTypography.subhead.copyWith(
                    color: AppColors.textPrimary,
                  ),
                ),
                if (subtitle != null) ...[
                  const SizedBox(height: 2),
                  Text(
                    subtitle!,
                    style: AppTypography.caption.copyWith(
                      color: AppColors.textSecondary,
                    ),
                  ),
                ],
              ],
            ),
          ),

          trailing ??
              const Icon(
                Icons.chevron_right,
                color: AppColors.textSecondary,
                size: 20,
              ),
        ],
      ),
    );
  }
}
