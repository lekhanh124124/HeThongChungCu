import 'package:klks_app/core/network/api_client.dart';

import '../models/thong_bao_model.dart';

class ServiceResult<T> {
  final T? data;
  final String? errorMessage;

  bool get isOk => errorMessage == null;

  const ServiceResult.success(this.data) : errorMessage = null;
  const ServiceResult.failure(this.errorMessage) : data = null;
}

class ThongBaoService {
  ThongBaoService._internal();
  static final ThongBaoService instance = ThongBaoService._internal();

  static final _client = ApiClient.instance;

  Future<ServiceResult<PagedResult<ThongBaoItem>>> getList({
    String keyword = '',
    int pageNumber = 0,
    int pageSize = 20,
    bool onlyUnread = false,
    String sortCol = 'createdAt',
    bool isAsc = false,
  }) async {
    try {
      final res = await _client.post(
        '/api/thong-bao/get-list',
        body: {
          'keyword': keyword,
          'sortCol': sortCol,
          'isAsc': isAsc,
          'pageNumber': pageNumber,
          'pageSize': pageSize,
          'onlyUnread': onlyUnread,
        },
      );
      return ServiceResult.success(res.pagedResult(ThongBaoItem.fromJson));
    } on AppException catch (e) {
      return ServiceResult.failure(e.message);
    } catch (e) {
      return ServiceResult.failure('Lỗi không xác định: $e');
    }
  }

  Future<ServiceResult<bool>> daDoc({required int phanBoThongBaoId}) async {
    try {
      await _client.put(
        '/api/thong-bao/da-doc',
        body: {'phanBoThongBaoId': phanBoThongBaoId},
      );
      return const ServiceResult.success(true);
    } on AppException catch (e) {
      return ServiceResult.failure(e.message);
    } catch (e) {
      return ServiceResult.failure('Lỗi không xác định: $e');
    }
  }
}
