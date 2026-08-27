import { Component, OnInit, ViewChild, Input, OnChanges, SimpleChanges } from '@angular/core';
import { DichVuService } from '@features/dich-vu/data-access';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { BangGiaFormComponent } from '../bang-gia-form/bang-gia-form.component';

@Component({
  selector: 'app-bang-gia-list',
  templateUrl: './bang-gia-list.component.html',
  styleUrls: ['./bang-gia-list.component.scss'],
})
export class BangGiaListComponent implements OnInit, OnChanges {
  list: any[] = [];
  loading = false;
  @Input() inModal = false;
  @Input() dichVuId?: number | null;

  // paging
  pageNumber = 1;
  pageSize = 10;
  total = 0;
  keyword = '';

  // modal / form
  @ViewChild(BangGiaFormComponent) formComp?: BangGiaFormComponent;
  isModalVisible = false;
  modalTitle = '';
  saving = false;
  editingId?: number | null;
  readOnly = false;

  constructor(
    private dichVuService: DichVuService,
    private modal: NzModalService,
    private notification: NzNotificationService,
  ) {}

  ngOnInit(): void {
    if (this.dichVuId == null) {
      this.load();
    }
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

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.dichVuId) {
      this.dichVuId = changes.dichVuId.currentValue;
      this.pageNumber = 1;
      this.load();
    }
  }

  load(): void {
    this.loading = true;
    const q: any = {
      dichVuId: this.dichVuId || undefined,
      keyword: this.keyword || undefined,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
    };
    this.dichVuService.getBangGiaList(q).subscribe({
      next: (res: any) => {
        this.loading = false;
        let items: any[] = [];
        if (!res) items = [];
        else if (Array.isArray(res)) items = res;
        else if (Array.isArray(res.result)) items = res.result;
        else if (res.result && Array.isArray(res.result.items))
          items = res.result.items;
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
      },
    });
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

  openCreateModal(): void {
    this.editingId = null;
    this.readOnly = false;
    this.modalTitle = 'Tạo bảng giá';
    this.isModalVisible = true;
    setTimeout(() => this.formComp?.resetForCreate(), 50);
  }

  view(item: any): void {
    if (!item) return;
    this.editingId = item.id;
    this.readOnly = true;
    this.modalTitle = 'Chi tiết bảng giá';
    this.isModalVisible = true;
  }

  renew(item: any): void {
    if (!item) return;
    this.editingId = null; // create new
    this.readOnly = false;
    this.modalTitle = 'Gia hạn bảng giá';
    this.isModalVisible = true;
    setTimeout(() => this.formComp?.prefillForRenew(item), 50);
  }

  revoke(item: any): void {
    if (!item) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận vô hiệu hóa',
      nzContent: 'Bạn có chắc chắn muốn vô hiệu hóa bảng giá này?',
      nzOnOk: () => {
        const payload = { dichVuId: item.dichVuId, ids: [item.id] };
        this.dichVuService.revokeBangGia(payload).subscribe({
          next: () => {
            this.notification.success('Thành công', 'Đã vô hiệu hóa bảng giá');
            this.load();
          },
          error: () => this.notification.error('Lỗi', 'Vô hiệu hóa thất bại'),
        });
      },
    });
  }

  activate(item: any): void {
    if (!item) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận kích hoạt',
      nzContent: 'Bạn có chắc chắn muốn kích hoạt bảng giá này?',
      nzOnOk: () => {
        const payload = { dichVuId: item.dichVuId, ids: [item.id] };
        this.dichVuService.activateBangGia(payload).subscribe({
          next: () => {
            this.notification.success('Thành công', 'Đã kích hoạt bảng giá');
            this.load();
          },
          error: () => this.notification.error('Lỗi', 'Kích hoạt thất bại'),
        });
      },
    });
  }

  delete(item: any): void {
    if (!item) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận xóa',
      nzContent: 'Bạn có chắc chắn muốn xóa bảng giá này?',
      nzOkText: 'Xóa',
      nzOkDanger: true,
      nzOnOk: () => {
        const payload = { dichVuId: item.dichVuId, ids: [item.id] };
        this.dichVuService.deleteBangGia(payload).subscribe({
          next: () => {
            this.notification.success('Thành công', 'Đã xóa bảng giá');
            this.load();
          },
          error: () => this.notification.error('Lỗi', 'Xóa thất bại'),
        });
      },
    });
  }

  cancelModal(): void {
    this.isModalVisible = false;
    this.saving = false;
    this.readOnly = false;
  }

  onModalOk(): void {
    this.saving = true;
    this.formComp?.save();
  }

  onSaved(): void {
    this.saving = false;
    this.isModalVisible = false;
    this.readOnly = false;
    this.load();
    this.notification.success('Thành công', 'Lưu bảng giá thành công');
  }

  // helpers: determine status
  getStatusLabel(item: any): string {
    const now = new Date();
    const start = item.ngayApDung ? new Date(item.ngayApDung) : null;
    const end = item.ngayKetThuc ? new Date(item.ngayKetThuc) : null;

    if (!item.isActive) return 'Đã vô hiệu';
    if (end && end < now) return 'Hết hạn';
    if (start && start > now) return 'Sắp tới';
    if (start && start <= now && (!end || end >= now)) return 'Đang áp dụng';
    return 'Không xác định';
  }

  getStatusClasses(item: any): string {
    const label = this.getStatusLabel(item);
    switch (label) {
      case 'Đang áp dụng':
        return 'bg-green-100 text-green-700 border border-green-200';
      case 'Sắp tới':
        return 'bg-blue-100 text-blue-700 border border-blue-200';
      case 'Hết hạn':
      case 'Đã vô hiệu':
        return 'bg-gray-100 text-gray-500 border border-gray-200';
      default:
        return 'bg-gray-50 text-gray-400';
    }
  }

  isActive(item: any): boolean {
    return item.isActive && this.getStatusLabel(item) === 'Đang áp dụng';
  }

  isExpiring(item: any): boolean {
    if (!item.isActive || !item.ngayKetThuc) return false;
    const end = new Date(item.ngayKetThuc);
    const now = new Date();
    const diffDays = Math.ceil(
      (end.getTime() - now.getTime()) / (1000 * 60 * 60 * 24),
    );
    return diffDays >= 0 && diffDays <= 7;
  }

  isExpired(item: any): boolean {
    const label = this.getStatusLabel(item);
    return label === 'Hết hạn' || label === 'Đã vô hiệu';
  }
}
