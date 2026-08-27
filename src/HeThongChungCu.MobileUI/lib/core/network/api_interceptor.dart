import 'dart:async';
import 'package:dio/dio.dart';

import 'package:klks_app/core/storage/user_session.dart';
import 'package:klks_app/core/guards/auth_guard.dart';
import 'package:klks_app/features/auth/services/auth_service.dart';

class ApiInterceptor extends Interceptor {
  final Dio dio;
  final UserSession _session = UserSession.instance;

  bool _isRefreshing = false;
  final List<_PendingRequest> _queue = [];

  ApiInterceptor(this.dio);

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    if (options.extra['skipAuth'] == true) return handler.next(options);
    final token = _session.accessToken;
    if (token != null && token.isNotEmpty) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  }

  @override
  void onResponse(Response response, ResponseInterceptorHandler handler) {
    handler.next(response);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    final request = err.requestOptions;

    if (err.response?.statusCode != 401) return handler.next(err);

    if (request.extra['skipAuth'] == true || request.extra['isRetry'] == true) {
      await _logout();
      return handler.next(err);
    }

    if (_isRefreshing) {
      final completer = Completer<Response>();
      _queue.add(_PendingRequest(request, completer));
      try {
        return handler.resolve(await completer.future);
      } catch (_) {
        return handler.next(err);
      }
    }

    _isRefreshing = true;

    try {
      await AuthService.instance.refreshToken();

      final retried = await _retry(request);

      for (final p in _queue) {
        try {
          p.completer.complete(await _retry(p.request));
        } catch (e) {
          p.completer.completeError(e);
        }
      }
      _queue.clear();
      return handler.resolve(retried);
    } catch (_) {
      await _failQueue();
      await _logout();
      return handler.next(err);
    } finally {
      _isRefreshing = false;
    }
  }

  Future<Response> _retry(RequestOptions request) {
    final token = _session.accessToken;
    return dio.request(
      request.path,
      data: request.data,
      queryParameters: request.queryParameters,
      options: Options(
        method: request.method,
        headers: {
          ...request.headers,
          if (token != null && token.isNotEmpty)
            'Authorization': 'Bearer $token',
        },
        extra: {...request.extra, 'isRetry': true},
      ),
    );
  }

  Future<void> _failQueue() async {
    for (final p in _queue) {
      p.completer.completeError(Exception('Refresh token failed'));
    }
    _queue.clear();
  }

  Future<void> _logout() async {
    await AuthGuard.instance.logout();
  }
}

class _PendingRequest {
  final RequestOptions request;
  final Completer<Response> completer;
  _PendingRequest(this.request, this.completer);
}
