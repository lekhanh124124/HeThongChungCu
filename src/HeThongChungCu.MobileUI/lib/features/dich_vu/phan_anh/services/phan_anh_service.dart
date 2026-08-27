import 'dart:io';

import 'package:klks_app/core/network/api_client.dart';
import 'package:klks_app/features/cu_tru/quan_he/services/cu_tru_service.dart';
import 'package:klks_app/features/shared/services/upload_service.dart';

import '../models/phan_anh_model.dart';

class PhanAnhService {
  PhanAnhService._();
  static final PhanAnhService instance = PhanAnhService._();

  static final _client = ApiClient.instance;

  static final _upload = UploadService.instance;

  Future<List<QuanHeCuTruModel>> getCanHoList() =>
      CuTruService.instance.getQuanHeCuTruList();

  Future<PagedResult<PhanAnhResponse>> getList({
    int? canHoId,
    int? trangThaiPhanAnhId,
    int? loaiPhanAnhId,
    int? nguoiXuLyId,
    String? keyword,
    DateTime? ngayTaoTu,
    DateTime? ngayTaoDen,
    String sortCol = 'CreatedAt',
    bool isAsc = false,
    int pageNumber = 1,
    int pageSize = 10,
  }) async {
    final res = await _client.post(
      '/api/phan-anh/get-list',
      body: {
        'canHoId': ?canHoId,
        'trangThaiPhanAnhId': ?trangThaiPhanAnhId,
        'loaiPhanAnhId': ?loaiPhanAnhId,
        'nguoiXuLyId': ?nguoiXuLyId,
        if (keyword != null && keyword.isNotEmpty) 'keyword': keyword,
        if (ngayTaoTu != null) 'ngayTaoTu': ngayTaoTu.toIso8601String(),
        if (ngayTaoDen != null) 'ngayTaoDen': ngayTaoDen.toIso8601String(),
        'sortCol': sortCol,
        'isAsc': isAsc,
        'pageNumber': pageNumber,
        'pageSize': pageSize,
      },
    );
    return res.pagedResult(PhanAnhResponse.fromJson);
  }

  Future<PhanAnhDetailResponse> getById(int id) async {
    final res = await _client.post('/api/phan-anh/get-by-id', body: {'id': id});
    return res.item(PhanAnhDetailResponse.fromJson);
  }

  Future<PhanAnhResponse> create({
    required int canHoId,
    required String tieuDe,
    required String noiDung,
    required int loaiPhanAnhId,
    List<int> danhSachTepIds = const [],
    bool isSubmit = false,
  }) async {
    final res = await _client.post(
      '/api/phan-anh',
      body: {
        'canHoId': canHoId,
        'tieuDe': tieuDe,
        'noiDung': noiDung,
        'loaiPhanAnhId': loaiPhanAnhId,
        'danhSachTepIds': danhSachTepIds,
        'isSubmit': isSubmit,
      },
    );
    return res.item(PhanAnhResponse.fromJson);
  }

  Future<PhanAnhResponse> update({
    required int id,
    required String tieuDe,
    required String noiDung,
    required int loaiPhanAnhId,
    List<int> danhSachTepIds = const [],
    bool isSubmit = false,
    bool isWithdraw = false,
  }) async {
    final res = await _client.put(
      '/api/phan-anh',
      body: {
        'id': id,
        'tieuDe': tieuDe,
        'noiDung': noiDung,
        'loaiPhanAnhId': loaiPhanAnhId,
        'danhSachTepIds': danhSachTepIds,
        'isSubmit': isSubmit,
        'isWithdraw': isWithdraw,
      },
    );
    return res.item(PhanAnhResponse.fromJson);
  }

  Future<PhanAnhResponse> withdraw(int id) => update(
    id: id,
    tieuDe: '',
    noiDung: '',
    loaiPhanAnhId: 0,
    isWithdraw: true,
  );

  Future<PhanAnhResponse> submitTraLoi({
    required int phanAnhId,
    required String noiDung,
  }) async {
    final res = await _client.post(
      '/api/phan-anh/submit-tra-loi',
      body: {'phanAnhId': phanAnhId, 'noiDung': noiDung},
    );
    return res.item(PhanAnhResponse.fromJson);
  }

  Future<PhanAnhResponse> danhGia({
    required int phanAnhId,
    required int diemDanhGia,
    String nhanXetDanhGia = '',
  }) async {
    final res = await _client.put(
      '/api/phan-anh/danh-gia',
      body: {
        'phanAnhId': phanAnhId,
        'diemDanhGia': diemDanhGia,
        'nhanXetDanhGia': nhanXetDanhGia,
      },
    );
    return res.item(PhanAnhResponse.fromJson);
  }

  Future<List<UploadedFile>> uploadMedia({
    required List<File> files,
    required String targetContainer,
  }) => _upload.uploadMedia(files: files, targetContainer: targetContainer);
}
