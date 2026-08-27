import 'package:flutter/material.dart';

abstract final class AppElevation {
  static const List<BoxShadow> level1 = [
    BoxShadow(color: Color(0x0D000000), blurRadius: 4, offset: Offset(0, 2)),
    BoxShadow(color: Color(0x08000000), blurRadius: 1, offset: Offset(0, 0)),
  ];

  static const List<BoxShadow> level2 = [
    BoxShadow(color: Color(0x1A000000), blurRadius: 16, offset: Offset(0, 4)),
    BoxShadow(color: Color(0x0D000000), blurRadius: 4, offset: Offset(0, 1)),
  ];

  static const List<BoxShadow> level3 = [
    BoxShadow(color: Color(0x29000000), blurRadius: 32, offset: Offset(0, 8)),
  ];

  static const double cardElevation = 1.0;
  static const double dialogElevation = 6.0;
  static const double bottomNavElevation = 8.0;
}
