import 'package:flutter/foundation.dart';

import 'package:klks_app/features/thong_bao/models/thong_bao_model.dart';
import 'package:klks_app/features/thong_bao/services/thong_bao_service.dart';

class HomeService extends ChangeNotifier {
  HomeService._();
  static final HomeService instance = HomeService._();

  final _thongBaoService = ThongBaoService.instance;

  // ── Thông báo chưa đọc ──────────────────────────────────────────────────────

  List<ThongBaoItem> _thongBaoMoi = [];
  List<ThongBaoItem> get thongBaoMoi => _thongBaoMoi;

  bool _isLoadingThongBao = false;
  bool get isLoadingThongBao => _isLoadingThongBao;

  String? _thongBaoError;
  String? get thongBaoError => _thongBaoError;

  // ── Public API ───────────────────────────────────────────────────────────────

  /// Tải tất cả dữ liệu cần thiết cho home screen.
  Future<void> loadAll() async {
    await _loadThongBaoMoi();
  }

  /// Tải lại riêng phần thông báo (dùng khi nhận SignalR event).
  Future<void> refreshThongBao() async {
    await _loadThongBaoMoi();
  }

  // ── Private ──────────────────────────────────────────────────────────────────

  Future<void> _loadThongBaoMoi() async {
    _isLoadingThongBao = true;
    _thongBaoError = null;
    notifyListeners();

    final result = await _thongBaoService.getList(
      pageNumber: 0,
      pageSize: 5,
      onlyUnread: true,
      sortCol: 'createdAt',
      isAsc: false,
    );

    if (result.isOk) {
      _thongBaoMoi = result.data!.items;
      _thongBaoError = null;
    } else {
      _thongBaoError = result.errorMessage;
    }

    _isLoadingThongBao = false;
    notifyListeners();
  }
}
