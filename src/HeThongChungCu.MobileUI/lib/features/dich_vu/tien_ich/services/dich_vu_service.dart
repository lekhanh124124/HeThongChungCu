import 'package:klks_app/core/network/api_client.dart';
import 'package:klks_app/features/cu_tru/quan_he/services/cu_tru_service.dart';

import '../models/dich_vu_model.dart';

abstract final class DichVuCatalog {
  static const int loaiDichVu = 3;
  static const int trangThaiDichVu = 1;

  static const loaiDinhGia = [
    SelectorItem(id: 1, name: 'Cố định'),
    SelectorItem(id: 2, name: 'Lũy tiến'),
    SelectorItem(id: 6, name: 'Theo diện tích'),
    SelectorItem(id: 7, name: 'Theo khung giờ'),
  ];

  static const trangThaiDangKy = [
    SelectorItem(id: 1, name: 'Chờ duyệt'),
    SelectorItem(id: 2, name: 'Đang sử dụng'),
    SelectorItem(id: 3, name: 'Tạm ngưng'),
    SelectorItem(id: 4, name: 'Đã hủy'),
  ];

  static const ngayTrongTuan = [
    SelectorItem(id: 0, name: 'Chủ Nhật'),
    SelectorItem(id: 1, name: 'Thứ Hai'),
    SelectorItem(id: 2, name: 'Thứ Ba'),
    SelectorItem(id: 3, name: 'Thứ Tư'),
    SelectorItem(id: 4, name: 'Thứ Năm'),
    SelectorItem(id: 5, name: 'Thứ Sáu'),
    SelectorItem(id: 6, name: 'Thứ Bảy'),
  ];

  static String trangThaiDangKyName(int id) => trangThaiDangKy
      .firstWhere(
        (e) => e.id == id,
        orElse: () => const SelectorItem(id: 0, name: 'Không xác định'),
      )
      .name;

  static String loaiDinhGiaName(int id) => loaiDinhGia
      .firstWhere(
        (e) => e.id == id,
        orElse: () => const SelectorItem(id: 0, name: 'Không xác định'),
      )
      .name;

  static String ngayTrongTuanName(int id) => ngayTrongTuan
      .firstWhere(
        (e) => e.id == id,
        orElse: () => const SelectorItem(id: 0, name: '?'),
      )
      .name;
}

class SelectorItem {
  final int id;
  final String name;
  const SelectorItem({required this.id, required this.name});
}

class DichVuService {
  DichVuService._();
  static final DichVuService instance = DichVuService._();

  static final _client = ApiClient.instance;

  Future<List<QuanHeCuTruModel>> getCanHoList() =>
      CuTruService.instance.getQuanHeCuTruList();

  Future<PagedResult<DichVuItem>> getDichVuList({
    String? keyword,
    int pageNumber = 1,
    int pageSize = 10,
  }) async {
    final res = await _client.post(
      '/api/dich-vu/get-list',
      body: {
        'loaiDichVuId': DichVuCatalog.loaiDichVu,
        'trangThaiDichVuId': DichVuCatalog.trangThaiDichVu,
        'isBatBuoc': false,
        if (keyword != null && keyword.isNotEmpty) 'keyword': keyword,
        'pageNumber': pageNumber,
        'pageSize': pageSize,
      },
    );
    return res.pagedResult(DichVuItem.fromJson);
  }

  Future<DichVuDetail> getDichVuById(int id) async {
    final res = await _client.post('/api/dich-vu/get-by-id', body: {'id': id});
    return res.item(DichVuDetail.fromJson);
  }

  Future<PagedResult<DichVuDangKyItem>> getDanhSachDangKy(
    DichVuDangKyRequest request,
  ) async {
    final res = await _client.post(
      '/api/dich-vu/dang-ky/get-list',
      body: request.toJson(),
    );
    return res.pagedResult(DichVuDangKyItem.fromJson);
  }

  Future<int> dangKyDichVu({
    required int canHoId,
    required int dichVuId,
    required DateTime ngaySuDung,
    int soLuong = 1,
    int? khungGioId,
  }) async {
    final res = await _client.post(
      '/api/dich-vu/dang-ky',
      body: {
        'canHoId': canHoId,
        'dichVuId': dichVuId,
        'ngaySuDung': ngaySuDung.toIso8601String(),
        'soLuong': soLuong,
        'khungGioId': ?khungGioId,
      },
    );
    return res.raw<int>();
  }
}
