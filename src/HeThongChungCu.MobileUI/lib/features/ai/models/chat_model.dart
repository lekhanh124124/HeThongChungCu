class ChatMessage {
  final String role;
  final String content;

  const ChatMessage({required this.role, required this.content});

  factory ChatMessage.user(String content) =>
      ChatMessage(role: 'user', content: content);

  factory ChatMessage.assistant(String content) =>
      ChatMessage(role: 'assistant', content: content);

  Map<String, dynamic> toJson() => {'role': role, 'content': content};
}

class ChatSource {
  final String source;
  final String? h1;
  final String? h2;
  final String? h3;
  final double score;

  const ChatSource({
    required this.source,
    this.h1,
    this.h2,
    this.h3,
    required this.score,
  });

  String get displayTitle {
    if (h2 != null && h2!.isNotEmpty) return h2!;
    if (h1 != null && h1!.isNotEmpty) return h1!;
    return source;
  }

  factory ChatSource.fromJson(Map<String, dynamic> json) => ChatSource(
    source: json['source'] as String? ?? '',
    h1: json['h1'] as String?,
    h2: json['h2'] as String?,
    h3: json['h3'] as String?,
    score: (json['score'] as num?)?.toDouble() ?? 0,
  );
}

class ChatResponse {
  final String answer;
  final List<ChatSource> sources;
  final bool isCondensed;

  const ChatResponse({
    required this.answer,
    required this.sources,
    required this.isCondensed,
  });

  factory ChatResponse.fromJson(Map<String, dynamic> json) => ChatResponse(
    answer: json['answer'] as String? ?? '',
    sources: (json['sources'] as List<dynamic>? ?? [])
        .map((e) => ChatSource.fromJson(e as Map<String, dynamic>))
        .toList(),
    isCondensed: json['isCondensed'] as bool? ?? false,
  );
}
