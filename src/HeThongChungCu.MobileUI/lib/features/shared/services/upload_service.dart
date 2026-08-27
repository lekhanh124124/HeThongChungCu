import 'dart:io';
import 'package:dio/dio.dart';

import 'package:klks_app/core/network/api_client.dart';

import '../models/file_model.dart';

class UploadService {
  UploadService._();

  static final UploadService instance = UploadService._();

  static final _client = ApiClient.instance;

  Future<List<UploadedFile>> uploadMedia({
    required List<File> files,
    required String targetContainer,
  }) async {
    final formData = FormData()
      ..fields.add(MapEntry('targetContainer', targetContainer));
    for (final file in files) {
      formData.files.add(
        MapEntry(
          'files',
          await MultipartFile.fromFile(
            file.path,
            filename: file.path.split('/').last,
          ),
        ),
      );
    }
    final res = await _client.postForm('/api/upload-media', formData);
    return res.list(UploadedFile.fromJson);
  }
}
