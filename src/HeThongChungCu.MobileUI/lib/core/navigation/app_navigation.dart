import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

class AppNavigation {
  static StatefulNavigationShell? _shell;
  static GoRouter? _router;

  static void setShell(StatefulNavigationShell shell) => _shell = shell;
  static void setRouter(GoRouter router) => _router = router;

  static void goTab(int index, {bool reset = true}) {
    _shell?.goBranch(index, initialLocation: reset);
  }

  static void goHome() => goTab(0);
  static void goNotification() => goTab(1);
  static void goTienIch() => goTab(2);
  static void goResidence() => goTab(3);
  static void goProfile() => goTab(4);

  static void goTabThenPush(int tabIndex, String route) {
    _shell?.goBranch(tabIndex, initialLocation: true);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _router?.push(route);
    });
  }
}
