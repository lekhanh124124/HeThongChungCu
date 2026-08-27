import { Component, OnInit } from '@angular/core';
import { SaoLuuService, BackupItem } from '@features/sao-luu/data-access';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-sao-luu-list',
  templateUrl: './sao-luu-list.component.html',
  styleUrls: ['./sao-luu-list.component.scss']
})
export class SaoLuuListComponent implements OnInit {
  items: BackupItem[] = [];
  loading = false;
  creating = false;

  keyword = '';
  pageNumber = 1;
  pageSize = 10;
  totalItems = 0;
  sortCol = '';
  isAsc = true;

  constructor(
    private svc: SaoLuuService,
    private modal: NzModalService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.load();
  }

  onRefresh(): void {
    this.keyword = '';
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNumber = 1;
    this.load();
  }

  onSort(col: string): void {
    if (this.sortCol === col) this.isAsc = !this.isAsc;
    else {
      this.sortCol = col;
      this.isAsc = true;
    }
    this.pageNumber = 1;
    this.load();
  }

  formatSize(bytes: number): string {
    if (!bytes) return '-';
    const units = ['B', 'KB', 'MB', 'GB'];
    let i = 0;
    let size = bytes;
    while (size >= 1024 && i < units.length - 1) {
      size /= 1024;
      i++;
    }
    return `${size.toFixed(i > 0 ? 2 : 0)} ${units[i]}`;
  }

  openCreateModal(): void {
    this.modal.confirm({
      nzTitle: 'Tạo bản sao lưu',
      nzContent: 'Bạn có chắc chắn muốn tạo một bản sao lưu CSDL mới? Quá trình này có thể mất vài phút.',
      nzOkText: 'Tạo',
      nzCancelText: 'Hủy',
      nzOkLoading: this.creating,
      nzOnOk: () => {
        this.creating = true;
        return new Promise<void>((resolve) => {
          this.svc.create().subscribe({
            next: (res: any) => {
              this.creating = false;
              if (res && res.isOk) {
                this.notification.success('Thành công', 'Tạo bản sao lưu thành công');
                this.load();
              } else {
                this.notification.error('Lỗi', res?.errors?.[0]?.description || 'Tạo bản sao lưu thất bại');
              }
              resolve();
            },
            error: () => {
              this.creating = false;
              this.notification.error('Lỗi', 'Tạo bản sao lưu thất bại');
              resolve();
            }
          });
        });
      }
    });
  }

  confirmRestore(it: BackupItem): void {
    this.modal.confirm({
      nzTitle: 'Khôi phục dữ liệu',
      nzContent: `Bạn có chắc chắn muốn khôi phục dữ liệu từ bản sao lưu "${it.fileName}"? Toàn bộ dữ liệu hiện tại sẽ bị thay thế. Hành động này không thể hoàn tác.`,
      nzOkText: 'Khôi phục',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => {
        return new Promise<void>((resolve) => {
          this.svc.restore(it.fileId).subscribe({
            next: (res: any) => {
              if (res && res.isOk) {
                this.notification.success('Thành công', 'Khôi phục dữ liệu thành công');
                this.load();
              } else {
                this.notification.error('Lỗi', res?.errors?.[0]?.description || 'Khôi phục dữ liệu thất bại');
              }
              resolve();
            },
            error: () => {
              this.notification.error('Lỗi', 'Khôi phục dữ liệu thất bại');
              resolve();
            }
          });
        });
      }
    });
  }

  confirmDelete(it: BackupItem): void {
    this.modal.confirm({
      nzTitle: 'Xóa bản sao lưu',
      nzContent: `Bạn có chắc chắn muốn xóa bản sao lưu "${it.fileName}"? Dữ liệu sẽ bị xóa vĩnh viễn khỏi Cloud.`,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => {
        return new Promise<void>((resolve) => {
          this.svc.delete(it.fileId).subscribe({
            next: (res: any) => {
              if (res && res.isOk) {
                this.notification.success('Thành công', 'Xóa bản sao lưu thành công');
                this.load();
              } else {
                this.notification.error('Lỗi', res?.errors?.[0]?.description || 'Xóa bản sao lưu thất bại');
              }
              resolve();
            },
            error: () => {
              this.notification.error('Lỗi', 'Xóa bản sao lưu thất bại');
              resolve();
            }
          });
        });
      }
    });
  }

  load(): void {
    this.loading = true;
    const query: any = {
      keyword: this.keyword || undefined,
      sortCol: this.sortCol || undefined,
      isAsc: this.isAsc,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };
    this.svc.getList(query).subscribe({
      next: (r: any) => {
        this.loading = false;
        if (r && r.isOk) {
          this.items = r.result.items || [];
          const pi = r.result.pagingInfo;
          if (pi) {
            this.pageSize = pi.pageSize || this.pageSize;
            this.pageNumber = pi.pageNumber || this.pageNumber;
            this.totalItems = pi.totalItems || 0;
          }
        }
      },
      error: () => {
        this.loading = false;
        this.items = [];
      }
    });
  }
}
