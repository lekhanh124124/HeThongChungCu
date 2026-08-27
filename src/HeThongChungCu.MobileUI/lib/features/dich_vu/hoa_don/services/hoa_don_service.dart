import 'package:klks_app/core/network/api_client.dart';
import 'package:klks_app/features/cu_tru/quan_he/services/cu_tru_service.dart';

import '../models/hoa_don_model.dart';

class HoaDonService {
  HoaDonService._();
  static final instance = HoaDonService._();

  static final _client = ApiClient.instance;

  Future<List<QuanHeCuTruModel>> getCanHoList() =>
      CuTruService.instance.getQuanHeCuTruList();

  Future<PagedResult<HoaDon>> getList({
    required int canHoId,
    int? trangThaiHoaDonId,
    int? thang,
    int? nam,
    String? keyword,
    int pageNumber = 1,
    int pageSize = 10,
  }) async {
    final res = await _client.post(
      '/api/hoa-don/get-list',
      body: {
        'canHoId': canHoId,
        'trangThaiHoaDonId': trangThaiHoaDonId,
        'thang': thang,
        'nam': nam,
        if (keyword != null && keyword.isNotEmpty) 'keyword': keyword,
        'pageNumber': pageNumber,
        'pageSize': pageSize,
        'isAsc': false,
      },
    );
    return res.pagedResult(HoaDon.fromJson);
  }

  Future<HoaDonDetail> getById(int id) async {
    final res = await _client.post('/api/hoa-don/get-by-id', body: {'id': id});
    return res.item(HoaDonDetail.fromJson);
  }

  Future<ChiTietCoDinh> getChiTietCoDinh(int chiTietId) async {
    final res = await _client.post(
      '/api/hoa-don/get-chi-tiet-co-dinh',
      body: {'id': chiTietId},
    );
    return res.item(ChiTietCoDinh.fromJson);
  }

  Future<ChiTietLuyTien> getChiTietLuyTien(int chiTietId) async {
    final res = await _client.post(
      '/api/hoa-don/get-chi-tiet-luy-tien',
      body: {'id': chiTietId},
    );
    return res.item(ChiTietLuyTien.fromJson);
  }

  Future<ChiTietDienTich> getChiTietDienTich(int chiTietId) async {
    final res = await _client.post(
      '/api/hoa-don/get-chi-tiet-dien-tich',
      body: {'id': chiTietId},
    );
    return res.item(ChiTietDienTich.fromJson);
  }

  Future<ChiTietKhungGio> getChiTietKhungGio(int chiTietId) async {
    final res = await _client.post(
      '/api/hoa-don/get-chi-tiet-khung-gio',
      body: {'id': chiTietId},
    );
    return res.item(ChiTietKhungGio.fromJson);
  }

  Future<PhienThanhToan> taoPhienThanhToan({
    required int hoaDonId,
    List<int> chiTietHoaDonIds = const [],
  }) async {
    final res = await _client.post(
      '/api/giao-dich-thanh-toan/tao-phien',
      body: {'hoaDonId': hoaDonId, 'chiTietHoaDonIds': chiTietHoaDonIds},
    );
    return res.item(PhienThanhToan.fromJson);
  }
}
