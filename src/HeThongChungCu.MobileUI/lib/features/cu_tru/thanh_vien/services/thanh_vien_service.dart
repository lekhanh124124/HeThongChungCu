import 'dart:io';

import 'package:klks_app/core/network/api_client.dart';

import 'package:klks_app/features/shared/services/upload_service.dart';
import 'package:klks_app/features/shared/services/selector_service.dart';

import '../models/thanh_vien_model.dart';

class ThanhVienService {
  ThanhVienService._();
  static final ThanhVienService instance = ThanhVienService._();

  static final _client = ApiClient.instance;

  static final _upload = UploadService.instance;

  static final _selector = SelectorService.instance;

  Future<List<ThanhVienCuTruModel>> getThanhVienCuTru(int canHoId) async {
    final res = await _client.post(
      '/api/cu-dan/thanh-vien-cu-tru',
      body: {'canHoId': canHoId},
    );
    return res.list(ThanhVienCuTruModel.fromJson);
  }

  Future<ThongTinCuDanModel> getThongTinCuDan(int quanHeCuTruId) async {
    final res = await _client.post(
      '/api/cu-dan/thong-tin',
      body: {'quanHeCuTruId': quanHeCuTruId},
    );
    return res.item(ThongTinCuDanModel.fromJson);
  }

  Future<PagedResult<YeuCauCuTruModel>> getYeuCauList(
    GetListYeuCauCuTruRequest request,
  ) async {
    final res = await _client.post(
      '/api/quan-he-cu-tru/yeu-cau/get-list',
      body: request.toJson(),
    );
    return res.pagedResult(YeuCauCuTruModel.fromJson);
  }

  Future<YeuCauCuTruModel> getYeuCauById(int requestId) async {
    final res = await _client.post(
      '/api/quan-he-cu-tru/yeu-cau/get-by-id',
      body: {'requestId': requestId},
    );
    return res.item(YeuCauCuTruModel.fromJson);
  }

  Future<YeuCauCuTruModel> createYeuCau(TaoYeuCauCuTruRequest request) async {
    final res = await _client.post(
      '/api/quan-he-cu-tru/yeu-cau',
      body: request.toJson(),
    );
    return res.item(YeuCauCuTruModel.fromJson);
  }

  Future<YeuCauCuTruModel> updateYeuCau(
    CapNhatYeuCauCuTruRequest request,
  ) async {
    final res = await _client.put(
      '/api/quan-he-cu-tru/yeu-cau',
      body: request.toJson(),
    );
    return res.item(YeuCauCuTruModel.fromJson);
  }

  Future<List<UploadedFile>> uploadMedia({
    required List<File> files,
    String targetContainer = 'tai-lieu-cu-tru',
  }) => _upload.uploadMedia(files: files, targetContainer: targetContainer);

  Future<List<SelectorItem>> getGioiTinhSelector() => _selector.getGioiTinh();

  Future<List<SelectorItem>> getLoaiQuanHeCuTruSelector() =>
      _selector.getLoaiQuanHeCuTru();

  Future<List<SelectorItem>> getLoaiGiayToSelector() =>
      _selector.getLoaiGiayTo();
}
