import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ChungCuService, Tang } from '@features/resident/data-access';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { TangFormComponent } from './tang-form.component';

@Component({
  selector: 'app-tang-list',
  templateUrl: './tang-list.component.html'
})
export class TangListComponent implements OnInit {
  items: Tang[] = [];
  toaNhaId?: number;
  toaNhaName?: string;
  loading = false;

  // search / paging / selection state (match ToaNha list UX)
  keyword = '';
  pageNumber = 1;
  pageSize = 10;
  sortCol = '';
  isAsc = true;
  totalItems = 0;

  listOfCurrentPageData: readonly Tang[] = [];
  setOfCheckedId = new Set<number>();
  checked = false;
  indeterminate = false;

  // loai tang options
  loaiTangOptions: any[] = [];
  loaiTangDict: { [id: number]: string } = {};

  // modal form state
  @ViewChild(TangFormComponent) formComp?: TangFormComponent;
  isModalVisible = false;
  modalTitle = '';
  editingId?: number;
  saving = false;

  constructor(
    private svc: ChungCuService,
    private route: ActivatedRoute,
    private router: Router,
    private modal: NzModalService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((q: any) => {
      this.toaNhaId = q['toaNhaId'] ? +q['toaNhaId'] : undefined;
      this.toaNhaName = q['toaNhaName'];
      this.load();
    });
    this.loadLoaiTangOptions();
  }

  private loadLoaiTangOptions(): void {
    this.svc.getLoaiTangForSelector().subscribe(r => {
      if (r && r.isOk && Array.isArray(r.result)) {
        this.loaiTangOptions = r.result;
        const dict: { [id: number]: string } = {};
        for (const o of this.loaiTangOptions) {
          if (o && o.id != null) dict[o.id] = o.name || o.ten || o.label || o.title || o.value;
        }
        this.loaiTangDict = dict;
      }
    });
  }

  getLoaiTangName(id?: number): string {
    if (id == null) return '-';
    return this.loaiTangDict[id] || '-';
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.load();
  }

  onRefresh(): void {
    this.keyword = '';
    this.pageNumber = 1;
    this.setOfCheckedId.clear();
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

  toggleSort(col: string): void {
    if (!col) return;
    if (this.sortCol === col) {
      this.isAsc = !this.isAsc;
    } else {
      this.sortCol = col;
      this.isAsc = true;
    }
    this.pageNumber = 1;
    this.load();
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

  onCurrentPageDataChange($event: readonly Tang[]): void {
    this.listOfCurrentPageData = $event;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    this.checked = this.listOfCurrentPageData.every(item => this.setOfCheckedId.has((item as any).id));
    this.indeterminate = this.listOfCurrentPageData.some(item => this.setOfCheckedId.has((item as any).id)) && !this.checked;
  }

  editTang(id?: number): void {
    if (!id) return;
    this.editingId = id;
    this.modalTitle = 'Cập nhật tầng';
    this.isModalVisible = true;
  }

  openCreateModal(): void {
    this.editingId = undefined;
    this.modalTitle = 'Tạo tầng';
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

  onSaved(): void {
    this.saving = false;
    this.isModalVisible = false;
    this.load();
    if (this.editingId) this.notification.success('Thành công', 'Cập nhật tầng thành công');
    else this.notification.success('Thành công', 'Tạo tầng thành công');
  }

  onDone(): void {
    this.saving = false;
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
    if (this.toaNhaId) query.toaNhaId = this.toaNhaId;
    this.svc.getTangList(query).subscribe({
      next: (r: any) => {
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
      },
      error: () => {
        this.loading = false;
        this.items = [];
      }
    });
  }

  deleteOne(id?: number): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Xóa tầng',
      nzContent: 'Bạn có chắc chắn muốn xóa tầng này?',
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => this.svc.deleteTang([id!]).subscribe({
        next: (res: any) => {
          if (res && res.isOk) {
            this.notification.success('Thành công', 'Xóa tầng thành công');
            this.load();
          } else {
            this.notification.error('Lỗi', 'Xóa tầng thất bại');
          }
        },
        error: () => this.notification.error('Lỗi', 'Xóa tầng thất bại')
      })
    });
  }

  deleteSelectedMultiple(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) {
      this.notification.warning('Thông báo', 'Chưa chọn tầng nào');
      return;
    }
    this.modal.confirm({
      nzTitle: 'Xóa tầng đã chọn',
      nzContent: `Bạn có chắc chắn muốn xóa ${ids.length} tầng đã chọn?`,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => this.svc.deleteTang(ids).subscribe({
        next: (res: any) => {
          if (res && res.isOk) this.notification.success('Thành công', `Xóa ${ids.length} tầng thành công`);
          else this.notification.warning('Cảnh báo', 'Một số tầng có thể chưa được xóa');
          this.setOfCheckedId.clear();
          this.load();
        },
        error: () => this.notification.error('Lỗi', 'Xóa tầng thất bại')
      })
    });
  }
}
