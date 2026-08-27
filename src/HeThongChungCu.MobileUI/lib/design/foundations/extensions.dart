import 'package:flutter/material.dart';

import '../tokens/colors.dart';

extension AppContextExtensions on BuildContext {
  ColorScheme get colorScheme => Theme.of(this).colorScheme;

  TextTheme get textTheme => Theme.of(this).textTheme;

  Size get screenSize => MediaQuery.sizeOf(this);

  double get screenWidth => screenSize.width;
  double get screenHeight => screenSize.height;

  bool get isKeyboardVisible => MediaQuery.viewInsetsOf(this).bottom > 0;
}

extension DoubleSpacingExtension on double {
  SizedBox get verticalSpace => SizedBox(height: this);

  SizedBox get horizontalSpace => SizedBox(width: this);
}

extension IntSpacingExtension on int {
  SizedBox get verticalSpace => SizedBox(height: toDouble());
  SizedBox get horizontalSpace => SizedBox(width: toDouble());
}

extension TextStyleColorExtension on TextStyle {
  TextStyle withColor(Color color) => copyWith(color: color);
  TextStyle get primary => copyWith(color: AppColors.primary);
  TextStyle get secondary => copyWith(color: AppColors.textSecondary);
  TextStyle get error => copyWith(color: AppColors.error);
  TextStyle get disabled => copyWith(color: AppColors.textDisabled);
  TextStyle get onPrimary => copyWith(color: AppColors.textOnPrimary);
}
