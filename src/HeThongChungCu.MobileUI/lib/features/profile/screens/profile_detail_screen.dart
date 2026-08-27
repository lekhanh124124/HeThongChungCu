import 'package:flutter/material.dart';

import 'package:klks_app/core/storage/user_session.dart';
import 'package:klks_app/design/design.dart';

import '../models/user_profile.dart';
import '../services/profile_service.dart';

class ProfileDetailScreen extends StatefulWidget {
  const ProfileDetailScreen({super.key});

  @override
  State<ProfileDetailScreen> createState() => _ProfileDetailScreenState();
}

class _ProfileDetailScreenState extends State<ProfileDetailScreen> {
  UserProfile? _profile;
  bool _loading = true;
  Object? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final data = await ProfileService.instance.getProfile();
      if (mounted) setState(() => _profile = data);
    } catch (e) {
      if (mounted) setState(() => _error = e);
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: 'Chi tiết hồ sơ',
      actions: [IconButton(icon: const Icon(Icons.refresh), onPressed: _load)],
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_loading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_error != null) {
      return ErrorDisplay.fullScreen(error: _error, onRetry: _load);
    }

    final p = _profile!;

    return ListView(
      padding: AppSpacing.insetAll16,
      children: [
        Center(
          child: ValueListenableBuilder(
            valueListenable: UserSession.instance.anhDaiDienUrlNotifier,
            builder: (context, url, _) => CircleAvatar(
              radius: 52,
              backgroundColor: context.colorScheme.primaryContainer,
              backgroundImage: url != null ? NetworkImage(url) : null,
              child: url == null
                  ? Icon(
                      Icons.person,
                      size: 52,
                      color: context.colorScheme.primary,
                    )
                  : null,
            ),
          ),
        ),
        const SizedBox(height: AppSpacing.xl),

        AppCard(
          child: Column(
            children: [
              _InfoRow(label: 'Họ', value: p.lastName),
              _Divider(),
              _InfoRow(label: 'Tên', value: p.firstName),
              _Divider(),
              _InfoRow(label: 'Tên đăng nhập', value: p.username),
              _Divider(),
              _InfoRow(label: 'Email', value: p.email),
              _Divider(),
              _InfoRow(label: 'Số điện thoại', value: p.phoneNumber ?? '—'),
              _Divider(),
              _InfoRow(label: 'Địa chỉ', value: p.diaChi ?? '—'),
              _Divider(),
              _InfoRow(label: 'Giới tính', value: p.gioiTinhName ?? '—'),
              _Divider(),
              _InfoRow(
                label: 'Ngày sinh',
                value: p.dob != null
                    ? '${p.dob!.day.toString().padLeft(2, '0')}/'
                          '${p.dob!.month.toString().padLeft(2, '0')}/'
                          '${p.dob!.year}'
                    : '—',
              ),
              _Divider(),
              _InfoRow(label: 'Vai trò', value: p.roles.join(', ')),
            ],
          ),
        ),
      ],
    );
  }
}

class _InfoRow extends StatelessWidget {
  final String label;
  final String value;

  const _InfoRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.sm),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 130,
            child: Text(
              label,
              style: context.textTheme.bodyMedium?.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: context.textTheme.bodyMedium?.copyWith(
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _Divider extends StatelessWidget {
  @override
  Widget build(BuildContext context) => const Divider(height: 1);
}
