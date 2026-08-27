import { Component, OnInit, ViewChild } from '@angular/core';
import { DichVuService } from '@features/dich-vu/data-access';
import { DoiTacApiService } from '@features/doi-tac/data-access';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { DichVuFormComponent } from '../dich-vu-form/dich-vu-form.component';

@Component({
  selector: 'app-dich-vu-list',
  templateUrl: './dich-vu-list.component.html',
  styleUrls: ['./dich-vu-list.component.scss']
})
export class DichVuListComponent implements OnInit {
  list: any[] = [];
  loading = false;

  // modal / create-edit
  @ViewChild(DichVuFormComponent) formComp?: DichVuFormComponent;
  isModalVisible = false;
  modalTitle = '';
  saving = false;
  editingId?: number | null;

  // paging / searching / filters
  pageNumber = 1;
  pageSize = 10;
  total = 0;

  keyword = '';
  advancedVisible = false;

  loaiDichVuId: any = null;
  doiTacId: any = null;
  trangThaiDichVuId: any = null;

  loaiDichVuOptions: any[] = [];
  doiTacOptions: any[] = [];
  trangThaiOptions: any[] = [];

  // selection state (checkbox column)
  listOfCurrentPageData: readonly any[] = [];
  setOfCheckedId = new Set<any>();
  checked = false;
  indeterminate = false;

  constructor(
    private dichVuService: DichVuService,
    private doiTacApi: DoiTacApiService,
    private modal: NzModalService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    this.loadSelectors();
    this.load();
  }

  private loadSelectors(): void {
    try {
      this.dichVuService.getLoaiDichVuForSelector({}).subscribe({
        next: (res: any) => {
          this.loaiDichVuOptions = Array.isArray(res) ? res : (res?.result?.items ?? res?.result ?? res?.data ?? []);
        },
        error: () => { this.loaiDichVuOptions = []; }
      });
    } catch (e) { this.loaiDichVuOptions = []; }

    try {
      (this.dichVuService as any).getTrangThaiDichVuForSelector?.({}).subscribe({
        next: (res: any) => {
          this.trangThaiOptions = Array.isArray(res) ? res : (res?.result?.items ?? res?.result ?? res?.data ?? []);
        },
        error: () => { this.trangThaiOptions = []; }
      });
    } catch (e) { this.trangThaiOptions = []; }

    try {
      this.doiTacApi.getList({ pageNumber: 1, pageSize: 1000 }).subscribe({
        next: (res: any) => {
          const items = res?.result?.items ?? res?.result ?? res?.data ?? [];
          this.doiTacOptions = Array.isArray(items) ? items : [];
        },
        error: () => { this.doiTacOptions = []; }
      });
    } catch (e) { this.doiTacOptions = []; }
  }


  load(): void {
    this.loading = true;
    const q: any = {
      keyword: this.keyword || undefined,
      loaiDichVuId: this.loaiDichVuId || undefined,
      doiTacId: this.doiTacId || undefined,
      trangThaiDichVuId: this.trangThaiDichVuId || undefined,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
    };

    this.dichVuService.getDichVuList(q).subscribe({
      next: (res: any) => {
        this.loading = false;
        let items: any[] = [];
        if (!res) items = [];
        else if (Array.isArray(res)) items = res;
        else if (Array.isArray(res.result)) items = res.result;
        else if (res.result && Array.isArray(res.result.items)) items = res.result.items;
        else if (Array.isArray(res.data)) items = res.data;
        else if (Array.isArray(res.items)) items = res.items;
        else {
          const maybe = res.result ?? res.data ?? res;
          items = Array.isArray(maybe) ? maybe : [];
        }

        this.list = items;
        const pi = res?.result?.pagingInfo;
        if (pi) {
          this.pageSize = pi.pageSize || this.pageSize;
          this.pageNumber = pi.pageNumber || this.pageNumber;
          this.total = pi.totalItems || 0;
        } else {
          this.total = this.list.length;
        }
      },
      error: () => {
        this.loading = false;
        this.list = [];
        this.total = 0;
      }
    });
  }

  onSearch(): void { this.pageNumber = 1; this.load(); }

  onRefresh(): void {
    this.keyword = '';
    this.loaiDichVuId = null;
    this.doiTacId = null;
    this.trangThaiDichVuId = null;
    this.pageNumber = 1;
    this.load();
  }

