class UserModel {
  final int userId;
  final int accountId;
  final String username;
  final String email;
  final String anhDaiDienUrl;
  final String role;
  final String fullName;
  final String accessToken;
  final String refreshToken;

  const UserModel({
    required this.userId,
    required this.accountId,
    required this.username,
    required this.email,
    required this.anhDaiDienUrl,
    required this.role,
    required this.fullName,
    required this.accessToken,
    required this.refreshToken,
  });

  factory UserModel.fromJson(Map<String, dynamic> json) => UserModel(
    userId: json['userId'] as int? ?? 0,
    accountId: json['accountId'] as int? ?? 0,
    username: json['username'] as String? ?? '',
    email: json['email'] as String? ?? '',
    anhDaiDienUrl: json['anhDaiDienUrl'] as String? ?? '',
    role: json['role'] as String? ?? '',
    fullName: json['fullName'] as String? ?? '',
    accessToken: json['accessToken'] as String? ?? '',
    refreshToken: json['refreshToken'] as String? ?? '',
  );

  Map<String, dynamic> toJson() => {
    'userId': userId,
    'accountId': accountId,
    'username': username,
    'email': email,
    'anhDaiDienUrl': anhDaiDienUrl,
    'role': role,
    'fullName': fullName,
    'accessToken': accessToken,
    'refreshToken': refreshToken,
  };

  UserModel copyWith({
    int? userId,
    int? accountId,
    String? username,
    String? email,
    String? anhDaiDienUrl,
    String? role,
    String? fullName,
    String? accessToken,
    String? refreshToken,
  }) => UserModel(
    userId: userId ?? this.userId,
    accountId: accountId ?? this.accountId,
    username: username ?? this.username,
    email: email ?? this.email,
    anhDaiDienUrl: anhDaiDienUrl ?? this.anhDaiDienUrl,
    role: role ?? this.role,
    fullName: fullName ?? this.fullName,
    accessToken: accessToken ?? this.accessToken,
    refreshToken: refreshToken ?? this.refreshToken,
  );
}
