import 'dart:async';
import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:signalr_netcore/signalr_client.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';

import 'package:klks_app/core/network/api_client.dart';
import 'package:klks_app/core/storage/user_session.dart';

class ThongBaoEvent {
  final String tieuDe;
  final String noiDung;
  final int? phanBoThongBaoId;

  const ThongBaoEvent({
    required this.tieuDe,
    required this.noiDung,
    this.phanBoThongBaoId,
  });
}

class ThongBaoHubService {
  static const String hubUrl = '${ApiClient.baseUrl}/notifications';

  ThongBaoHubService._();
  static final ThongBaoHubService instance = ThongBaoHubService._();

  final _session = UserSession.instance;

  HubConnection? _connection;

  final _connectionStateController =
      StreamController<HubConnectionState>.broadcast();
  Stream<HubConnectionState> get onConnectionStateChanged =>
      _connectionStateController.stream;
  HubConnectionState get connectionState =>
      _connection?.state ?? HubConnectionState.Disconnected;

  final _eventController = StreamController<ThongBaoEvent>.broadcast();
  Stream<ThongBaoEvent> get onThongBaoMoi => _eventController.stream;

  final _unreadCountController = StreamController<int>.broadcast();
  Stream<int> get onUnreadCountChanged => _unreadCountController.stream;
  int _unreadCount = 0;

  bool get isConnected => _connection?.state == HubConnectionState.Connected;

  final _localNotif = FlutterLocalNotificationsPlugin();
  bool _notifInitialized = false;

  Future<void> _initLocalNotif() async {
    if (_notifInitialized) return;

    if ((await Permission.notification.status).isDenied) {
      await Permission.notification.request();
    }

    await _localNotif.initialize(
      settings: const InitializationSettings(
        android: AndroidInitializationSettings('@mipmap/ic_launcher'),
        iOS: DarwinInitializationSettings(
          requestAlertPermission: true,
          requestBadgePermission: true,
          requestSoundPermission: true,
        ),
      ),
    );

    final androidImpl = _localNotif
        .resolvePlatformSpecificImplementation<
          AndroidFlutterLocalNotificationsPlugin
        >();
    await androidImpl?.createNotificationChannel(
      const AndroidNotificationChannel(
        'thong_bao_channel',
        'Thông báo',
        description: 'Thông báo từ hệ thống chung cư',
        importance: Importance.high,
        playSound: true,
      ),
    );

    _notifInitialized = true;
  }

  Future<void> _showLocalNotif(ThongBaoEvent event) async {
    if (!(await Permission.notification.status).isGranted) return;

    final notifId =
        event.phanBoThongBaoId ??
        DateTime.now().millisecondsSinceEpoch & 0x7FFFFFFF;

    await _localNotif.show(
      id: notifId,
      title: event.tieuDe,
      body: event.noiDung,
      notificationDetails: const NotificationDetails(
        android: AndroidNotificationDetails(
          'thong_bao_channel',
          'Thông báo',
          channelDescription: 'Thông báo từ hệ thống chung cư',
          importance: Importance.high,
          priority: Priority.high,
          playSound: true,
        ),
        iOS: DarwinNotificationDetails(
          presentAlert: true,
          presentBadge: true,
          presentSound: true,
        ),
      ),
    );
  }

  Future<void> connect() async {
    final state = _connection?.state;
    if (state == HubConnectionState.Connected ||
        state == HubConnectionState.Connecting ||
        state == HubConnectionState.Reconnecting) {
      return;
    }

    final token = _session.accessToken;
    if (token == null || token.isEmpty) return;

    await _initLocalNotif();

    _connectionStateController.add(HubConnectionState.Connecting);

    _connection = HubConnectionBuilder()
        .withUrl(
          hubUrl,
          options: HttpConnectionOptions(
            transport: HttpTransportType.LongPolling,
            accessTokenFactory: () async => _session.accessToken ?? '',
          ),
        )
        .withAutomaticReconnect(retryDelays: [0, 2000, 10000, 30000])
        .build();

    _connection!.on('ReceiveNotification', _onReceiveNotification);

    _connection!.onreconnecting(({error}) {
      _connectionStateController.add(HubConnectionState.Reconnecting);
    });

    _connection!.onreconnected(({connectionId}) {
      _connectionStateController.add(connectionState);
    });

    _connection!.onclose(({error}) {
      _connectionStateController.add(connectionState);
    });

    try {
      await _connection!.start()?.timeout(const Duration(seconds: 30));

      if (_connection!.state != HubConnectionState.Connected) {
        throw StateError('SignalR connected but state=${_connection!.state}');
      }

      _connectionStateController.add(connectionState);
    } catch (_) {
      await _connection?.stop().catchError((_) {});
      _connectionStateController.add(connectionState);
    }
  }

  void _onReceiveNotification(List<Object?>? arguments) {
    if (arguments == null || arguments.isEmpty) return;

    Map<String, dynamic> data;
    final raw = arguments[0];

    if (raw is String) {
      try {
        data = jsonDecode(utf8.decode(raw.codeUnits)) as Map<String, dynamic>;
      } catch (_) {
        data = {};
      }
    } else if (raw is Map) {
      data = raw.map((k, v) {
        if (v is String) {
          try {
            return MapEntry(k, utf8.decode(latin1.encode(v)));
          } catch (_) {
            return MapEntry(k, v);
          }
        }
        return MapEntry(k, v);
      }).cast<String, dynamic>();
    } else {
      return;
    }

    final event = ThongBaoEvent(
      tieuDe:
          (data['tieuDe'] as String?) ??
          (data['TieuDe'] as String?) ??
          'Thông báo mới',
      noiDung:
          (data['noiDung'] as String?) ?? (data['NoiDung'] as String?) ?? '',
      phanBoThongBaoId:
          (data['phanBoThongBaoId'] as int?) ??
          (data['PhanBoThongBaoId'] as int?),
    );

    _eventController.add(event);
    _unreadCountController.add(++_unreadCount);

    unawaited(
      _showLocalNotif(event).catchError((e) {
        debugPrint('[ThongBaoHub] _showLocalNotif error: $e');
      }),
    );
  }

  void resetUnreadCount() {
    _unreadCount = 0;
    _unreadCountController.add(0);
  }

  Future<void> disconnect() async {
    await _connection?.stop();
    _connection = null;
    _unreadCount = 0;
    _unreadCountController.add(0);
    _connectionStateController.add(HubConnectionState.Disconnected);
  }
}
