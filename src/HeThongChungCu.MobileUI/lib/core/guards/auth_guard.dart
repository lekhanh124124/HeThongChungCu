import 'package:flutter/material.dart';

import 'package:klks_app/features/auth/services/auth_service.dart';
import 'package:klks_app/core/storage/user_session.dart';

class AuthGuard extends ChangeNotifier {
  AuthGuard._();
  static final AuthGuard instance = AuthGuard._();

  final UserSession _session = UserSession.instance;

  AuthStatus _status = AuthStatus.unknown;
  AuthStatus get status => _status;

  bool _initialized = false;
  bool _isInitializing = false;

  Future<void> init() async {
    if (_initialized || _isInitializing) return;
    _isInitializing = true;
    try {
      final results = await Future.wait([
        tryAutoLogin(),
        Future.delayed(const Duration(milliseconds: 2000)),
      ]);
      final isLoggedIn = results[0] as bool;
      _setStatus(
        isLoggedIn ? AuthStatus.authenticated : AuthStatus.unauthenticated,
      );
    } catch (_) {
      _setStatus(AuthStatus.unauthenticated);
    } finally {
      _initialized = true;
      _isInitializing = false;
    }
  }

  Future<bool> tryAutoLogin() async {
    final refreshToken = _session.refreshToken;
    if (refreshToken == null || refreshToken.isEmpty) return false;

    final newAccess = await AuthService.instance.refreshToken();
    return newAccess != null;
  }

  Future<void> logout() async {
    await AuthService.instance.logout();
    setUnauthenticated();
  }

  void _setStatus(AuthStatus status) {
    if (_status == status) return;
    _status = status;
    notifyListeners();
  }

  void setAuthenticated() => _setStatus(AuthStatus.authenticated);
  void setUnauthenticated() => _setStatus(AuthStatus.unauthenticated);
}

enum AuthStatus { unknown, authenticated, unauthenticated }
