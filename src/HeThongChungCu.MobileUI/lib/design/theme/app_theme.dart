import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import 'package:klks_app/design/tokens/colors.dart';
import 'package:klks_app/design/tokens/radius.dart';
import 'package:klks_app/design/tokens/elevation.dart';
import 'package:klks_app/design/tokens/spacing.dart';
import 'package:klks_app/design/tokens/typography.dart';

import 'app_color_scheme.dart';
import 'app_text_theme.dart';

abstract final class AppTheme {
  static ThemeData get light => ThemeData(
    useMaterial3: true,
    colorScheme: AppColorScheme.light,
    textTheme: AppTextTheme.light,
    scaffoldBackgroundColor: AppColors.background,
    fontFamily: 'Be Vietnam Pro',

    appBarTheme: AppBarTheme(
      backgroundColor: AppColors.surface,
      foregroundColor: AppColors.textPrimary,
      elevation: 0,
      scrolledUnderElevation: 1,
      shadowColor: AppColors.border,
      surfaceTintColor: Colors.transparent,
      centerTitle: true,
      titleTextStyle: AppTypography.headline.copyWith(
        color: AppColors.textPrimary,
      ),
      systemOverlayStyle: const SystemUiOverlayStyle(
        statusBarColor: Colors.transparent,
        statusBarIconBrightness: Brightness.dark,
        statusBarBrightness: Brightness.light,
      ),
      iconTheme: const IconThemeData(color: AppColors.textPrimary, size: 24),
    ),

    bottomNavigationBarTheme: const BottomNavigationBarThemeData(
      backgroundColor: AppColors.surface,
      selectedItemColor: AppColors.primary,
      unselectedItemColor: AppColors.secondary,
      type: BottomNavigationBarType.fixed,
      elevation: AppElevation.bottomNavElevation,
      selectedLabelStyle: AppTypography.captionSmall,
      unselectedLabelStyle: AppTypography.captionSmall,
    ),

    elevatedButtonTheme: ElevatedButtonThemeData(
      style: ElevatedButton.styleFrom(
        backgroundColor: AppColors.primary,
        foregroundColor: AppColors.textOnPrimary,
        disabledBackgroundColor: AppColors.secondaryLight,
        disabledForegroundColor: AppColors.textDisabled,
        elevation: 0,
        shadowColor: Colors.transparent,
        padding: AppSpacing.buttonPadding,
        minimumSize: const Size(double.infinity, 52),
        shape: AppRadius.buttonShape,
        textStyle: AppTypography.buttonLabel,
      ),
    ),

    outlinedButtonTheme: OutlinedButtonThemeData(
      style: OutlinedButton.styleFrom(
        foregroundColor: AppColors.primary,
        disabledForegroundColor: AppColors.textDisabled,
        padding: AppSpacing.buttonPadding,
        minimumSize: const Size(double.infinity, 52),
        shape: AppRadius.buttonShape,
        side: const BorderSide(color: AppColors.primary, width: 1.5),
        textStyle: AppTypography.buttonLabel,
      ),
    ),

    textButtonTheme: TextButtonThemeData(
      style: TextButton.styleFrom(
        foregroundColor: AppColors.primary,
        padding: AppSpacing.buttonPadding,
        textStyle: AppTypography.buttonLabel,
      ),
    ),

    inputDecorationTheme: InputDecorationTheme(
      filled: true,
      fillColor: AppColors.inputFill,
      contentPadding: AppSpacing.inputPadding,
      hintStyle: AppTypography.input.copyWith(color: AppColors.textDisabled),
      errorStyle: AppTypography.captionSmall.copyWith(color: AppColors.error),
      border: OutlineInputBorder(
        borderRadius: AppRadius.inputField,
        borderSide: BorderSide.none,
      ),
      enabledBorder: OutlineInputBorder(
        borderRadius: AppRadius.inputField,
        borderSide: BorderSide.none,
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: AppRadius.inputField,
        borderSide: const BorderSide(
          color: AppColors.borderFocused,
          width: 1.5,
        ),
      ),
      errorBorder: OutlineInputBorder(
        borderRadius: AppRadius.inputField,
        borderSide: const BorderSide(color: AppColors.borderError, width: 1.5),
      ),
      focusedErrorBorder: OutlineInputBorder(
        borderRadius: AppRadius.inputField,
        borderSide: const BorderSide(color: AppColors.borderError, width: 1.5),
      ),
      disabledBorder: OutlineInputBorder(
        borderRadius: AppRadius.inputField,
        borderSide: BorderSide.none,
      ),
    ),

    cardTheme: CardThemeData(
      color: AppColors.surface,
      elevation: AppElevation.cardElevation,
      shadowColor: const Color(0x0D000000),
      surfaceTintColor: Colors.transparent,
      shape: AppRadius.cardShape,
      margin: EdgeInsets.zero,
      clipBehavior: Clip.antiAlias,
    ),

    dialogTheme: DialogThemeData(
      backgroundColor: AppColors.surface,
      elevation: AppElevation.dialogElevation,
      surfaceTintColor: Colors.transparent,
      shape: RoundedRectangleBorder(borderRadius: AppRadius.card),
      titleTextStyle: AppTypography.headline.copyWith(
        color: AppColors.textPrimary,
      ),
      contentTextStyle: AppTypography.body.copyWith(
        color: AppColors.textSecondary,
      ),
    ),

    chipTheme: ChipThemeData(
      backgroundColor: AppColors.inputFill,
      selectedColor: AppColors.primaryLight,
      disabledColor: AppColors.secondaryLight,
      labelStyle: AppTypography.caption,
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
      shape: const StadiumBorder(),
    ),

    dividerTheme: const DividerThemeData(
      color: AppColors.divider,
      thickness: 1,
      space: 1,
    ),

    progressIndicatorTheme: const ProgressIndicatorThemeData(
      color: AppColors.primary,
    ),

    snackBarTheme: SnackBarThemeData(
      backgroundColor: AppColors.textPrimary,
      contentTextStyle: AppTypography.body.copyWith(color: AppColors.surface),
      actionTextColor: AppColors.primaryLight,
      shape: RoundedRectangleBorder(borderRadius: AppRadius.buttonSmall),
      behavior: SnackBarBehavior.floating,
    ),
  );
}
