import { Component, OnInit, ViewChild } from '@angular/core';
import { TriThucApiService } from '../../../../data-access/src/lib/tri-thuc.service';
import { TriThucFormComponent } from '../tri-thuc-form/tri-thuc-form.component';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-tri-thuc-list',
  templateUrl: './tri-thuc-list.component.html',
  styleUrls: ['./tri-thuc-list.component.scss'],
})
export class TriThucListComponent implements OnInit {
  items: any[] = [];
  loading = false;
  isModalVisible = false;
  modalTitle = '';
  saving = false;
  editingId?: number | null;

  // filters
  keyword = '';
  danhMucFilter: string | null = null;
  isActiveFilter: string | null = null;
  isSyncedFilter: string | null = null;
  danhMucOptions: string[] = [];

  // paging / sorting
  pageSize = 20;
  pageNumber = 1;
  total = 0;
  sortCol: string | null = null;
  isAsc = true;

  // selection
  listOfCurrentPageData: readonly any[] = [];
  setOfCheckedId = new Set<number>();
  checked = false;
  indeterminate = false;

  // sync state
  syncing = false;
  unsyncedCount = 0;

  // import
  importModalVisible = false;
  importFile?: File;
  importDanhMuc: string | null = null;
  importing = false;

  @ViewChild(TriThucFormComponent) formComp?: TriThucFormComponent;

  constructor(
    private api: TriThucApiService,
    private notification: NzNotificationService,
    private modal: NzModalService
  ) {}

  ngOnInit(): void {
    this.load();
    this.loadDanhMuc();
  }

