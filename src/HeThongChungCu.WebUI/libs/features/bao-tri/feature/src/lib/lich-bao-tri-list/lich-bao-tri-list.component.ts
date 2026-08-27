import { Component, OnInit } from '@angular/core';
import { BaoTriService } from '@features/bao-tri/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';
import { LichBaoTriFormComponent } from './lich-bao-tri-form.component';

@Component({
  selector: 'app-lich-bao-tri-list',
  templateUrl: './lich-bao-tri-list.component.html',
})
export class LichBaoTriListComponent implements OnInit {
  loading = false;
  items: any[] = [];

  thietBiId: number | null = null;
  hangMucId: number | null = null;

  pageSize = 10;
  pageNumber = 1;
  totalItems = 0;

  thietBiOptions: any[] = [];
  hangMucOptions: any[] = [];
  tanSuatBaoTriOptions: any[] = [];

  constructor(
    private baoTriService: BaoTriService,
    private notification: NzNotificationService,
    private modal: NzModalService,
  ) {}

  ngOnInit(): void {
    this.loadOptions();
    this.load();
  }

  loadOptions(): void {
    this.baoTriService
      .getThietBiList({
        pageNumber: 1,
        pageSize: 1000,
        sortCol: 'id',
        isAsc: false,
      })
      .subscribe((res) => {
        this.thietBiOptions = res.result?.items || [];
      });
    this.baoTriService
      .getHangMucList({
        pageNumber: 1,
        pageSize: 1000,
        sortCol: 'id',
        isAsc: false,
      })
      .subscribe((res) => {
        this.hangMucOptions = res.result?.items || [];
      });
    this.baoTriService.getTanSuatBaoTriForSelector().subscribe((res) => {
      this.tanSuatBaoTriOptions = res.result || [];
    });
  }

  load(): void {
    this.loading = true;
    const query = {
      thietBiId: this.thietBiId,
      hangMucId: this.hangMucId,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortCol: 'id',
      isAsc: false,
    };

    this.baoTriService.getLichBaoTriList(query).subscribe({
      next: (res: ApiResponse<any>) => {
        this.items = res.result?.items || [];
        this.totalItems = res.result?.pagingInfo?.totalItems || 0;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải danh sách lịch bảo trì');
      },
    });
  }

  onRefresh(): void {
    this.thietBiId = null;
    this.hangMucId = null;
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  openForm(item?: any): void {
    const modal = this.modal.create({
      nzTitle: item ? 'Cập nhật lịch bảo trì' : 'Thiết lập lịch bảo trì mới',
      nzContent: LichBaoTriFormComponent,
      nzComponentParams: {
        item,
        thietBiOptions: this.thietBiOptions,
        hangMucOptions: this.hangMucOptions,
      },
      nzFooter: null,
      nzWidth: 600,
      nzBodyStyle: { 'max-height': '70vh', 'overflow': 'auto' },
    });

    modal.afterClose.subscribe((res) => {
      if (res) this.load();
    });
  }

  quetLich(): void {
    this.modal.confirm({
      nzTitle: 'Xác nhận quét lịch',
      nzContent:
        'Hệ thống sẽ kiểm tra các lịch bảo trì đến hạn và tự động sinh phiếu bảo trì dự thảo. Bạn có muốn tiếp tục?',
      nzOnOk: () => {
        this.loading = true;
        this.baoTriService.quetLichBaoTri().subscribe({
          next: (res: ApiResponse<any>) => {
            if (res.isOk) {
              this.notification.success(
                'Thành công',
                'Đã quét lịch và sinh phiếu bảo trì dự thảo',
              );
              this.load();
            }
            this.loading = false;
          },
          error: () => (this.loading = false),
        });
      },
    });
  }

  getTanSuatLabel(id: number): string {
    const found = this.tanSuatBaoTriOptions.find((o) => o.id === id);
    return found ? found.name : 'Định kỳ';
  }
}
