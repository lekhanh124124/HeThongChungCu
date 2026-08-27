import 'package:flutter/material.dart';

import 'package:klks_app/design/tokens/colors.dart';
import 'package:klks_app/design/tokens/typography.dart';
import 'package:klks_app/design/tokens/radius.dart';
import 'package:klks_app/design/foundations/constants.dart';
import 'package:klks_app/design/components/buttons/app_button.dart';

enum AppBadgeVariant { success, warning, error, info }

class AppStatusBadge extends StatelessWidget {
  const AppStatusBadge({
    super.key,
    required this.label,
    this.variant = AppBadgeVariant.info,
  });

  final String label;
  final AppBadgeVariant variant;

  Color get _bgColor => switch (variant) {
    AppBadgeVariant.success => AppColors.successLight,
    AppBadgeVariant.warning => AppColors.warningLight,
    AppBadgeVariant.error => AppColors.errorLight,
    AppBadgeVariant.info => AppColors.primaryLight,
  };

  Color get _textColor => switch (variant) {
    AppBadgeVariant.success => AppColors.success,
    AppBadgeVariant.warning => AppColors.warning,
    AppBadgeVariant.error => AppColors.error,
    AppBadgeVariant.info => AppColors.primary,
  };

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(color: _bgColor, borderRadius: AppRadius.badge),
      child: Text(
        label.toUpperCase(),
        style: AppTypography.captionSmall.copyWith(
          color: _textColor,
          fontWeight: FontWeight.w700,
          letterSpacing: 0.5,
        ),
      ),
    );
  }
}

class AppConfirmDialog extends StatelessWidget {
  const AppConfirmDialog({
    super.key,
    required this.title,
    required this.message,
    this.confirmLabel = 'Xác nhận',
    this.cancelLabel = 'Huỷ',
    this.isDangerous = false,
  });

  final String title;
  final String message;
  final String confirmLabel;
  final String cancelLabel;

  final bool isDangerous;

  static Future<bool?> show(
    BuildContext context, {
    required String title,
    required String message,
    String confirmLabel = 'Xác nhận',
    String cancelLabel = 'Huỷ',
    bool isDangerous = false,
  }) {
    return showDialog<bool>(
      context: context,
      builder: (_) => AppConfirmDialog(
        title: title,
        message: message,
        confirmLabel: confirmLabel,
        cancelLabel: cancelLabel,
        isDangerous: isDangerous,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(title),
      content: Text(message),
      actions: [
        AppButton(
          label: cancelLabel,
          variant: AppButtonVariant.outline,
          expanded: false,
          height: 40,
          onPressed: () => Navigator.of(context).pop(false),
        ),
        AppButton(
          label: confirmLabel,
          variant: isDangerous
              ? AppButtonVariant.danger
              : AppButtonVariant.primary,
          expanded: false,
          height: 40,
          onPressed: () => Navigator.of(context).pop(true),
        ),
      ],
    );
  }
}

class AppLoadingIndicator extends StatelessWidget {
  const AppLoadingIndicator({
    super.key,
    required this.child,
    this.isLoading = false,
  });

  final Widget child;
  final bool isLoading;

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        child,
        if (isLoading)
          Positioned.fill(
            child: ColoredBox(
              color: const Color(0x66FFFFFF),
              child: Center(
                child: CircularProgressIndicator(
                  color: AppColors.primary,
                  strokeWidth: AppConstants.spinnerStrokeWidth,
                ),
              ),
            ),
          ),
      ],
    );
  }
}
