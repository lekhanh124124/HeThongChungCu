import 'package:flutter/material.dart';

abstract final class AppRadius {
  static const double xs = 4.0;
  static const double sm = 8.0;
  static const double input = 12.0;
  static const double standard = 16.0;
  static const double lg = 24.0;
  static const double full = 999.0;

  static const BorderRadius card = BorderRadius.all(Radius.circular(standard));
  static const BorderRadius inputField = BorderRadius.all(
    Radius.circular(input),
  );
  static const BorderRadius button = BorderRadius.all(
    Radius.circular(standard),
  );
  static const BorderRadius buttonSmall = BorderRadius.all(Radius.circular(sm));
  static const BorderRadius badge = BorderRadius.all(Radius.circular(full));
  static const BorderRadius modal = BorderRadius.vertical(
    top: Radius.circular(lg),
  );

  static RoundedRectangleBorder cardShape = const RoundedRectangleBorder(
    borderRadius: card,
  );

  static RoundedRectangleBorder buttonShape = const RoundedRectangleBorder(
    borderRadius: button,
  );
}
