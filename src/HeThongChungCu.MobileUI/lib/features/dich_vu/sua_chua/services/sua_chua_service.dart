import 'dart:io';

import 'package:klks_app/core/network/api_client.dart';
import 'package:klks_app/features/shared/services/selector_service.dart';
import 'package:klks_app/features/shared/services/upload_service.dart';

import '../models/sua_chua_model.dart';

class YeuCauSuaChuaService {
  YeuCauSuaChuaService._();
  static final YeuCauSuaChuaService instance = YeuCauSuaChuaService._();

  static final _client = ApiClient.instance;
  final _selector = SelectorService.instance;
  final _upload = UploadService.instance;

  Future<List<SelectorItem>> getTrangThaiYeuCau() =>
      _selector.getTrangThaiYeuCau();

  Future<List<SelectorItem>> getTrangThaiSuaChua() =>
      _selector.getTrangThaiSuaChua();

  Future<List<SelectorItem>> getLoaiSuCo() => _selector.getLoaiSuCo();

  Future<List<SelectorItem>> getPhamViSuaChua() => _selector.getPhamViSuaChua();

  Future<List<UploadedFile>> uploadMedia({
    required List<File> files,
    String targetContainer = 'yeu-cau-sua-chua',
  }) => _upload.uploadMedia(files: files, targetContainer: targetContainer);

  Future<PagedResult<YeuCauSuaChua>> getList(
    GetListYeuCauRequest request,
  ) async {
    final res = await _client.post(
      '/api/yeu-cau-sua-chua/get-list',
      body: request.toJson(),
    );
    return res.pagedResult(YeuCauSuaChua.fromJson);
  }

  Future<YeuCauSuaChua> getById(int id) async {
    final res = await _client.post(
      '/api/yeu-cau-sua-chua/get-by-id',
      body: {'id': id},
    );
    return res.item(YeuCauSuaChua.fromJson);
  }

  Future<YeuCauSuaChua> taoYeuCau(TaoYeuCauRequest request) async {
    final res = await _client.post(
      '/api/yeu-cau-sua-chua',
      body: request.toJson(),
    );
    return res.item(YeuCauSuaChua.fromJson);
  }

  Future<YeuCauSuaChua> capNhatYeuCau(CapNhatYeuCauRequest request) async {
    final res = await _client.put(
      '/api/yeu-cau-sua-chua',
      body: request.toJson(),
    );
    return res.item(YeuCauSuaChua.fromJson);
  }

  Future<YeuCauSuaChua> thuHoiYeuCau({
    required int id,
    required int phamViId,
    required int loaiSuCoId,
    required String noiDung,
  }) => capNhatYeuCau(
    CapNhatYeuCauRequest(
      id: id,
      phamViId: phamViId,
      loaiSuCoId: loaiSuCoId,
      noiDung: noiDung,
      isSubmit: false,
      isWithdraw: true,
    ),
  );
}
