class SelectorItem {
  final int id;
  final String name;

  const SelectorItem({required this.id, required this.name});

  factory SelectorItem.fromJson(Map<String, dynamic> json) => SelectorItem(
    id: json['id'] as int? ?? 0,
    name: json['name'] as String? ?? '',
  );

  Map<String, dynamic> toJson() => {'id': id, 'name': name};

  SelectorItem copyWith({int? id, String? name}) =>
      SelectorItem(id: id ?? this.id, name: name ?? this.name);

  @override
  bool operator ==(Object other) => other is SelectorItem && other.id == id;

  @override
  int get hashCode => id.hashCode;

  @override
  String toString() => name;
}
