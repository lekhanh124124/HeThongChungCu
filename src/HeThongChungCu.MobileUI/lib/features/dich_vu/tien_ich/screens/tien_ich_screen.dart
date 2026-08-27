import 'package:flutter/material.dart';

import 'package:klks_app/design/design.dart';

import 'tien_ich_list_screen.dart';
import 'dang_ky_list_screen.dart';

class TienIchScreen extends StatelessWidget {
  const TienIchScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
      length: 2,
      child: AppScaffold(
        appBar: AppTopBar(
          title: 'Dịch vụ tiện ích',
          bottom: const TabBar(
            tabs: [
              Tab(text: 'Danh sách'),
              Tab(text: 'Lịch sử'),
            ],
          ),
        ),
        body: const TabBarView(
          children: [TienIchListScreen(), DangKyListScreen()],
        ),
      ),
    );
  }
}
