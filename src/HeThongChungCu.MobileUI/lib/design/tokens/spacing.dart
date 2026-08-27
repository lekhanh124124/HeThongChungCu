import 'package:flutter/material.dart';

abstract final class AppSpacing {
  static const double xs = 4.0;
  static const double sm = 8.0;
  static const double sm2 = 12.0;
  static const double md = 16.0;
  static const double lg = 24.0;
  static const double xl = 32.0;
  static const double xxl = 48.0;

  static const double screenHorizontal = md;

  static const double screenHorizontalWide = lg;

  static const double gapSmall = sm;

  static const double gapMedium = md;

  static const double gapLarge = lg;

  static const EdgeInsets insetAll4 = EdgeInsets.all(xs);
  static const EdgeInsets insetAll8 = EdgeInsets.all(sm);
  static const EdgeInsets insetAll16 = EdgeInsets.all(md);
  static const EdgeInsets insetAll24 = EdgeInsets.all(lg);

  static const EdgeInsets insetH16 = EdgeInsets.symmetric(horizontal: md);
  static const EdgeInsets insetH24 = EdgeInsets.symmetric(horizontal: lg);

  static const EdgeInsets insetV8 = EdgeInsets.symmetric(vertical: sm);
  static const EdgeInsets insetV12 = EdgeInsets.symmetric(vertical: sm2);
  static const EdgeInsets insetV16 = EdgeInsets.symmetric(vertical: md);

  static const EdgeInsets cardPadding = EdgeInsets.all(md);

  static const EdgeInsets buttonPadding = EdgeInsets.symmetric(
    horizontal: 20,
    vertical: 14,
  );

  static const EdgeInsets inputPadding = EdgeInsets.symmetric(
    horizontal: md,
    vertical: 14,
  );
}