  load(): void {
    const query: any = {
      keyword: this.keyword || undefined,
      danhMuc: this.danhMucFilter || undefined,
      isActive: this.isActiveFilter !== null ? this.isActiveFilter === 'true' : undefined,
      isSynced: this.isSyncedFilter !== null ? this.isSyncedFilter === 'true' : undefined,
      sortCol: this.sortCol || undefined,
      isAsc: this.sortCol ? this.isAsc : undefined,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
    };

    this.loading = true;
    this.api.getList(query).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.items = res?.result?.items || [];
        const pi = res?.result?.pagingInfo;
        if (pi) {
          this.pageSize = pi.pageSize || this.pageSize;
          this.pageNumber = pi.pageNumber || this.pageNumber;
          this.total = pi.totalItems || 0;
        } else {
          this.total = this.items.length;
        }
        this.refreshCheckedStatus();
      },
      error: () => {
        this.loading = false;
        this.items = [];
        this.total = 0;
      },
    });
  }

  loadDanhMuc(): void {
    this.api.getDanhMuc().subscribe({
      next: (res: any) => {
        this.danhMucOptions = Array.isArray(res?.result) ? res.result : [];
      },
      error: () => { this.danhMucOptions = []; },
    });
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.load();
  }

  onRefresh(): void {
    this.keyword = '';
    this.danhMucFilter = null;
    this.isActiveFilter = null;
    this.isSyncedFilter = null;
    this.sortCol = null;
    this.isAsc = true;
    this.pageNumber = 1;
    this.setOfCheckedId.clear();
    this.load();
  }

  onSortSelect(col: string | null): void {
    if (!col) return;
    if (this.sortCol === col) this.isAsc = !this.isAsc;
    else {
      this.sortCol = col;
      this.isAsc = true;
    }
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

  // selection
  updateCheckedSet(id: number | undefined, checked: boolean): void {
    if (id == null) return;
    if (checked) this.setOfCheckedId.add(id);
    else this.setOfCheckedId.delete(id);
  }

  onItemChecked(id: number | undefined, checked: boolean): void {
    if (id == null) return;
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onCurrentPageDataChange(list: readonly any[]): void {
    this.listOfCurrentPageData = list || [];
    this.refreshCheckedStatus();
  }

  onAllChecked(checked: boolean): void {
    (this.listOfCurrentPageData || []).forEach(item => this.updateCheckedSet(item.id, checked));
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    const list = this.listOfCurrentPageData || [];
    if (!list || list.length === 0) {
      this.checked = false;
      this.indeterminate = false;
      return;
    }
    this.checked = list.every(item => this.setOfCheckedId.has(item.id));
    this.indeterminate = list.some(item => this.setOfCheckedId.has(item.id)) && !this.checked;
  }

  // status helpers
  getStatusInfo(item: any): { text: string; class: string } {
    if (!item.isActive && !item.isSynced) return { text: 'Nháp', class: 'status-draft' };
    if (item.isActive && !item.isSynced) return { text: 'Chờ đồng bộ', class: 'status-pending-sync' };
    if (item.isActive && item.isSynced) return { text: 'Đã đồng bộ', class: 'status-synced' };
    return { text: 'Chờ gỡ bỏ', class: 'status-pending-remove' };
  }

  formatDate(d: string | null): string {
    if (!d) return '';
    const dt = new Date(d);
    if (isNaN(dt.getTime())) return '';
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${pad(dt.getDate())}/${pad(dt.getMonth() + 1)}/${dt.getFullYear()} ${pad(dt.getHours())}:${pad(dt.getMinutes())}`;
  }

  // CRUD
  openCreateModal(): void {
    this.editingId = undefined;
    this.modalTitle = 'Thêm tri thức mới';
    this.isModalVisible = true;
  }

  edit(item: any): void {
    if (!item) return;
    this.editingId = item.id;
    this.modalTitle = 'Cập nhật tri thức';
    this.isModalVisible = true;
  }

  cancelModal(): void {
    this.isModalVisible = false;
    this.saving = false;
  }

  onModalOk(): void {
    this.saving = true;
    this.formComp?.submit();
  }

  onSaved(): void {
    this.saving = false;
    this.isModalVisible = false;
    this.load();
  }

  // toggle active
  toggleActive(item: any): void {
    if (!item) return;
    const id = item.id;
    if (item.isActive) {
      this.api.deactivate(id).subscribe({
        next: () => {
          this.notification.success('Thành công', 'Đã vô hiệu hóa tri thức');
          this.load();
        },
        error: () => this.notification.error('Lỗi', 'Thao tác thất bại'),
      });
    } else {
      this.api.activate(id).subscribe({
        next: () => {
          this.notification.success('Thành công', 'Đã kích hoạt tri thức');
          this.load();
        },
        error: () => this.notification.error('Lỗi', 'Thao tác thất bại'),
      });
    }
  }

  // delete
  deleteOne(id: number | undefined): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận xóa',
      nzContent: 'Bạn có chắc chắn muốn xóa mục tri thức này? Chỉ có thể xóa mục đang ở trạng thái "Nháp" hoặc "Chờ gỡ bỏ".',
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this.api.delete([id]).subscribe({
          next: () => {
            this.notification.success('Thành công', 'Đã xóa tri thức');
            this.setOfCheckedId.delete(id);
            this.load();
          },
          error: (err) => {
            const msg = err?.error?.errors?.[0]?.description || 'Xóa thất bại. Cần vô hiệu hóa trước khi xóa.';
            this.notification.error('Lỗi', msg);
          },
        });
      },
    });
  }

  deleteSelectedMultiple(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) return;

    const inactiveIds = this.items.filter(i => this.setOfCheckedId.has(i.id) && !i.isActive).map(i => i.id);
    if (inactiveIds.length !== ids.length) {
      this.notification.warning('Không thể xóa', 'Chỉ được xóa các mục đã vô hiệu hóa (IsActive = false). Hãy tắt các mục đang hoạt động trước.');
      return;
    }

    this.modal.confirm({
      nzTitle: 'Xác nhận',
      nzContent: `Bạn có chắc chắn muốn xóa ${ids.length} tri thức đã chọn?`,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this.api.delete(ids).subscribe({
          next: () => {
            this.notification.success('Thành công', 'Đã xóa tri thức');
            this.setOfCheckedId.clear();
            this.load();
          },
          error: (err) => {
            const msg = err?.error?.errors?.[0]?.description || 'Xóa thất bại';
            this.notification.error('Lỗi', msg);
          },
        });
      },
    });
  }

  // sync
  onSync(): void {
    this.syncing = true;
    this.api.sync().subscribe({
      next: (res: any) => {
        this.syncing = false;
        const r = res?.result || {};
        const upserted = r.upsertedCount ?? 0;
        const deleted = r.deletedCount ?? 0;
        const ms = r.elapsedMs ?? 0;
        this.notification.success(
          'Đồng bộ thành công',
          `Đã upsert ${upserted} vector, xóa ${deleted} vector (${(ms / 1000).toFixed(1)}s)`
        );
        this.load();
      },
      error: () => {
        this.syncing = false;
        this.notification.error('Lỗi', 'Đồng bộ thất bại');
      },
    });
  }

  getUnsyncedCount(): number {
    return (this.items || []).filter(i => (i.isActive && !i.isSynced) || (!i.isActive && i.isSynced)).length;
  }

  // import
  openImportModal(): void {
    this.importFile = undefined;
    this.importDanhMuc = null;
    this.importModalVisible = true;
  }

  onImportFileChange(event: any): void {
    const file = event?.target?.files?.[0];
    if (file) {
      this.importFile = file;
    }
  }

  doImport(): void {
    if (!this.importFile) {
      this.notification.warning('Thiếu file', 'Vui lòng chọn file .md để import');
      return;
    }
    if (!this.importFile.name.endsWith('.md')) {
      this.notification.warning('Sai định dạng', 'Chỉ hỗ trợ file .md');
      return;
    }
    if (this.importFile.size > 5 * 1024 * 1024) {
      this.notification.warning('Quá lớn', 'File tối đa 5MB');
      return;
    }

    this.importing = true;
    const formData = new FormData();
    formData.append('File', this.importFile);
    if (this.importDanhMuc) {
      formData.append('DanhMuc', this.importDanhMuc);
    }

    this.api.importFile(formData).subscribe({
      next: (res: any) => {
        this.importing = false;
        const r = res?.result || {};
        this.importModalVisible = false;
        this.notification.success(
          'Import thành công',
          `Đã import ${r.importedCount ?? 0} mục tri thức vào danh mục "${r.danhMuc || ''}"`
        );
        this.load();
      },
      error: (err) => {
        this.importing = false;
        const msg = err?.error?.errors?.[0]?.description || 'Import thất bại';
        this.notification.error('Lỗi', msg);
      },
    });
  }

  cancelImport(): void {
    this.importModalVisible = false;
    this.importFile = undefined;
    this.importDanhMuc = null;
  }
}
