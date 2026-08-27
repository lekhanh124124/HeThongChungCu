import 'package:klks_app/core/network/api_client.dart';
import 'package:klks_app/core/storage/user_session.dart';
import 'package:klks_app/features/cu_tru/quan_he/services/cu_tru_service.dart';

import '../models/khao_sat_model.dart';

class KhaoSatService {
  KhaoSatService._();
  static final KhaoSatService instance = KhaoSatService._();

  static final _client = ApiClient.instance;

  Future<List<QuanHeCuTruModel>> getCanHoList() =>
      CuTruService.instance.getQuanHeCuTruList();

  String? getNguoiDungID() => UserSession.instance.userId;

  Future<PagedResult<KhaoSatResponse>> getList({
    int? trangThaiId,
    int? loaiKhaoSatId,
    String? keyword,
    DateTime? ngayTaoTu,
    DateTime? ngayTaoDen,
    String sortCol = 'CreatedAt',
    bool isAsc = false,
    int pageNumber = 1,
    int pageSize = 10,
  }) async {
    final res = await _client.post(
      '/api/khao-sat/get-list',
      body: {
        'trangThaiId': ?trangThaiId,
        'loaiKhaoSatId': ?loaiKhaoSatId,
        if (keyword != null && keyword.isNotEmpty) 'keyword': keyword,
        if (ngayTaoTu != null) 'ngayTaoTu': ngayTaoTu.toIso8601String(),
        if (ngayTaoDen != null) 'ngayTaoDen': ngayTaoDen.toIso8601String(),
        'sortCol': sortCol,
        'isAsc': isAsc,
        'pageNumber': pageNumber,
        'pageSize': pageSize,
      },
    );
    return res.pagedResult(KhaoSatResponse.fromJson);
  }

  Future<KhaoSatDetailResponse> getById(int id) async {
    final res = await _client.post('/api/khao-sat/get-by-id', body: {'id': id});
    return res.item(KhaoSatDetailResponse.fromJson);
  }

  Future<String> guiOtpBieuQuyet({
    required int khaoSatId,
    required int canHoId,
    required int nguoiDungId,
  }) async {
    final res = await _client.post(
      '/api/khao-sat/gui-otp-bieu-quyet',
      body: {
        'khaoSatId': khaoSatId,
        'canHoId': canHoId,
        'nguoiDungId': nguoiDungId,
      },
    );
    return res.raw<String>();
  }

  Future<bool> xacNhanBieuQuyet(XacNhanBieuQuyetRequest request) async {
    final res = await _client.post(
      '/api/khao-sat/xac-nhan-bieu-quyet',
      body: request.toJson(),
    );
    return res.raw<bool>();
  }

  Future<KetQuaKhaoSatResponse> getKetQua(int id) async {
    final res = await _client.post(
      '/api/khao-sat/get-ket-qua',
      body: {'id': id},
    );
    return res.item(KetQuaKhaoSatResponse.fromJson);
  }
}
