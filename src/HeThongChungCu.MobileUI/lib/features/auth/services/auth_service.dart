import 'dart:async';

import 'package:dio/dio.dart';

import 'package:klks_app/core/guards/auth_guard.dart';
import 'package:klks_app/core/network/api_client.dart';
import 'package:klks_app/core/storage/user_session.dart';

import 'package:klks_app/features/thong_bao/services/thong_bao_hub_service.dart';

import '../models/user_model.dart';

class AuthService {
  AuthService._();
  static final AuthService instance = AuthService._();

  static final _client = ApiClient.instance;

  final _session = UserSession.instance;

  Future<UserModel> login({
    required String username,
    required String password,
  }) async {
    if (username.trim().isEmpty) {
      throw const AppException('Vui lòng nhập username');
    }
    if (password.trim().isEmpty) {
      throw const AppException('Vui lòng nhập password');
    }

    final res = await _client.post(
      '/api/auth/login',
      body: {'username': username.trim(), 'password': password.trim()},
    );
    final user = res.item(UserModel.fromJson);

    if (user.accessToken.isEmpty || user.refreshToken.isEmpty) {
      throw const AppException('Token không hợp lệ');
    }

    await _session.save(user);
    AuthGuard.instance.setAuthenticated();
    unawaited(ThongBaoHubService.instance.connect());

    return user;
  }

  Future<UserModel> register({
    required String email,
    required String password,
    required String confirmPassword,
  }) async {
    final res = await _client.post(
      '/api/auth/register',
      body: {
        'email': email.trim(),
        'password': password.trim(),
        'confirmPassword': confirmPassword.trim(),
      },
    );

    return res.item(UserModel.fromJson);
  }

  Future<void> logout() async {
    try {
      await ApiClient.instance.plainDio.post(
        '/api/auth/logout',
        options: Options(
          headers: {'Authorization': 'Bearer ${_session.accessToken ?? ''}'},
        ),
      );
    } catch (_) {
    } finally {
      await _session.clear();
      AuthGuard.instance.setUnauthenticated();
      unawaited(ThongBaoHubService.instance.disconnect());
    }
  }

  Future<String> forgotPassword({required String username}) async {
    if (username.trim().isEmpty) {
      throw const AppException('Vui lòng nhập username');
    }

    final res = await _client.post(
      '/api/auth/forgot-password',
      body: {'username': username.trim()},
    );
    return res.raw<String?>() ?? '';
  }

  Future<String> resetPassword({
    required String username,
    required String resetCode,
    required String newPassword,
    required String confirmPassword,
  }) async {
    if (newPassword.trim() != confirmPassword.trim()) {
      throw const AppException('Mật khẩu không khớp');
    }

    final res = await _client.post(
      '/api/auth/reset-password',
      body: {
        'username': username.trim(),
        'resetCode': resetCode.trim(),
        'newPassword': newPassword.trim(),
        'confirmPassword': confirmPassword.trim(),
      },
    );
    return res.raw<String?>() ?? '';
  }

  Future<String?> refreshToken() async {
    final token = _session.refreshToken;
    if (token == null || token.isEmpty) return null;

    try {
      final response = await ApiClient.instance.plainDio.post(
        '/api/auth/refresh-token',
        data: {'refreshToken': token},
      );

      final data = response.data;
      if (data == null) return null;

      final map = data as Map<String, dynamic>;
      final isOk = map['isOk'] as bool? ?? false;

      if (!isOk) return null;

      final result = map['result'];
      if (result == null) return null;

      final user = UserModel.fromJson(result as Map<String, dynamic>);
      if (user.accessToken.isEmpty) return null;

      await _session.save(user);
      await _session.updateTokens(
        accessToken: user.accessToken,
        refreshToken: user.refreshToken,
      );
      unawaited(ThongBaoHubService.instance.connect());

      return user.accessToken;
    } catch (_) {
      await _session.clear();
      AuthGuard.instance.setUnauthenticated();
      unawaited(ThongBaoHubService.instance.disconnect());
      return null;
    }
  }
}
