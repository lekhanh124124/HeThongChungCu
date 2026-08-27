import 'dart:io';

import 'package:klks_app/core/network/api_client.dart';
import 'package:klks_app/features/cu_tru/quan_he/services/cu_tru_service.dart';
import 'package:klks_app/features/shared/services/selector_service.dart';
import 'package:klks_app/features/shared/services/upload_service.dart';

import '../models/thi_cong_model.dart';

class YeuCauThiCongService {
  YeuCauThiCongService._();
  static final YeuCauThiCongService instance = YeuCauThiCongService._();

  static final _client = ApiClient.instance;
  final _selector = SelectorService.instance;
  final _upload = UploadService.instance;

  Future<List<QuanHeCuTruModel>> getCanHoList() =>
      CuTruService.instance.getQuanHeCuTruList();

  Future<List<SelectorItem>> getTrangThaiYeuCauList() =>
      _selector.getTrangThaiYeuCau();

  Future<List<SelectorItem>> getTrangThaiThiCongList() =>
      _selector.getTrangThaiThiCong();

  Future<PagedResult<YeuCauThiCongListItem>> getList({
    int? canHoId,
    int? trangThaiId,
    int? trangThaiThiCongId,
    String? keyword,
    String sortCol = 'CreatedAt',
    bool isAsc = false,
    int pageNumber = 1,
    int pageSize = 10,
  }) async {
    final res = await _client.post(
      '/api/yeu-cau-thi-cong/get-list',
      body: {
        'canHoId': ?canHoId,
        'trangThaiId': ?trangThaiId,
        'trangThaiThiCongId': ?trangThaiThiCongId,
        if (keyword != null && keyword.isNotEmpty) 'keyword': keyword,
        'sortCol': sortCol,
        'isAsc': isAsc,
        'pageNumber': pageNumber,
        'pageSize': pageSize,
      },
    );
    return res.pagedResult(YeuCauThiCongListItem.fromJson);
  }

  Future<YeuCauThiCongDetail> getById(int id) async {
    final res = await _client.post(
      '/api/yeu-cau-thi-cong/get-by-id',
      body: {'id': id},
    );
    return res.item(YeuCauThiCongDetail.fromJson);
  }

  Future<YeuCauThiCongListItem> create({
    required int canHoId,
    required String hangMucThiCong,
    required DateTime duKienBatDau,
    required DateTime duKienKetThuc,
    required String noiDung,
    required String tenDonViThiCong,
    required String nguoiDaiDien,
    required String soDienThoaiDaiDien,
    required List<NhanSuThiCong> danhSachNhanSu,
    required List<int> danhSachTepIds,
    required bool isSubmit,
  }) async {
    final res = await _client.post(
      '/api/yeu-cau-thi-cong',
      body: {
        'canHoId': canHoId,
        'hangMucThiCong': hangMucThiCong,
        'duKienBatDau': duKienBatDau.toIso8601String(),
        'duKienKetThuc': duKienKetThuc.toIso8601String(),
        'noiDung': noiDung,
        'tenDonViThiCong': tenDonViThiCong,
        'nguoiDaiDien': nguoiDaiDien,
        'soDienThoaiDaiDien': soDienThoaiDaiDien,
        'danhSachNhanSu': danhSachNhanSu.map((e) => e.toJson()).toList(),
        'danhSachTepIds': danhSachTepIds,
        'isSubmit': isSubmit,
      },
    );
    return res.item(YeuCauThiCongListItem.fromJson);
  }

  Future<YeuCauThiCongListItem> update({
    required int id,
    required String hangMucThiCong,
    required DateTime duKienBatDau,
    required DateTime duKienKetThuc,
    required String noiDung,
    required String tenDonViThiCong,
    required String nguoiDaiDien,
    required String soDienThoaiDaiDien,
    required List<NhanSuThiCong> danhSachNhanSu,
    required List<int> danhSachTepIds,
    required bool isSubmit,
    bool isWithdraw = false,
  }) async {
    final res = await _client.put(
      '/api/yeu-cau-thi-cong',
      body: {
        'id': id,
        'hangMucThiCong': hangMucThiCong,
        'duKienBatDau': duKienBatDau.toIso8601String(),
        'duKienKetThuc': duKienKetThuc.toIso8601String(),
        'noiDung': noiDung,
        'tenDonViThiCong': tenDonViThiCong,
        'nguoiDaiDien': nguoiDaiDien,
        'soDienThoaiDaiDien': soDienThoaiDaiDien,
        'danhSachNhanSu': danhSachNhanSu.map((e) => e.toJson()).toList(),
        'danhSachTepIds': danhSachTepIds,
        'isSubmit': isSubmit,
        'isWithdraw': isWithdraw,
      },
    );
    return res.item(YeuCauThiCongListItem.fromJson);
  }

  Future<YeuCauThiCongListItem> withdraw(YeuCauThiCongDetail detail) => update(
    id: detail.id,
    hangMucThiCong: detail.hangMucThiCong,
    duKienBatDau: detail.duKienBatDau ?? DateTime.now(),
    duKienKetThuc: detail.duKienKetThuc ?? DateTime.now(),
    noiDung: detail.noiDung,
    tenDonViThiCong: detail.tenDonViThiCong,
    nguoiDaiDien: detail.nguoiDaiDien,
    soDienThoaiDaiDien: detail.soDienThoaiDaiDien,
    danhSachNhanSu: detail.nhanSuThiCongs,
    danhSachTepIds: detail.danhSachTep.map((e) => e.id).toList(),
    isSubmit: false,
    isWithdraw: true,
  );

  Future<List<UploadedFile>> uploadMedia({
    required List<File> files,
    String targetContainer = 'yeu-cau-thi-cong',
  }) => _upload.uploadMedia(files: files, targetContainer: targetContainer);
}
