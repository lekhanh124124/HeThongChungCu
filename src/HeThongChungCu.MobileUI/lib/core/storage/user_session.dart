import 'package:flutter/foundation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import 'package:klks_app/features/auth/models/user_model.dart';

abstract class _K {
  static const accessToken = 'accessToken';
  static const refreshToken = 'refreshToken';
  static const userId = 'userId';
  static const accountId = 'accountId';
  static const username = 'username';
  static const email = 'email';
  static const fullName = 'fullName';
  static const role = 'role';
  static const anhDaiDienUrl = 'anhDaiDienUrl';
}

class UserSession {
  UserSession._();
  static final UserSession instance = UserSession._();

  final _storage = const FlutterSecureStorage();

  final anhDaiDienUrlNotifier = ValueNotifier<String?>(null);

  String? get anhDaiDienUrl => anhDaiDienUrlNotifier.value;

  String? accessToken;
  String? refreshToken;
  String? userId;
  String? accountId;
  String? username;
  String? email;
  String? fullName;
  String? role;

  bool get isLoggedIn => accessToken?.isNotEmpty == true;

  Future<void> load() async {
    final values = await Future.wait([
      _storage.read(key: _K.accessToken),
      _storage.read(key: _K.refreshToken),
      _storage.read(key: _K.userId),
      _storage.read(key: _K.accountId),
      _storage.read(key: _K.username),
      _storage.read(key: _K.email),
      _storage.read(key: _K.fullName),
      _storage.read(key: _K.role),
      _storage.read(key: _K.anhDaiDienUrl),
    ]);

    accessToken = values[0];
    refreshToken = values[1];
    userId = values[2];
    accountId = values[3];
    username = values[4];
    email = values[5];
    fullName = values[6];
    role = values[7];

    anhDaiDienUrlNotifier.value = values[8];
  }

  Future<void> save(UserModel user) async {
    accessToken = user.accessToken;
    refreshToken = user.refreshToken;
    userId = user.userId.toString();
    accountId = user.accountId.toString();
    username = user.username;
    email = user.email;
    fullName = user.fullName;
    role = user.role;

    anhDaiDienUrlNotifier.value = user.anhDaiDienUrl;

    await Future.wait([
      _storage.write(key: _K.accessToken, value: user.accessToken),
      _storage.write(key: _K.refreshToken, value: user.refreshToken),
      _storage.write(key: _K.userId, value: user.userId.toString()),
      _storage.write(key: _K.accountId, value: user.accountId.toString()),
      _storage.write(key: _K.username, value: user.username),
      _storage.write(key: _K.email, value: user.email),
      _storage.write(key: _K.fullName, value: user.fullName),
      _storage.write(key: _K.role, value: user.role),
      _storage.write(key: _K.anhDaiDienUrl, value: user.anhDaiDienUrl),
    ]);
  }

  Future<void> updateTokens({
    required String accessToken,
    required String refreshToken,
  }) async {
    this.accessToken = accessToken;
    this.refreshToken = refreshToken;
    await Future.wait([
      _storage.write(key: _K.accessToken, value: accessToken),
      _storage.write(key: _K.refreshToken, value: refreshToken),
    ]);
  }

  Future<void> updateAvatar(String newUrl) async {
    anhDaiDienUrlNotifier.value = newUrl;
    await _storage.write(key: _K.anhDaiDienUrl, value: newUrl);
  }

  Future<void> clear() async {
    accessToken = null;
    refreshToken = null;
    userId = null;
    accountId = null;
    username = null;
    email = null;
    fullName = null;
    role = null;

    anhDaiDienUrlNotifier.value = null;

    await _storage.deleteAll();
  }
}
