import 'package:klks_app/core/network/api_client.dart';

import '../models/quan_he_cu_tru_model.dart';

class CuTruService {
  CuTruService._();
  static final CuTruService instance = CuTruService._();

  static final _client = ApiClient.instance;

  Future<List<QuanHeCuTruModel>> getQuanHeCuTruList() async {
    final res = await _client.post('/api/cu-dan/quan-he-cu-tru');
    return res.list(QuanHeCuTruModel.fromJson);
  }
}
