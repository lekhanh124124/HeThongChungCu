import { Component, OnInit } from '@angular/core';
import { ChiSoTieuThuService } from '@features/dich-vu/data-access';
import { ChungCuService } from '@features/resident/data-access';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { Router, ActivatedRoute } from '@angular/router';
import { ChiSoTieuThuImportComponent } from '../chi-so-tieu-thu-import/chi-so-tieu-thu-import.component';
import { ChiSoTieuThuFormComponent } from '../chi-so-tieu-thu-form/chi-so-tieu-thu-form.component';

@Component({
  selector: 'app-chi-so-tieu-thu-list',
  templateUrl: './chi-so-tieu-thu-list.component.html',
  styleUrls: ['./chi-so-tieu-thu-list.component.scss'],
})
export class ChiSoTieuThuListComponent implements OnInit {
  list: any[] = [];
  loading = false;
  total = 0;
  pageNumber = 1;
  pageSize = 10;

  // Filters
  thang: number | null = null;
  nam: number | null = null;
  toaNhaId: number | null = null;
  tangId: number | null = null;
  canHoId: number | null = null;
  dichVuId: number | null = null;
  trangThaiChiSoId: number | null = null;

  // Selectors
  dichVuOptions: any[] = [];
  trangThaiOptions: any[] = [];

  // Selection
  setOfCheckedId = new Set<number>();
  checked = false;
  indeterminate = false;

  constructor(
    private chiSoService: ChiSoTieuThuService,
    private chungCuService: ChungCuService,
    private modal: NzModalService,
    private notification: NzNotificationService,
    private router: Router,
    private route: ActivatedRoute,
  ) {}

  ngOnInit(): void {
    this.loadSelectors();

    // Listen to query params from tree component
    this.route.queryParams.subscribe((params) => {
      this.toaNhaId = params['toaNhaId'] ? Number(params['toaNhaId']) : null;
      this.tangId = params['tangId'] ? Number(params['tangId']) : null;
      this.canHoId = params['canHoId'] ? Number(params['canHoId']) : null;
      this.onSearch();
    });
  }

  loadSelectors(): void {
    this.chiSoService.getListDichVuTieuThu().subscribe((res) => {
      this.dichVuOptions = res.result || [];
    });
    this.chiSoService.getTrangThaiChiSoForSelector().subscribe((res) => {
      this.trangThaiOptions = res.result || [];
    });
  }

  load(): void {
    this.loading = true;
    const query = {
      thang: this.thang || undefined,
      nam: this.nam || undefined,
      toaNhaId: this.toaNhaId || undefined,
      tangId: this.tangId || undefined,
      canHoId: this.canHoId || undefined,
      dichVuId: this.dichVuId || undefined,
      trangThaiChiSoId: this.trangThaiChiSoId || undefined,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortCol: 'ngayGhiNhan',
      isAsc: false,
    };

    this.chiSoService.getList(query).subscribe({
      next: (res) => {
        this.list = res.result?.items || [];
        this.total = res.result?.pagingInfo?.totalItems || 0;
        this.loading = false;
        this.refreshCheckedStatus();
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải danh sách chỉ số');
      },
    });
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.load();
  }

  onRefresh(): void {
    this.thang = null;
    this.nam = null;
    this.toaNhaId = null;
    this.tangId = null;
    this.canHoId = null;
    this.dichVuId = null;
    this.trangThaiChiSoId = null;
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  // Checkbox logic
  updateCheckedSet(id: number, checked: boolean): void {
    if (checked) this.setOfCheckedId.add(id);
    else this.setOfCheckedId.delete(id);
  }

  onItemChecked(id: number, checked: boolean): void {
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(value: boolean): void {
    this.list.forEach((item) => this.updateCheckedSet(item.id, value));
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    this.checked =
      this.list.length > 0 &&
      this.list.every((item) => this.setOfCheckedId.has(item.id));
    this.indeterminate =
      this.list.some((item) => this.setOfCheckedId.has(item.id)) &&
      !this.checked;
  }

  // Actions
  exportTemplate(): void {
    if (!this.thang || !this.nam) {
      this.notification.warning('Cảnh báo', 'Vui lòng chọn tháng và năm');
      return;
    }
    const payload = {
      toaNhaId: this.toaNhaId || undefined,
      tangId: this.tangId || undefined,
      dichVuId: this.dichVuId || undefined,
      thang: this.thang!,
      nam: this.nam!,
    };
    this.chiSoService.exportExcel(payload).subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `Mau_Ghi_Chi_So_${this.thang ?? ''}_${this.nam ?? ''}.xlsx`;
      a.click();
    });
  }

