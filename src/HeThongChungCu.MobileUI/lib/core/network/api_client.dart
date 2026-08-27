import 'package:dio/dio.dart';

import 'package:klks_app/features/shared/models/paging_model.dart';

import 'api_interceptor.dart';

enum ErrorType { network, unauthorized, validation, server, unknown }

class AppException implements Exception {
  final String message;
  final List<String>? messages;
  final ErrorType type;
  final int? code;
  final dynamic raw;

  const AppException(
    this.message, {
    this.messages,
    this.type = ErrorType.unknown,
    this.code,
    this.raw,
  });

  @override
  String toString() => message;
}

class ErrorParser {
  ErrorParser._();

  static AppException parse(dynamic data, {int? statusCode}) {
    try {
      if (data == null) {
        return AppException(
          'Có lỗi xảy ra',
          type: _mapType(statusCode),
          code: statusCode,
        );
      }

      if (data is Map<String, dynamic>) {
        final errors = data['errors'];
        if (errors is List && errors.isNotEmpty) {
          final msgs = errors
              .map<String>((e) => e['description']?.toString() ?? '')
              .where((s) => s.isNotEmpty)
              .toList();
          if (msgs.isNotEmpty) {
            return AppException(
              msgs.join('\n'),
              messages: msgs,
              type: ErrorType.validation,
              code: statusCode,
              raw: data,
            );
          }
        }

        final warnings = data['warningMessages'];
        if (warnings is List && warnings.isNotEmpty) {
          final msgs = warnings.map((e) => e.toString()).toList();
          return AppException(
            msgs.join('\n'),
            messages: msgs,
            type: ErrorType.validation,
            code: statusCode,
            raw: data,
          );
        }

        final msg = data['message'];
        if (msg != null) {
          return AppException(
            msg.toString(),
            type: _mapType(statusCode),
            code: statusCode,
            raw: data,
          );
        }
      }

      return AppException(
        'Có lỗi xảy ra',
        type: _mapType(statusCode),
        code: statusCode,
        raw: data,
      );
    } catch (_) {
      return AppException(
        'Có lỗi xảy ra',
        type: ErrorType.unknown,
        code: statusCode,
        raw: data,
      );
    }
  }

  static ErrorType _mapType(int? statusCode) {
    if (statusCode == null) return ErrorType.unknown;
    if (statusCode == 401) return ErrorType.unauthorized;
    if (statusCode >= 400 && statusCode < 500) return ErrorType.validation;
    if (statusCode >= 500) return ErrorType.server;
    return ErrorType.unknown;
  }
}

class ApiResponse {
  final dynamic _result;
  final int? statusCode;

  const ApiResponse(this._result, {this.statusCode});

  T item<T>(T Function(Map<String, dynamic>) fromJson) {
    if (_result == null) {
      throw const AppException(
        'Không có dữ liệu trả về',
        type: ErrorType.server,
      );
    }
    return fromJson(_result as Map<String, dynamic>);
  }

  T? itemOrNull<T>(T Function(Map<String, dynamic>) fromJson) {
    if (_result == null) return null;
    return fromJson(_result as Map<String, dynamic>);
  }

  List<T> list<T>(T Function(Map<String, dynamic>) fromJson) {
    final raw = _result as List<dynamic>? ?? [];
    return raw.map((e) => fromJson(e as Map<String, dynamic>)).toList();
  }

  PagedResult<T> pagedResult<T>(T Function(Map<String, dynamic>) fromJson) {
    final map = _result as Map<String, dynamic>;
    return PagedResult.fromJson(map, fromJson);
  }

  T raw<T>() => _result as T;
}

class ApiClient {
  static const String baseUrl =
      'https://hethongchungcu-webapi.azurewebsites.net';

  ApiClient._internal();
  static final ApiClient instance = ApiClient._internal();

  late final Dio dio = _createDio();
  late final Dio plainDio = _createPlainDio();

  Dio _createDio() {
    final d = Dio(_baseOptions());
    d.interceptors.add(ApiInterceptor(d));
    return d;
  }

  Dio _createPlainDio() => Dio(_baseOptions());

  BaseOptions _baseOptions() => BaseOptions(
    baseUrl: baseUrl,
    connectTimeout: const Duration(seconds: 30),
    receiveTimeout: const Duration(seconds: 30),
    sendTimeout: const Duration(seconds: 30),
    headers: {'Content-Type': 'application/json'},
  );

  Future<ApiResponse> get(
    String path, {
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) => _execute(
    () => dio.get(path, queryParameters: queryParameters, options: options),
  );

  Future<ApiResponse> post(
    String path, {
    Map<String, dynamic>? body,
    Options? options,
  }) => _execute(() => dio.post(path, data: body ?? {}, options: options));

  Future<ApiResponse> put(
    String path, {
    Map<String, dynamic>? body,
    Options? options,
  }) => _execute(() => dio.put(path, data: body ?? {}, options: options));

  Future<ApiResponse> delete(
    String path, {
    Map<String, dynamic>? body,
    Options? options,
  }) => _execute(() => dio.delete(path, data: body ?? {}, options: options));

  Future<ApiResponse> postForm(String path, FormData formData) => _execute(
    () => dio.post(
      path,
      data: formData,
      options: Options(contentType: 'multipart/form-data'),
    ),
  );

  Future<ApiResponse> _execute(
    Future<Response<dynamic>> Function() call,
  ) async {
    try {
      final response = await call();
      return _unwrap(response);
    } on AppException {
      rethrow;
    } on DioException catch (e) {
      throw _fromDio(e);
    } catch (e) {
      throw AppException(e.toString(), type: ErrorType.unknown);
    }
  }

  ApiResponse _unwrap(Response<dynamic> response) {
    final data = response.data;

    if (data is List) {
      return ApiResponse(data, statusCode: response.statusCode);
    }

    if (data == null) {
      throw const AppException(
        'Không có dữ liệu trả về',
        type: ErrorType.server,
      );
    }

    final map = data as Map<String, dynamic>;
    final isOk = map['isOk'] as bool? ?? true;

    if (!isOk) {
      throw ErrorParser.parse(map, statusCode: response.statusCode);
    }

    return ApiResponse(map['result'], statusCode: response.statusCode);
  }

  AppException _fromDio(DioException e) {
    if (e.response?.data != null) {
      return ErrorParser.parse(
        e.response!.data,
        statusCode: e.response?.statusCode,
      );
    }
    return AppException(
      _dioMessage(e),
      type: _dioType(e),
      code: e.response?.statusCode,
    );
  }

  String _dioMessage(DioException e) => switch (e.type) {
    DioExceptionType.connectionTimeout ||
    DioExceptionType.sendTimeout ||
    DioExceptionType.receiveTimeout =>
      'Kết nối quá thời gian, vui lòng thử lại',
    DioExceptionType.connectionError => 'Không có kết nối mạng',
    _ => e.message ?? 'Có lỗi xảy ra',
  };

  ErrorType _dioType(DioException e) => switch (e.type) {
    DioExceptionType.connectionTimeout ||
    DioExceptionType.sendTimeout ||
    DioExceptionType.receiveTimeout ||
    DioExceptionType.connectionError => ErrorType.network,
    _ => ErrorType.unknown,
  };
}
