
import 'dart:io';

import 'package:klks_app/core/network/api_client.dart';
import 'package:klks_app/features/shared/models/paging_model.dart';
import 'package:klks_app/features/shared/services/selector_service.dart';
import 'package:klks_app/features/shared/services/upload_service.dart';

import '../models/phuong_tien_model.dart';

class PhuongTienService {
  PhuongTienService._();
  static final instance = PhuongTienService._();

  static final _client = ApiClient.instance;

  static final _upload = UploadService.instance;

  static final _selector = SelectorService.instance;

  Future<PagedResult<PhuongTien>> getListPhuongTien(
    GetListPhuongTienRequest request,
  ) async {
    final res = await _client.post(
      '/api/phuong-tien/get-list',
      body: request.toJson(),
    );
    return res.pagedResult(PhuongTien.fromJson);
  }

  Future<PhuongTien> getPhuongTienById(int id) async {
    final res = await _client.post(
      '/api/phuong-tien/get-by-id',
      body: {'id': id},
    );
    return res.item(PhuongTien.fromJson);
  }

  Future<PagedResult<YeuCauPhuongTien>> getListYeuCau(
    GetListYeuCauPhuongTienRequest request,
  ) async {
    final res = await _client.post(
      '/api/phuong-tien/yeu-cau/get-list',
      body: request.toJson(),
    );
    return res.pagedResult(YeuCauPhuongTien.fromJson);
  }

  Future<YeuCauPhuongTien> getYeuCauById(int requestId) async {
    final res = await _client.post(
      '/api/phuong-tien/yeu-cau/get-by-id',
      body: {'requestId': requestId},
    );
    return res.item(YeuCauPhuongTien.fromJson);
  }

  Future<YeuCauPhuongTien> taoYeuCau(TaoYeuCauPhuongTienRequest request) async {
    final res = await _client.post(
      '/api/phuong-tien/yeu-cau',
      body: request.toJson(),
    );
    return res.item(YeuCauPhuongTien.fromJson);
  }

  Future<YeuCauPhuongTien> capNhatYeuCau(
    CapNhatYeuCauPhuongTienRequest request,
  ) async {
    final res = await _client.put(
      '/api/phuong-tien/yeu-cau',
      body: request.toJson(),
    );
    return res.item(YeuCauPhuongTien.fromJson);
  }

  Future<void> baoMatThe(List<int> theIds) async {
    await _client.put(
      '/api/phuong-tien/the-phuong-tien/bao-mat',
      body: {'theIds': theIds},
    );
  }

  Future<List<UploadedFile>> uploadMedia({
    required List<File> files,
    String targetContainer = 'tai-lieu-phuong-tien',
  }) => _upload.uploadMedia(files: files, targetContainer: targetContainer);

  Future<List<SelectorItem>> getLoaiPhuongTienSelector() =>
      _selector.getLoaiPhuongTien();
}
