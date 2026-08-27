import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { DichVuService } from '@features/dich-vu/data-access';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { KhungGioFormComponent } from '../khung-gio-form/khung-gio-form.component';

@Component({
  selector: 'app-khung-gio-list',
  templateUrl: './khung-gio-list.component.html',
  styleUrls: ['./khung-gio-list.component.scss']
})
export class KhungGioListComponent implements OnInit, OnChanges {
  @Input() inModal = false;
  @Input() dichVuId?: number | null;

  list: any[] = [];
  loading = false;
  ngayTrongTuanOptions: any[] = [];

  pageNumber = 1;
  pageSize = 10;
  total = 0;
  keyword = '';

  @ViewChild(KhungGioFormComponent) formComp?: KhungGioFormComponent;
  isModalVisible = false;
  modalTitle = '';
  saving = false;
  editingId?: number | null;
  readOnly = false;

  constructor(
    private dichVuService: DichVuService,
    private modal: NzModalService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    if (this.dichVuId == null) {
      this.load();
    }
    this.loadNgayTrongTuanOptions();
  }

  private loadNgayTrongTuanOptions(): void {
    this.dichVuService.getNgayTrongTuanForSelector().subscribe({
      next: (r: any) => {
        if (!r) return;
        if (r.isOk && Array.isArray(r.result)) this.ngayTrongTuanOptions = r.result;
        else if (Array.isArray(r.result ?? r)) this.ngayTrongTuanOptions = r.result ?? r;
      },
      error: () => {}
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.dichVuId) {
      this.pageNumber = 1;
      this.load();
    }
  }

  load(): void {
    if (!this.dichVuId) {
      this.list = [];
      this.total = 0;
      return;
    }
    this.loading = true;
    const q: any = {
      dichVuId: this.dichVuId,
      keyword: this.keyword || undefined,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      isActive: true
    };
    this.dichVuService.getKhungGioList(q).subscribe({
      next: (res: any) => {
        this.loading = false;
        const items = res?.result?.items ?? res?.result ?? res?.data ?? [];
        this.list = Array.isArray(items) ? items : [];
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

  openCreateModal(): void {
    if (!this.dichVuId) {
      this.notification.warning('Thiếu dữ liệu', 'Vui lòng chọn dịch vụ trước.');
      return;
    }
    this.editingId = null;
    this.readOnly = false;
    this.modalTitle = 'Tạo khung giờ';
    this.isModalVisible = true;
    setTimeout(() => this.formComp?.resetForCreate(this.dichVuId as number), 0);
  }

  view(item: any): void {
    if (!item) return;
    this.editingId = item.id;
    this.readOnly = true;
    this.modalTitle = 'Chi tiết khung giờ';
    this.isModalVisible = true;
  }

  activate(item: any): void {
    if (!item || !this.dichVuId) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận kích hoạt',
      nzContent: 'Bạn có chắc chắn muốn kích hoạt khung giờ này?',
      nzOnOk: () => {
        this.dichVuService.activateKhungGio({ dichVuId: this.dichVuId, ids: [item.id] }).subscribe({
          next: () => { this.notification.success('Thành công', 'Đã kích hoạt khung giờ'); this.load(); },
          error: () => this.notification.error('Lỗi', 'Kích hoạt thất bại')
        });
      }
    });
  }

  revoke(item: any): void {
    if (!item || !this.dichVuId) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận thu hồi',
      nzContent: 'Bạn có chắc chắn muốn thu hồi khung giờ này?',
      nzOnOk: () => {
        this.dichVuService.revokeKhungGio({ dichVuId: this.dichVuId, ids: [item.id] }).subscribe({
          next: () => { this.notification.success('Thành công', 'Đã thu hồi khung giờ'); this.load(); },
          error: () => this.notification.error('Lỗi', 'Thu hồi thất bại')
        });
      }
    });
  }

  delete(item: any): void {
    if (!item || !this.dichVuId) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận xóa',
      nzContent: 'Bạn có chắc chắn muốn xóa khung giờ này?',
      nzOkText: 'Xóa',
      nzOkDanger: true,
      nzOnOk: () => {
        this.dichVuService.deleteKhungGio({ dichVuId: this.dichVuId, ids: [item.id] }).subscribe({
          next: () => { this.notification.success('Thành công', 'Đã xóa khung giờ'); this.load(); },
          error: () => this.notification.error('Lỗi', 'Xóa thất bại')
        });
      }
    });
  }

  cancelModal(): void {
    this.isModalVisible = false;
    this.saving = false;
    this.readOnly = false;
  }

  onModalOk(): void {
    if (this.readOnly) {
      this.cancelModal();
      return;
    }
    this.saving = true;
    this.formComp?.save();
  }

  onSaved(): void {
    this.saving = false;
    this.isModalVisible = false;
    this.readOnly = false;
    this.load();
    this.notification.success('Thành công', 'Lưu khung giờ thành công');
  }

  getNgayTrongTuanLabel(value: any): string {
    if (value === null || value === undefined) return 'Mọi ngày';
    const v = Number(value);
    const opt = (this.ngayTrongTuanOptions || []).find((d: any) => {
      const id = (d && d.value !== undefined && d.value !== null) ? d.value : d.id;
      return id === v;
    });
    if (opt) return opt.name ?? opt.label ?? opt.ten ?? opt.text ?? String(opt.id ?? v);
    return 'Thứ ' + (v + 1);
  }

}
