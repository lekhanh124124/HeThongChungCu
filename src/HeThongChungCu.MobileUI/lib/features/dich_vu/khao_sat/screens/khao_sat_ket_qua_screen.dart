import 'package:flutter/material.dart';
import 'package:klks_app/design/design.dart';

import '../models/khao_sat_model.dart';
import '../services/khao_sat_service.dart';

class KhaoSatKetQuaScreen extends StatefulWidget {
  final int khaoSatId;

  const KhaoSatKetQuaScreen({super.key, required this.khaoSatId});

  @override
  State<KhaoSatKetQuaScreen> createState() => _KhaoSatKetQuaScreenState();
}

class _KhaoSatKetQuaScreenState extends State<KhaoSatKetQuaScreen> {
  final _service = KhaoSatService.instance;

  KetQuaKhaoSatResponse? _ketQua;
  bool _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final result = await _service.getKetQua(widget.khaoSatId);
      if (!mounted) return;
      setState(() {
        _ketQua = result;
        _isLoading = false;
      });
    } on Exception catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.toString();
        _isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppScaffold(
      title: 'Kết quả khảo sát',
      actions: [
        IconButton(
          icon: const Icon(Icons.refresh),
          tooltip: 'Làm mới',
          onPressed: _load,
        ),
      ],
      body: _buildBody(),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_error != null) {
      return ErrorDisplay.fullScreen(error: _error!, onRetry: _load);
    }

    final k = _ketQua!;
    return RefreshIndicator(
      onRefresh: _load,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Text(k.tieuDeKhaoSat, style: AppTypography.headline),
          const SizedBox(height: 4),
          Text(
            k.coCheTinhDiemTen,
            style: AppTypography.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
          const SizedBox(height: 16),

          _TongQuanCard(ketQua: k),
          const SizedBox(height: 16),

          _HieuLucCard(isHieuLuc: k.isHieuLuc),
          const SizedBox(height: 20),

          ...k.ketQuaCauHois.asMap().entries.map(
            (entry) => Padding(
              padding: const EdgeInsets.only(bottom: 16),
              child: _KetQuaCauHoiCard(
                index: entry.key + 1,
                data: entry.value,
                isAreaBased: k.coCheTinhDiemId == 2,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _TongQuanCard extends StatelessWidget {
  final KetQuaKhaoSatResponse ketQua;
  const _TongQuanCard({required this.ketQua});

  @override
  Widget build(BuildContext context) {
    final k = ketQua;
    final tyLe = k.tyLeThamGia.clamp(0.0, 100.0) / 100.0;
    final isOk = k.tyLeThamGia >= k.tyleThamGiaToiThieu;

    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Tổng quan tham gia',
            style: AppTypography.subhead.copyWith(color: AppColors.textPrimary),
          ),
          const SizedBox(height: 14),

          Row(
            children: [
              Expanded(
                child: _StatBox(
                  label: 'Tổng căn hộ',
                  value: '${k.tongSoCanHo}',
                  icon: Icons.apartment_outlined,
                  color: AppColors.primary,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: _StatBox(
                  label: 'Đã tham gia',
                  value: '${k.soCanHoDaThamGia}',
                  icon: Icons.how_to_vote_outlined,
                  color: AppColors.success,
                ),
              ),
            ],
          ),
          const SizedBox(height: 14),

          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Tỷ lệ tham gia',
                style: AppTypography.caption.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
              Text(
                '${k.tyLeThamGia.toStringAsFixed(1)}%  '
                '(tối thiểu ${k.tyleThamGiaToiThieu.toInt()}%)',
                style: AppTypography.caption.copyWith(
                  color: isOk ? AppColors.success : AppColors.warning,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ],
          ),
          const SizedBox(height: 6),
          ClipRRect(
            borderRadius: AppRadius.badge,
            child: LinearProgressIndicator(
              value: tyLe,
              minHeight: 10,
              backgroundColor: AppColors.secondaryLight,
              valueColor: AlwaysStoppedAnimation(
                isOk ? AppColors.success : AppColors.warning,
              ),
            ),
          ),

          const SizedBox(height: 4),
          Align(
            alignment: Alignment(
              ((k.tyleThamGiaToiThieu / 100) * 2 - 1).clamp(-1.0, 1.0),
              0,
            ),
            child: Text(
              '▲ ${k.tyleThamGiaToiThieu.toInt()}%',
              style: AppTypography.captionSmall.copyWith(
                color: AppColors.textDisabled,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _StatBox extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;
  final Color color;

  const _StatBox({
    required this.label,
    required this.value,
    required this.icon,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: color.withAlpha(18),
        borderRadius: AppRadius.inputField,
        border: Border.all(color: color.withAlpha(50)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: color, size: 20),
          const SizedBox(height: 6),
          Text(value, style: AppTypography.headline.copyWith(color: color)),
          Text(
            label,
            style: AppTypography.captionSmall.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
    );
  }
}

class _HieuLucCard extends StatelessWidget {
  final bool isHieuLuc;
  const _HieuLucCard({required this.isHieuLuc});

  @override
  Widget build(BuildContext context) {
    final color = isHieuLuc ? AppColors.success : AppColors.error;
    final bgColor = isHieuLuc ? AppColors.successLight : AppColors.errorLight;
    final icon = isHieuLuc ? Icons.verified_outlined : Icons.cancel_outlined;
    final label = isHieuLuc
        ? 'Cuộc biểu quyết CÓ HIỆU LỰC PHÁP LÝ'
        : 'Cuộc biểu quyết CHƯA ĐẠT hiệu lực pháp lý';
    final sub = isHieuLuc
        ? 'Đủ điều kiện về tỷ lệ tham gia và tỷ lệ đồng ý.'
        : 'Chưa đủ tỷ lệ tham gia tối thiểu hoặc tỷ lệ đồng ý yêu cầu.';

    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: bgColor,
        borderRadius: AppRadius.card,
        border: Border.all(color: color.withAlpha(80)),
      ),
      child: Row(
        children: [
          Icon(icon, color: color, size: 28),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: AppTypography.subhead.copyWith(color: color),
                ),
                const SizedBox(height: 2),
                Text(
                  sub,
                  style: AppTypography.captionSmall.copyWith(
                    color: color.withAlpha(180),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _KetQuaCauHoiCard extends StatelessWidget {
  final int index;
  final KetQuaCauHoiModel data;
  final bool isAreaBased;

  const _KetQuaCauHoiCard({
    required this.index,
    required this.data,
    required this.isAreaBased,
  });

  @override
  Widget build(BuildContext context) {
    final sorted = [...data.ketQuaLuaChons]
      ..sort((a, b) => b.tyLePhanTram.compareTo(a.tyLePhanTram));

    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                width: 26,
                height: 26,
                decoration: const BoxDecoration(
                  color: AppColors.primaryLight,
                  shape: BoxShape.circle,
                ),
                child: Center(
                  child: Text(
                    '$index',
                    style: AppTypography.captionSmall.copyWith(
                      color: AppColors.primary,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Text(data.noiDungCauHoi, style: AppTypography.subhead),
              ),
            ],
          ),
          const SizedBox(height: 14),

          ...sorted.asMap().entries.map(
            (entry) => Padding(
              padding: const EdgeInsets.only(bottom: 10),
              child: _LuaChonResultRow(
                luaChon: entry.value,
                isTop: entry.key == 0,
                isAreaBased: isAreaBased,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _LuaChonResultRow extends StatelessWidget {
  final KetQuaLuaChonModel luaChon;
  final bool isTop;
  final bool isAreaBased;

  const _LuaChonResultRow({
    required this.luaChon,
    required this.isTop,
    required this.isAreaBased,
  });

  @override
  Widget build(BuildContext context) {
    final pct = luaChon.tyLePhanTram.clamp(0.0, 100.0);
    final barColor = isTop ? AppColors.primary : AppColors.secondary;
    final phieuLabel = isAreaBased
        ? '${luaChon.soLuongPhieuBau.toStringAsFixed(1)} m²'
        : '${luaChon.soLuongPhieuBau.toInt()} phiếu';

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            if (isTop)
              const Padding(
                padding: EdgeInsets.only(right: 4),
                child: Icon(
                  Icons.emoji_events_outlined,
                  color: AppColors.warning,
                  size: 16,
                ),
              ),
            Expanded(
              child: Text(
                luaChon.noiDungLuaChon,
                style: AppTypography.body.copyWith(
                  color: isTop
                      ? AppColors.textPrimary
                      : AppColors.textSecondary,
                  fontWeight: isTop ? FontWeight.w600 : FontWeight.w400,
                ),
              ),
            ),
            const SizedBox(width: 8),
            Text(
              '${pct.toStringAsFixed(1)}%',
              style: AppTypography.subhead.copyWith(color: barColor),
            ),
          ],
        ),
        const SizedBox(height: 5),

        ClipRRect(
          borderRadius: AppRadius.badge,
          child: LinearProgressIndicator(
            value: pct / 100,
            minHeight: 8,
            backgroundColor: AppColors.secondaryLight,
            valueColor: AlwaysStoppedAnimation(
              barColor.withAlpha(isTop ? 255 : 160),
            ),
          ),
        ),
        const SizedBox(height: 3),

        Text(
          phieuLabel,
          style: AppTypography.captionSmall.copyWith(
            color: AppColors.textDisabled,
          ),
        ),
      ],
    );
  }
}
