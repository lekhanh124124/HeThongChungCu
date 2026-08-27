import 'dart:io';
import 'package:dio/dio.dart';

import 'package:klks_app/core/network/api_client.dart';
import 'package:klks_app/core/storage/user_session.dart';
import 'package:klks_app/features/auth/services/auth_service.dart';
import '../models/user_profile.dart';

class ProfileService {
  ProfileService._();
  static final ProfileService instance = ProfileService._();

  static final _client = ApiClient.instance;

  Future<UserProfile> getProfile() async {
    final res = await _client.post('/api/profile/get-profile');
    return res.item(UserProfile.fromJson);
  }

  Future<String> changeAvatar(File file) async {
    final formData = FormData.fromMap({
      'avatar': await MultipartFile.fromFile(
        file.path,
        filename: file.path.split('/').last,
      ),
    });

    final res = await _client.postForm('/api/profile/change-avatar', formData);
    final url = res.raw<String>();
    await UserSession.instance.updateAvatar(url);
    return url;
  }

  Future<void> changePassword({
    required String oldPassword,
    required String newPassword,
    required String confirmPassword,
  }) async {
    if (newPassword != confirmPassword) {
      throw const AppException('Mật khẩu xác nhận không khớp');
    }
    if (oldPassword == newPassword) {
      throw const AppException('Mật khẩu mới không được trùng mật khẩu cũ');
    }

    await _client.post(
      '/api/profile/change-password',
      body: {
        'oldPassword': oldPassword,
        'newPassword': newPassword,
        'confirmPassword': confirmPassword,
      },
    );
  }

  Future<void> logout() => AuthService.instance.logout();
}
