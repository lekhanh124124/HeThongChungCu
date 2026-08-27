export interface BackupItem {
  fileId: number;
  fileName: string;
  fileUrl: string;
  size: number;
  createdAt: string;
  contentType: string;
}

export interface BackupListQuery {
  keyword?: string;
  sortCol?: string;
  isAsc: boolean;
  pageNumber: number;
  pageSize: number;
}