  toggleAdvanced(): void { this.advancedVisible = !this.advancedVisible; }
  applyAdvanced(): void { this.pageNumber = 1; this.load(); this.advancedVisible = false; }
  closeAdvanced(): void {
    this.advancedVisible = false;
  }

  onPageChange(page: number): void { this.pageNumber = page; this.load(); }
  onPageSizeChange(size: number): void { this.pageSize = size; this.pageNumber = 1; this.load(); }


  updateCheckedSet(id: any, checked: boolean): void {
    if (id == null) return;
    if (checked) this.setOfCheckedId.add(id);
    else this.setOfCheckedId.delete(id);
  }

  onCurrentPageDataChange(list: readonly any[]): void {
    this.listOfCurrentPageData = list || [];
    this.refreshCheckedStatus();
  }

  private refreshCheckedStatus(): void {
    const list = this.listOfCurrentPageData && this.listOfCurrentPageData.length ? this.listOfCurrentPageData : (this.list || []);
    if (!list || list.length === 0) {
      this.checked = false;
      this.indeterminate = false;
      return;
    }
    this.checked = list.every(item => this.setOfCheckedId.has(this.getRowId(item)));
    this.indeterminate = list.some(item => this.setOfCheckedId.has(this.getRowId(item))) && !this.checked;
  }

  onAllChecked(value: boolean): void {
    const targetList = (this.listOfCurrentPageData && this.listOfCurrentPageData.length) ? this.listOfCurrentPageData : (this.list || []);
    (targetList || []).forEach(item => this.updateCheckedSet(this.getRowId(item), value));
    this.refreshCheckedStatus();
  }

  onItemChecked(id: any, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  delete(id: any): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận',
      nzContent: 'Bạn có chắc chắn muốn xóa dịch vụ này?',
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this.dichVuService.deleteDichVu([id]).subscribe({
          next: () => {
            this.notification.success('Thành công', 'Đã xóa dịch vụ');
            this.load();
          },
          error: () => this.notification.error('Lỗi', 'Xóa dịch vụ thất bại')
        });
      }
    });
  }

  deleteSelectedMultiple(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận',
      nzContent: `Bạn có chắc chắn muốn xóa ${ids.length} dịch vụ đã chọn?`,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this.dichVuService.deleteDichVu(ids).subscribe({
          next: () => {
            this.notification.success('Thành công', `Đã xóa ${ids.length} dịch vụ`);
            this.setOfCheckedId.clear();
            this.load();
          },
          error: () => this.notification.error('Lỗi', 'Xóa dịch vụ thất bại')
        });
      }
    });
  }

  // Modal / create-edit handlers
  openCreateModal(): void {
    this.editingId = undefined;
    this.modalTitle = 'Tạo dịch vụ';
    this.isModalVisible = true;
  }

  edit(item: any): void {
    if (!item) return;
    this.editingId = this.getRowId(item);
    this.modalTitle = 'Cập nhật dịch vụ';
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
    this.notification.success('Thành công', 'Lưu dịch vụ thành công');
  }

  getRowId(item: any): any {
    if (!item) return null;
    return item.id ?? item.dichVuId ?? item.doiTacId ?? item.maDichVu ?? null;
  }

  detailVisible = false;
  detailId: any = null;

  viewDetail(item: any): void {
    if (!item) return;
    this.detailId = this.getRowId(item);
    this.detailVisible = true;
  }

  onDetailChanged(): void {
    this.load();
  }

  // Activate / revoke helpers (single and multiple)
  activate(id: any): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận kích hoạt',
      nzContent: 'Bạn có chắc chắn muốn kích hoạt mục này?',
      nzOnOk: () => {
        const ids = Array.isArray(id) ? id : [id];
        this.dichVuService.activate(ids).subscribe({
          next: () => { this.notification.success('Thành công', 'Đã kích hoạt'); this.load(); },
          error: () => this.notification.error('Lỗi', 'Kích hoạt thất bại')
        });
      }
    });
  }

  revoke(id: any): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận thu hồi',
      nzContent: 'Bạn có chắc chắn muốn thu hồi mục này?',
      nzOnOk: () => {
        const ids = Array.isArray(id) ? id : [id];
        this.dichVuService.revoke(ids).subscribe({
          next: () => { this.notification.success('Thành công', 'Đã thu hồi'); this.load(); },
          error: () => this.notification.error('Lỗi', 'Thu hồi thất bại')
        });
      }
    });
  }

  activateSelected(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) return;
    this.activate(ids);
  }

  revokeSelected(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) return;
    this.revoke(ids);
  }
}
