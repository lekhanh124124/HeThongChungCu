import 'package:klks_app/core/network/api_client.dart';

import '../models/chat_model.dart';

class ChatService {
  ChatService._();
  static final ChatService instance = ChatService._();

  static final _client = ApiClient.instance;

  Future<ChatResponse> chat({
    required String prompt,
    required List<ChatMessage> history,
    String? documentType,
    int limit = 5,
  }) async {
    final res = await _client.post(
      '/api/ai/chat',
      body: {
        'prompt': prompt,
        'history': history.map((m) => m.toJson()).toList(),
        'documentType': ?documentType,
        'limit': limit,
      },
    );
    return res.item(ChatResponse.fromJson);
  }
}
