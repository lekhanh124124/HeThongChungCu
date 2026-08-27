class UploadedFile {
  final int fileId;
  final String fileName;
  final String fileUrl;
  final String contentType;

  const UploadedFile({
    required this.fileId,
    required this.fileName,
    required this.fileUrl,
    required this.contentType,
  });

  bool get isImage => contentType.startsWith('image/');
  bool get isPdf => contentType == 'application/pdf';

  factory UploadedFile.fromJson(Map<String, dynamic> json) => UploadedFile(
    fileId: json['fileId'] as int? ?? 0,
    fileName: json['fileName'] as String? ?? '',
    fileUrl: json['fileUrl'] as String? ?? '',
    contentType: json['contentType'] as String? ?? '',
  );

  Map<String, dynamic> toJson() => {
    'fileId': fileId,
    'fileName': fileName,
    'fileUrl': fileUrl,
    'contentType': contentType,
  };

  UploadedFile copyWith({
    int? fileId,
    String? fileName,
    String? fileUrl,
    String? contentType,
  }) => UploadedFile(
    fileId: fileId ?? this.fileId,
    fileName: fileName ?? this.fileName,
    fileUrl: fileUrl ?? this.fileUrl,
    contentType: contentType ?? this.contentType,
  );
}

class FileAttachment {
  final int id;
  final String fileUrl;
  final String fileName;
  final String contentType;

  const FileAttachment({
    required this.id,
    required this.fileUrl,
    required this.fileName,
    required this.contentType,
  });

  bool get isImage => contentType.startsWith('image/');
  bool get isPdf => contentType == 'application/pdf';

  factory FileAttachment.fromJson(Map<String, dynamic> json) => FileAttachment(
    id: json['id'] as int? ?? 0,
    fileUrl: json['fileUrl'] as String? ?? '',
    fileName: json['fileName'] as String? ?? '',
    contentType: json['contentType'] as String? ?? '',
  );

  factory FileAttachment.fromJsonAlt(Map<String, dynamic> json) =>
      FileAttachment(
        id: json['fileId'] as int? ?? json['id'] as int? ?? 0,
        fileUrl: json['fileUrl'] as String? ?? '',
        fileName: json['fileName'] as String? ?? '',
        contentType: json['contentType'] as String? ?? '',
      );

  Map<String, dynamic> toJson() => {
    'id': id,
    'fileUrl': fileUrl,
    'fileName': fileName,
    'contentType': contentType,
  };

  FileAttachment copyWith({
    int? id,
    String? fileUrl,
    String? fileName,
    String? contentType,
  }) => FileAttachment(
    id: id ?? this.id,
    fileUrl: fileUrl ?? this.fileUrl,
    fileName: fileName ?? this.fileName,
    contentType: contentType ?? this.contentType,
  );
}
