class UserProfile {
  final String id;
  final String username;
  final String email;
  final String firstName;
  final String lastName;
  final String? phoneNumber;
  final String? diaChi;
  final DateTime? dob;
  final int? gioiTinhId;
  final String? gioiTinhName;
  final List<String> roles;
  final String? anhDaiDienUrl;

  const UserProfile({
    required this.id,
    required this.username,
    required this.email,
    required this.firstName,
    required this.lastName,
    required this.roles,
    this.phoneNumber,
    this.diaChi,
    this.dob,
    this.gioiTinhId,
    this.gioiTinhName,
    this.anhDaiDienUrl,
  });

  String get fullName => '$firstName $lastName'.trim();

  factory UserProfile.fromJson(Map<String, dynamic> json) => UserProfile(
    id: json['id']?.toString() ?? '',
    username: json['username'] as String? ?? '',
    email: json['email'] as String? ?? '',
    firstName: json['firstName'] as String? ?? '',
    lastName: json['lastName'] as String? ?? '',
    phoneNumber: json['phoneNumber'] as String?,
    diaChi: json['diaChi'] as String?,
    dob: json['dob'] != null ? DateTime.tryParse(json['dob'] as String) : null,
    gioiTinhId: json['gioiTinhId'] as int?,
    gioiTinhName: json['gioiTinhName'] as String?,
    roles:
        (json['roles'] as List<dynamic>?)?.map((e) => e.toString()).toList() ??
        [],
    anhDaiDienUrl: json['anhDaiDienUrl'] as String?,
  );

  Map<String, dynamic> toJson() => {
    'id': id,
    'username': username,
    'email': email,
    'firstName': firstName,
    'lastName': lastName,
    'phoneNumber': phoneNumber,
    'diaChi': diaChi,
    'dob': dob?.toIso8601String(),
    'gioiTinhId': gioiTinhId,
    'gioiTinhName': gioiTinhName,
    'roles': roles,
    'anhDaiDienUrl': anhDaiDienUrl,
  };

  UserProfile copyWith({
    String? id,
    String? username,
    String? email,
    String? firstName,
    String? lastName,
    String? phoneNumber,
    String? diaChi,
    DateTime? dob,
    int? gioiTinhId,
    String? gioiTinhName,
    List<String>? roles,
    String? anhDaiDienUrl,
  }) => UserProfile(
    id: id ?? this.id,
    username: username ?? this.username,
    email: email ?? this.email,
    firstName: firstName ?? this.firstName,
    lastName: lastName ?? this.lastName,
    phoneNumber: phoneNumber ?? this.phoneNumber,
    diaChi: diaChi ?? this.diaChi,
    dob: dob ?? this.dob,
    gioiTinhId: gioiTinhId ?? this.gioiTinhId,
    gioiTinhName: gioiTinhName ?? this.gioiTinhName,
    roles: roles ?? this.roles,
    anhDaiDienUrl: anhDaiDienUrl ?? this.anhDaiDienUrl,
  );
}
