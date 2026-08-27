import 'package:flutter/material.dart';

import 'package:klks_app/design/tokens/colors.dart';
import 'package:klks_app/design/tokens/typography.dart';
import 'package:klks_app/design/foundations/constants.dart';

enum AppButtonVariant { primary, secondary, outline, danger }

class AppButton extends StatelessWidget {
  const AppButton({
    super.key,
    required this.label,
    this.onPressed,
    this.variant = AppButtonVariant.primary,
    this.isLoading = false,
    this.leadingIcon,
    this.expanded = true,
    this.height = 52.0,
    this.backgroundColor,
    this.foregroundColor,
    this.borderColor,
  });

  final String label;
  final VoidCallback? onPressed;
  final AppButtonVariant variant;
  final bool isLoading;
  final IconData? leadingIcon;
  final bool expanded;
  final double height;

  final Color? backgroundColor;

  final Color? foregroundColor;

  final Color? borderColor;

  @override
  Widget build(BuildContext context) {
    final effectiveOnPressed = isLoading ? null : onPressed;

    Widget child = _ButtonContent(
      label: label,
      isLoading: isLoading,
      leadingIcon: leadingIcon,
      variant: variant,
      overrideForegroundColor: foregroundColor,
    );

    Widget button = switch (variant) {
      AppButtonVariant.primary => _PrimaryButton(
        onPressed: effectiveOnPressed,
        height: height,
        backgroundColor: backgroundColor,
        foregroundColor: foregroundColor,
        child: child,
      ),
      AppButtonVariant.secondary => _SecondaryButton(
        onPressed: effectiveOnPressed,
        height: height,
        backgroundColor: backgroundColor,
        foregroundColor: foregroundColor,
        child: child,
      ),
      AppButtonVariant.outline => _OutlineButton(
        onPressed: effectiveOnPressed,
        height: height,
        foregroundColor: foregroundColor,
        borderColor: borderColor,
        child: child,
      ),
      AppButtonVariant.danger => _DangerButton(
        onPressed: effectiveOnPressed,
        height: height,
        backgroundColor: backgroundColor,
        foregroundColor: foregroundColor,
        child: child,
      ),
    };

    return expanded
        ? SizedBox(width: double.infinity, child: button)
        : IntrinsicWidth(child: button);
  }
}

class _PrimaryButton extends StatelessWidget {
  const _PrimaryButton({
    required this.onPressed,
    required this.height,
    required this.child,
    this.backgroundColor,
    this.foregroundColor,
  });
  final VoidCallback? onPressed;
  final double height;
  final Widget child;
  final Color? backgroundColor;
  final Color? foregroundColor;

  @override
  Widget build(BuildContext context) {
    final bg = backgroundColor ?? AppColors.primary;
    final fg = foregroundColor ?? AppColors.textOnPrimary;

    return ElevatedButton(
      onPressed: onPressed,
      style:
          ElevatedButton.styleFrom(
            minimumSize: Size(0, height),
            backgroundColor: bg,
            foregroundColor: fg,
            disabledBackgroundColor: AppColors.secondaryLight,
            disabledForegroundColor: AppColors.textDisabled,
          ).copyWith(
            overlayColor: WidgetStateProperty.resolveWith(
              (states) => states.contains(WidgetState.pressed)
                  ? fg.withAlpha(30)
                  : null,
            ),
          ),
      child: child,
    );
  }
}

class _SecondaryButton extends StatelessWidget {
  const _SecondaryButton({
    required this.onPressed,
    required this.height,
    required this.child,
    this.backgroundColor,
    this.foregroundColor,
  });
  final VoidCallback? onPressed;
  final double height;
  final Widget child;
  final Color? backgroundColor;
  final Color? foregroundColor;

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: onPressed,
      style: ElevatedButton.styleFrom(
        minimumSize: Size(0, height),
        backgroundColor: backgroundColor ?? AppColors.secondaryLight,
        foregroundColor: foregroundColor ?? AppColors.textPrimary,
        disabledBackgroundColor: AppColors.secondaryLight,
        disabledForegroundColor: AppColors.textDisabled,
        elevation: 0,
        shadowColor: Colors.transparent,
      ),
      child: child,
    );
  }
}

class _OutlineButton extends StatelessWidget {
  const _OutlineButton({
    required this.onPressed,
    required this.height,
    required this.child,
    this.foregroundColor,
    this.borderColor,
  });
  final VoidCallback? onPressed;
  final double height;
  final Widget child;
  final Color? foregroundColor;
  final Color? borderColor;

  @override
  Widget build(BuildContext context) {
    final fg = foregroundColor ?? AppColors.primary;
    final border = borderColor ?? fg;

    return OutlinedButton(
      onPressed: onPressed,
      style:
          OutlinedButton.styleFrom(
            minimumSize: Size(0, height),
            foregroundColor: fg,
            disabledForegroundColor: AppColors.textDisabled,
          ).copyWith(
            side: WidgetStateProperty.resolveWith((states) {
              if (states.contains(WidgetState.disabled)) {
                return const BorderSide(color: AppColors.border, width: 1.5);
              }
              return BorderSide(color: border, width: 1.5);
            }),
          ),
      child: child,
    );
  }
}

class _DangerButton extends StatelessWidget {
  const _DangerButton({
    required this.onPressed,
    required this.height,
    required this.child,
    this.backgroundColor,
    this.foregroundColor,
  });
  final VoidCallback? onPressed;
  final double height;
  final Widget child;
  final Color? backgroundColor;
  final Color? foregroundColor;

  @override
  Widget build(BuildContext context) {
    return ElevatedButton(
      onPressed: onPressed,
      style: ElevatedButton.styleFrom(
        minimumSize: Size(0, height),
        backgroundColor: backgroundColor ?? AppColors.error,
        foregroundColor: foregroundColor ?? AppColors.textOnPrimary,
        disabledBackgroundColor: AppColors.secondaryLight,
        disabledForegroundColor: AppColors.textDisabled,
        elevation: 0,
        shadowColor: Colors.transparent,
      ),
      child: child,
    );
  }
}

class _ButtonContent extends StatelessWidget {
  const _ButtonContent({
    required this.label,
    required this.isLoading,
    required this.variant,
    this.leadingIcon,
    this.overrideForegroundColor,
  });

  final String label;
  final bool isLoading;
  final IconData? leadingIcon;
  final AppButtonVariant variant;
  final Color? overrideForegroundColor;

  Color get _spinnerColor {
    if (overrideForegroundColor != null) return overrideForegroundColor!;
    return switch (variant) {
      AppButtonVariant.primary ||
      AppButtonVariant.danger => AppColors.textOnPrimary,
      _ => AppColors.primary,
    };
  }

  @override
  Widget build(BuildContext context) {
    if (isLoading) {
      return Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          SizedBox.square(
            dimension: AppConstants.spinnerSize,
            child: CircularProgressIndicator(
              strokeWidth: AppConstants.spinnerStrokeWidth,
              valueColor: AlwaysStoppedAnimation(_spinnerColor),
            ),
          ),
          const SizedBox(width: 8),
          Text(label, style: AppTypography.buttonLabel),
        ],
      );
    }

    if (leadingIcon != null) {
      return Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(leadingIcon, size: 18),
          const SizedBox(width: 8),
          Text(label, style: AppTypography.buttonLabel),
        ],
      );
    }

    return Text(label, style: AppTypography.buttonLabel);
  }
}
