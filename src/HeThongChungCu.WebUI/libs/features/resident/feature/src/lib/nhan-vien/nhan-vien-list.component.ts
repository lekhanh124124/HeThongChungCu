import { Component, OnInit, ViewChild } from '@angular/core';
import { ChungCuService } from '@features/resident/data-access';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NhanVienFormComponent } from './nhan-vien-form.component';

@Component({
  selector: 'app-nhan-vien-list',
  templateUrl: './nhan-vien-list.component.html',
  styleUrls: ['./nhan-vien-list.component.scss']
})
export class NhanVienListComponent implements OnInit {
  items: any[] = [];
  loading = false;
  pageNumber = 1;
  pageSize = 10;
  total = 0;
  keyword = '';
  // advanced filters
  advancedVisible = false;
  loaiNhanVienId = 0;
  trangThaiNhanVienId = 0;
  loaiNhanVienOptions: any[] = [];
  trangThaiNhanVienOptions: any[] = [];
  // selection
  setOfCheckedId = new Set<number>();
  checked = false;
  indeterminate = false;
  currentPageData: readonly any[] = [];

  constructor(
    private chungCu: ChungCuService,
    private modal: NzModalService,
    private notification: NzNotificationService
  ) {}

  @ViewChild(NhanVienFormComponent) formComp?: NhanVienFormComponent;
  isModalVisible = false;
  modalTitle = '';
  saving = false;
  editingId?: number | undefined;

  ngOnInit(): void {
    this.load();
    this.loadSelectors();
  }

  load(): void {
    this.loading = true;
    const q = { keyword: this.keyword, pageNumber: this.pageNumber, pageSize: this.pageSize };
    if (this.loaiNhanVienId) (q as any).loaiNhanVienId = this.loaiNhanVienId;
    if (this.trangThaiNhanVienId) (q as any).trangThaiNhanVienId = this.trangThaiNhanVienId;

    this.chungCu.getNhanVienList(q).subscribe((res: any) => {
      this.loading = false;
      if (res && res.isOk && res.result) {
        this.items = res.result.items || [];
        this.total = (res.result.pagingInfo && res.result.pagingInfo.totalItems) || 0;
      }
    }, () => this.loading = false);
  }

  private loadSelectors(): void {
    this.chungCu.getLoaiNhanVienForSelector().subscribe((r: any) => {
      if (r && r.isOk && Array.isArray(r.result)) this.loaiNhanVienOptions = r.result;
    });
    this.chungCu.getTrangThaiNhanVienForSelector().subscribe((r: any) => {
      if (r && r.isOk && Array.isArray(r.result)) this.trangThaiNhanVienOptions = r.result;
    });
  }

  toggleAdvanced(): void { this.advancedVisible = !this.advancedVisible; }

  applyAdvanced(): void { this.pageNumber = 1; this.load(); }

  openAdvanced(): void { this.advancedVisible = true; }

  onPageChange(p: number): void { this.pageNumber = p; this.load(); }
  onPageSizeChange(s: number): void { this.pageSize = s; this.pageNumber = 1; this.load(); }

  onCurrentPageDataChange(list: readonly any[]): void {
    this.currentPageData = list || [];
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    const allChecked = this.currentPageData.length > 0 && this.currentPageData.every(item => this.setOfCheckedId.has(item.id));
    const noneChecked = this.currentPageData.every(item => !this.setOfCheckedId.has(item.id));
    this.checked = allChecked;
    this.indeterminate = !allChecked && !noneChecked;
  }

  onAllChecked(checked: boolean): void {
    this.currentPageData.forEach(item => {
      if (checked) this.setOfCheckedId.add(item.id);
      else this.setOfCheckedId.delete(item.id);
    });
    this.refreshCheckedStatus();
  }

  onItemChecked(id: number, checked: boolean): void {
    if (checked) this.setOfCheckedId.add(id); else this.setOfCheckedId.delete(id);
    this.refreshCheckedStatus();
  }

  create(): void {
    this.editingId = undefined;
    this.modalTitle = 'Tạo nhân viên';
    this.isModalVisible = true;
  }

  edit(id: number): void {
    if (!id) return;
    this.editingId = id;
    this.modalTitle = 'Cập nhật nhân viên';
    this.isModalVisible = true;
  }

  cancelModal(): void {
    this.isModalVisible = false;
    this.saving = false;
  }

  onModalOk(): void {
    if (!this.formComp) { this.notification.warning('Thông báo', 'Form chưa sẵn sàng, vui lòng thử lại'); return; }
    this.saving = true;
    this.formComp.submit();
  }

  onSaved(success: boolean): void {
    this.saving = false;
    if (success) {
      this.isModalVisible = false;
      this.notification.success('Thành công', 'Lưu nhân viên thành công');
      this.load();
    }
  }

  deleteOne(id: number): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Xóa nhân viên',
      nzContent: 'Bạn có chắc chắn muốn xóa nhân viên này?',
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => {
        this.chungCu.deleteNhanVien([id]).subscribe({
          next: res => {
            if (res && res.isOk) this.notification.success('Đã xóa', 'Xóa nhân viên thành công');
            else this.notification.error('Lỗi', 'Xóa nhân viên thất bại');
            this.load();
          },
          error: () => {
            this.notification.error('Lỗi', 'Xóa nhân viên thất bại');
          }
        });
      }
    });
  }

  deleteSelectedMultiple(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) {
      this.notification.warning('Thông báo', 'Chưa chọn nhân viên nào');
      return;
    }
    this.modal.confirm({
      nzTitle: 'Xóa nhân viên đã chọn',
      nzContent: `Bạn có chắc chắn muốn xóa ${ids.length} nhân viên đã chọn?`,
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => {
        this.chungCu.deleteNhanVien(ids).subscribe({
          next: res => {
            if (res && res.isOk) this.notification.success('Đã xóa', `Xóa ${ids.length} nhân viên thành công`);
            else this.notification.warning('Cảnh báo', 'Một số nhân viên có thể chưa được xóa');
            this.setOfCheckedId.clear();
            this.load();
          },
          error: () => {
            this.notification.error('Lỗi', 'Xóa nhân viên thất bại');
          }
        });
      }
    });
  }

  onRefresh(): void {
    this.keyword = '';
    this.pageNumber = 1;
    this.setOfCheckedId.clear();
    this.load();
  }
}
