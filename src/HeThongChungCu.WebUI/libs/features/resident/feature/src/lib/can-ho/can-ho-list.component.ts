import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ChungCuService, CanHo } from '@features/resident/data-access';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { CanHoFormComponent } from './can-ho-form.component';

@Component({
  selector: 'app-can-ho-list',
  templateUrl: './can-ho-list.component.html',
  styleUrls: ['./can-ho-list.component.scss']
})
export class CanHoListComponent implements OnInit {
  items: CanHo[] = [];
  toaNhaId?: number;
  toaNhaName?: string;
  tangId?: number;
  tangName?: string;
  loading = false;

  // catalog trạng thái căn hộ
  statusOptions: any[] = [];
  statusDict: { [id: number]: any } = {};

  // search / paging / selection state
  keyword = '';
  pageNumber = 1;
  pageSize = 10;
  totalItems = 0;
  sortCol = '';
  isAsc = true;

  listOfCurrentPageData: readonly CanHo[] = [];
  setOfCheckedId = new Set<number>();
  checked = false;
  indeterminate = false;
  @ViewChild(CanHoFormComponent) formComp?: CanHoFormComponent;
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
      this.tangId = q['tangId'] ? +q['tangId'] : undefined;
      this.tangName = q['tangName'];
      this.load();
    });
    this.loadTinhTrangOptions();
  }

  private loadTinhTrangOptions(): void {
    this.svc.getTinhTrangCanHoForSelector().subscribe(r => {
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

  getTinhTrangName(id?: number, fallbackName?: string): string {
    if (id == null) return fallbackName || 'Không rõ';
    const st = this.statusDict[id];
    const name = (st && (st.name || st.ten || st.label)) as string | undefined;
    return name || fallbackName || 'Không rõ';
  }
  getStatusClass(id?: number): string {
    if (id != null) {
      switch (id) {
        case 1: return 'status-chua-ban-giao';
        case 2: return 'status-dang-trong';
        case 3: return 'status-co-cu-dan';
        case 4: return 'status-dang-thi-cong';
        default: break;
      }
    }

    const st = id != null ? this.statusDict[id] : null;
    const raw = (st && (st.name || st.ten || st.label)) as string | undefined || '';
    const name = (raw || '').toLowerCase();
    if (!name) return 'status-unknown';

    const normalized = name
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd');

    if (normalized.includes('trong') || normalized.includes('empty')) return 'status-dang-trong';
    if (normalized.includes('thue') || normalized.includes('da thue') || normalized.includes('dat thue')) return 'status-co-cu-dan';
    if (normalized.includes('ban') || normalized.includes('daban')) return 'status-chua-ban-giao';
    if (normalized.includes('thi cong') || normalized.includes('baotri') || normalized.includes('bao tri')) return 'status-dang-thi-cong';
    return 'status-unknown';
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

  onCurrentPageDataChange($event: readonly CanHo[]): void {
    this.listOfCurrentPageData = $event;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    this.checked = this.listOfCurrentPageData.every(item => this.setOfCheckedId.has((item as any).id));
    this.indeterminate = this.listOfCurrentPageData.some(item => this.setOfCheckedId.has((item as any).id)) && !this.checked;
  }

  editCanHo(id?: number): void {
    if (!id) return;
    this.editingId = id;
    this.modalTitle = 'Cập nhật căn hộ';
    this.isModalVisible = true;
  }

  openCreateModal(): void {
    this.editingId = undefined;
    this.modalTitle = 'Tạo căn hộ';
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
    if (this.editingId) {
      this.notification.success('Thành công', 'Cập nhật căn hộ thành công');
    } else {
      this.notification.success('Thành công', 'Tạo căn hộ thành công');
    }
  }

  // onSavedNotify(): void {
  //   this.saving = false;
  //   this.isModalVisible = false;
  //   this.load();
  //   if (this.editingId) {
  //     this.notification.success('Thành công', 'Cập nhật căn hộ thành công');
  //   } else {
  //     this.notification.success('Thành công', 'Tạo căn hộ thành công');
  //   }
  // }

  onDone(): void {
    this.saving = false;
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

  load(): void {
    this.loading = true;
    const query: any = {
      tangId: this.tangId || undefined,
      keyword: this.keyword || undefined,
      sortCol: this.sortCol || undefined,
      isAsc: this.isAsc,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };
    this.svc.getCanHoList(query).subscribe({
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
      nzTitle: 'Xóa căn hộ',
      nzContent: 'Bạn có chắc chắn muốn xóa căn hộ này?',
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => this.svc.deleteCanHo([id!]).subscribe({
        next: (res: any) => {
          if (res && res.isOk) {
            this.notification.success('Thành công', 'Xóa căn hộ thành công');
            this.load();
          } else {
            this.notification.error('Lỗi', 'Xóa căn hộ thất bại');
          }
        },
        error: () => this.notification.error('Lỗi', 'Xóa căn hộ thất bại')
      })
    });
  }

  deleteSelectedMultiple(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) {
      this.notification.warning('Thông báo', 'Chưa chọn căn hộ nào');
      return;
    }
    this.modal.confirm({
      nzTitle: 'Xóa căn hộ đã chọn',
      nzContent: `Bạn có chắc chắn muốn xóa ${ids.length} căn hộ đã chọn?`,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => this.svc.deleteCanHo(ids).subscribe({
        next: (res: any) => {
          if (res && res.isOk) {
            this.notification.success('Thành công', `Xóa ${ids.length} căn hộ thành công`);
          } else {
            this.notification.warning('Cảnh báo', 'Một số căn hộ có thể chưa được xóa');
          }
          this.setOfCheckedId.clear();
          this.load();
        },
        error: () => this.notification.error('Lỗi', 'Xóa căn hộ thất bại')
      })
    });
  }
}