  confirmBulk(): void {
    if (!this.dichVuId) {
      this.notification.warning(
        'Cảnh báo',
        'Vui lòng chọn dịch vụ để xác nhận hàng loạt',
      );
      return;
    }
    if (!this.thang || !this.nam) {
      this.notification.warning('Cảnh báo', 'Vui lòng chọn tháng và năm');
      return;
    }
    this.modal.confirm({
      nzTitle: 'Xác nhận hàng loạt',
      nzContent: `Bạn có chắc chắn muốn xác nhận tất cả chỉ số của tháng ${this.thang}/${this.nam} cho dịch vụ đã chọn?`,
      nzOnOk: () => {
        this.chiSoService
          .confirm({
            thang: this.thang!,
            nam: this.nam!,
            dichVuId: this.dichVuId!,
          })
          .subscribe((res) => {
            if (res.isOk) {
              this.notification.success('Thành công', 'Đã xác nhận thành công');
              this.load();
            }
          });
      },
    });
  }

  deleteSelected(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (ids.length === 0) return;
    this.modal.confirm({
      nzTitle: 'Xác nhận xóa',
      nzContent: `Bạn có chắc chắn muốn xóa ${ids.length} chỉ số đã chọn?`,
      nzOkDanger: true,
      nzOnOk: () => {
        this.chiSoService.delete(ids).subscribe((res) => {
          if (res.isOk) {
            this.notification.success('Thành công', 'Đã xóa thành công');
            this.setOfCheckedId.clear();
            this.load();
          }
        });
      },
    });
  }

  // Modal placeholders (to be implemented)
  openManualEntry(): void {
    this.router.navigate(['/dashboard/dich-vu/chi-so-tieu-thu/ghi-nhan']);
  }

  openImportModal(): void {
    const modalRef = this.modal.create({
      nzTitle: 'Nhập dữ liệu & Ảnh đồng hồ',
      nzContent: ChiSoTieuThuImportComponent,
      nzComponentParams: {
        thang: this.thang || undefined,
        nam: this.nam || undefined,
      },
      nzFooter: null,
      nzWidth: 600,
      nzBodyStyle: { 'max-height': '70vh', 'overflow': 'auto' },
    });

    modalRef.componentInstance?.success.subscribe(() => {
      modalRef.close();
      this.load();
    });
  }

  editItem(item: any): void {
    const modalRef = this.modal.create({
      nzTitle: 'Cập nhật chỉ số tiêu thụ',
      nzContent: ChiSoTieuThuFormComponent,
      nzComponentParams: {
        itemId: item.id,
      },
      nzFooter: null,
      nzWidth: 600,
      nzBodyStyle: { 'max-height': '70vh', 'overflow': 'auto' },
    });

    modalRef.componentInstance?.saved.subscribe(() => {
      modalRef.close();
      this.load();
    });

    modalRef.componentInstance?.cancelled.subscribe(() => {
      modalRef.close();
    });
  }

  deleteItem(item: any): void {
    this.modal.confirm({
      nzTitle: 'Xác nhận xóa',
      nzContent: `Bạn có chắc chắn muốn xóa chỉ số tiêu thụ của căn hộ "${item.maCanHo}" tháng ${item.thang}/${item.nam}?`,
      nzOkDanger: true,
      nzOnOk: () => {
        this.chiSoService.delete([item.id]).subscribe((res) => {
          if (res.isOk) {
            this.notification.success('Thành công', 'Đã xóa chỉ số tiêu thụ');
            this.load();
          }
        });
      },
    });
  }

  getStatusColor(statusId: number): string {
    switch (statusId) {
      case 1:
        return 'blue';
      case 2:
        return 'green';
      case 3:
        return 'gold';
      default:
        return 'default';
    }
  }

  getConsumption(item: any): number {
    return (item.chiSoMoi || 0) - (item.chiSoCu || 0);
  }
}
