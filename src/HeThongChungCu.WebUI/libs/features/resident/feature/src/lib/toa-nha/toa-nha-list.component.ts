import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ChungCuService, ToaNha } from '@features/resident/data-access';
import { ViewChild } from '@angular/core';
import { ToaNhaFormComponent } from './toa-nha-form.component';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-toa-nha-list',
  templateUrl: './toa-nha-list.component.html',
  styleUrls: ['./toa-nha-list.component.scss']
})
export class ToaNhaListComponent implements OnInit {
  items: ToaNha[] = [];
  loading = false;

  // search / sort / paging state
  keyword = '';
  sortCol = '';
  isAsc = true;
  pageNumber = 1;
  pageSize = 10;
  totalItems = 0;

  listOfSelection: any[] = [];
  checked = false;
  indeterminate = false;
  listOfCurrentPageData: readonly ToaNha[] = [];
  setOfCheckedId = new Set<number>();

  // catalog trạng thái tòa nhà
  statusOptions: any[] = [];
  statusDict: { [id: number]: any } = {};

  @ViewChild(ToaNhaFormComponent) formComp?: ToaNhaFormComponent;

  isModalVisible = false;
  modalTitle = '';
  editingId?: number;
  saving = false;

  constructor(
    private svc: ChungCuService,
    private router: Router,
    private modal: NzModalService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    this.load();
    this.loadStatusOptions();
  }

  load(callback?: () => void): void {
    this.loading = true;
    const q = {
      keyword: this.keyword || undefined,
      sortCol: this.sortCol || undefined,
      isAsc: this.isAsc,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };
    this.svc.getToaNhaList(q).subscribe((r: any) => {
      this.loading = false;
      if (r && r.isOk) {
        this.items = r.result.items || [];
        const pi = r.result.pagingInfo;
        if (pi) {
          this.pageSize = pi.pageSize || this.pageSize;
          this.pageNumber = pi.pageNumber || this.pageNumber;
          this.totalItems = pi.totalItems || 0;
        } else {
          this.totalItems = r.result.pagingInfo?.totalItems || 0;
        }
      }
      if (callback) callback();
    }, () => {
      this.loading = false;
      if (callback) callback();
    });
  }

  private loadStatusOptions(): void {
    this.svc.getTrangThaiToaNhaForSelector().subscribe(r => {
      if (r && r.isOk && Array.isArray(r.result)) {
        this.statusOptions = r.result;
        const dict: { [id: number]: any } = {};
        for (const s of this.statusOptions) {
          if (s && s.id != null) dict[s.id] = s;
        }
        this.statusDict = dict;
      }
    });
  }

  onSearch(): void {
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

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNumber = 1;
    this.load();
  }

  onRefresh(): void {
    this.keyword = '';
    this.sortCol = '';
    this.isAsc = true;
    this.pageNumber = 1;
    this.setOfCheckedId.clear();
    this.load();
  }

  getStatusName(id?: number, fallbackName?: string): string {
    if (id == null) return fallbackName || 'Không rõ';
    const st = this.statusDict[id];
    const name = (st && (st.name || st.ten || st.label)) as string | undefined;
    return name || fallbackName || 'Không rõ';
  }

  getStatusClass(id?: number): string {
    if (id == null) return 'status-unknown';
    const st = this.statusDict[id];
    const rawName = (st && (st.name || st.ten || st.label)) as string | undefined;
    const name = (rawName || '').toLowerCase();
    if (name.includes('ngừng') || name.includes('ngưng')) return 'status-stopped';
    if (name.includes('hoạt động')) return 'status-active';
    if (name.includes('bảo trì')) return 'status-maintain';
    return 'status-unknown';
  }

  deleteSelected(id?: number): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Xóa tòa nhà',
      nzContent: 'Bạn có chắc chắn muốn xóa tòa nhà này?',
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => {
        this.loading = true;
        this.svc.deleteToaNha([id]).subscribe(() => {
          this.notification.success('Đã xóa', 'Xóa tòa nhà thành công');
          this.load();
        }, (err: any) => {
          this.loading = false;
          this.notification.error('Lỗi', err?.message || 'Xóa thất bại');
        });
      }
    });
  }

  deleteSelectedMultiple(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) {
      this.notification.warning('Thông báo', 'Chưa chọn tòa nhà nào');
      return;
    }
    this.modal.confirm({
      nzTitle: 'Xóa tòa nhà đã chọn',
      nzContent: `Bạn có chắc chắn muốn xóa ${ids.length} tòa nhà đã chọn?`,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => {
        this.loading = true;
        this.svc.deleteToaNha(ids).subscribe(() => {
          this.setOfCheckedId.clear();
          this.notification.success('Đã xóa', `Xóa ${ids.length} tòa nhà thành công`);
          this.load();
        }, (err: any) => {
          this.loading = false;
          this.notification.error('Lỗi', err?.message || 'Xóa thất bại');
        });
      }
    });
  }

  editToaNha(id?: number): void {
    if (!id) return;
    this.editingId = id;
    this.modalTitle = 'Cập nhật tòa nhà';
    this.isModalVisible = true;
  }

  openCreateModal(): void {
    this.editingId = undefined;
    this.modalTitle = 'Tạo tòa nhà';
    this.isModalVisible = true;
  }

  cancelModal(): void {
    this.isModalVisible = false;
    this.saving = false;
  }

  onModalOk(): void {
    this.saving = true;
    this.formComp?.save();
  }

  onDone(): void {
    this.saving = false;
  }

  onSaved(): void {
    this.saving = false;
    this.isModalVisible = false;
    const msg = this.editingId ? 'Cập nhật tòa nhà thành công' : 'Tạo tòa nhà thành công';
    this.load(() => this.notification.success('Thành công', msg));
  }

  expandedIds = new Set<number>();

  toggleRow(id?: number | null): void {
    if (id == null) return;
    if (this.expandedIds.has(id)) this.expandedIds.delete(id);
    else this.expandedIds.add(id);
  }

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

  onAllChecked(value: boolean): void {
    this.listOfCurrentPageData.forEach(item => this.updateCheckedSet((item as any).id, value));
    this.refreshCheckedStatus();
  }

  onCurrentPageDataChange($event: readonly ToaNha[]): void {
    this.listOfCurrentPageData = $event;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    this.checked = this.listOfCurrentPageData.every(item => this.setOfCheckedId.has((item as any).id));
    this.indeterminate = this.listOfCurrentPageData.some(item => this.setOfCheckedId.has((item as any).id)) && !this.checked;
  }
}
