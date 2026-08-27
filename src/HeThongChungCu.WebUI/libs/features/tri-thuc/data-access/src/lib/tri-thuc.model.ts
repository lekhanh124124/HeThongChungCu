export interface TriThucChatbotDto {
  id: number;
  tieuDe: string;
  noiDung: string;
  danhMuc: string;
  thuTuHienThi: number;
  isActive: boolean;
  isSynced: boolean;
  lastSyncedAt: string | null;
  createdAt: string;
  updatedAt: string | null;
  createdBy: string;
}

export interface RequestCreateTriThucChatbot {
  tieuDe: string;
  noiDung: string;
  danhMuc: string;
  thuTuHienThi: number;
}

export interface RequestUpdateTriThucChatbot {
  id: number;
  tieuDe: string;
  noiDung: string;
  danhMuc: string;
  thuTuHienThi: number;
}

export interface RequestGetListTriThucChatbot {
  danhMuc?: string;
  isActive?: boolean;
  isSynced?: boolean;
  keyword?: string;
  pageNumber: number;
  pageSize: number;
  sortCol?: string;
  isAsc?: boolean;
}

export interface RequestDeleteTriThucChatbot {
  ids: number[];
}

export interface RequestToggleTriThucChatbot {
  id: number;
}

export interface ImportTriThucChatbotResult {
  importedCount: number;
  danhMuc: string;
  importedTitles: string[];
}

export interface SyncTriThucChatbotResult {
  upsertedCount: number;
  deletedCount: number;
  skippedCount: number;
  errorIds: number[];
  elapsedMs: number;
}
