import 'package:flutter/material.dart';

import 'package:klks_app/design/tokens/typography.dart';
import 'package:klks_app/design/tokens/colors.dart';

abstract final class AppTextTheme {
  static TextTheme get light =>
      const TextTheme(
        displayLarge: AppTypography.display,
        displayMedium: AppTypography.display,
        displaySmall: AppTypography.display,

        headlineLarge: AppTypography.headline,
        headlineMedium: AppTypography.headline,
        headlineSmall: AppTypography.subhead,

        titleLarge: AppTypography.subhead,
        titleMedium: AppTypography.subhead,
        titleSmall: AppTypography.subhead,

        bodyLarge: AppTypography.bodyMedium,
        bodyMedium: AppTypography.body,
        bodySmall: AppTypography.captionSmall,

        labelLarge: AppTypography.buttonLabel,
        labelMedium: AppTypography.caption,
        labelSmall: AppTypography.captionSmall,
      ).apply(
        bodyColor: AppColors.textPrimary,
        displayColor: AppColors.textPrimary,
      );
}
