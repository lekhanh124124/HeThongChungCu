import 'package:klks_app/core/network/api_client.dart';

import '../models/selector_item_model.dart';

class SelectorService {
  SelectorService._();

  static final SelectorService instance = SelectorService._();

  static final _client = ApiClient.instance;

  Future<List<SelectorItem>> _fetchSelector(String path) async {
    final res = await _client.post(path);
    return res.list(SelectorItem.fromJson);
  }

  Future<List<SelectorItem>> getGioiTinh() =>
      _fetchSelector('/api/catalog/gioi-tinh-for-selector');

  Future<List<SelectorItem>> getLoaiQuanHeCuTru() =>
      _fetchSelector('/api/catalog/loai-quan-he-cu-tru-for-selector');

  Future<List<SelectorItem>> getLoaiGiayTo() =>
      _fetchSelector('/api/catalog/loai-giay-to-for-selector');

  Future<List<SelectorItem>> getLoaiPhuongTien() =>
      _fetchSelector('/api/catalog/loai-phuong-tien-for-selector');

  Future<List<SelectorItem>> getLoaiDichVu() =>
      _fetchSelector('/api/catalog/loai-dich-vu-for-selector');

  Future<List<SelectorItem>> getTrangThaiDichVu() =>
      _fetchSelector('/api/catalog/trang-thai-dich-vu-for-selector');

  Future<List<SelectorItem>> getLoaiDinhGia() =>
      _fetchSelector('/api/catalog/loai-dinh-gia-for-selector');

  Future<List<SelectorItem>> getTrangThaiDangKy() =>
      _fetchSelector('/api/catalog/trang-thai-dang-ky-for-selector');

  Future<List<SelectorItem>> getNgayTrongTuan() =>
      _fetchSelector('/api/catalog/ngay-trong-tuan-for-selector');

  Future<List<SelectorItem>> getTrangThaiYeuCau() =>
      _fetchSelector('/api/catalog/trang-thai-yeu-cau-for-selector');

  Future<List<SelectorItem>> getTrangThaiSuaChua() =>
      _fetchSelector('/api/catalog/trang-thai-sua-chua-for-selector');

  Future<List<SelectorItem>> getLoaiSuCo() =>
      _fetchSelector('/api/catalog/loai-su-co-ky-thuat-for-selector');

  Future<List<SelectorItem>> getPhamViSuaChua() =>
      _fetchSelector('/api/catalog/pham-vi-sua-chua-for-selector');

  Future<List<SelectorItem>> getTrangThaiThiCong() =>
      _fetchSelector('/api/catalog/trang-thai-thi-cong-for-selector');
}
