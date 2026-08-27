import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import 'package:klks_app/design/design.dart';

class DichVuScreen extends StatelessWidget {
  const DichVuScreen({super.key});

  static const _items = [
    _DichVuMeta(
      icon: Icons.miscellaneous_services_outlined,
      label: 'Tiện ích',
      description: 'Đăng ký và quản lý dịch vụ tiện ích',
      route: '/dich-vu/tien-ich',
    ),
    _DichVuMeta(
      icon: Icons.build_outlined,
      label: 'Sửa chữa',
      description: 'Yêu cầu sửa chữa thiết bị, hạ tầng',
      route: '/dich-vu/sua-chua',
    ),
    _DichVuMeta(
      icon: Icons.construction_outlined,
      label: 'Thi công',
      description: 'Yêu cầu thi công, cải tạo',
      route: '/dich-vu/thi-cong',
    ),
    _DichVuMeta(
      icon: Icons.receipt_long_outlined,
      label: 'Hóa đơn',
      description: 'Xem và thanh toán hóa đơn',
      route: '/dich-vu/hoa-don',
    ),
    _DichVuMeta(
      icon: Icons.campaign_outlined,
      label: 'Phản ánh',
      description: 'Gửi phản ánh, góp ý',
      route: '/dich-vu/phan-anh',
    ),
    _DichVuMeta(
      icon: Icons.poll_outlined,
      label: 'Khảo sát',
      description: 'Tham gia khảo sát cư dân',
      route: '/dich-vu/khao-sat',
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: 'Tiện ích',
      showAppBar: true,
      body: ListView.separated(
        padding: AppSpacing.insetAll16,
        itemCount: _items.length,
        separatorBuilder: (_, _) => const SizedBox(height: AppSpacing.sm),
        itemBuilder: (_, i) => AppServiceCard(
          icon: _items[i].icon,
          title: _items[i].label,
          subtitle: _items[i].description,
          onTap: () => context.push(_items[i].route),
        ),
      ),
    );
  }
}

class _DichVuMeta {
  final IconData icon;
  final String label;
  final String description;
  final String route;

  const _DichVuMeta({
    required this.icon,
    required this.label,
    required this.description,
    required this.route,
  });
}
