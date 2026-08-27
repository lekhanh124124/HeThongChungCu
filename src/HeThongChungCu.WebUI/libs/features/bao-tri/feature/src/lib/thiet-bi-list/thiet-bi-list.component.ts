import { Component, OnInit } from '@angular/core';
import { BaoTriService } from '@features/bao-tri/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';
import { ThietBiFormComponent } from './thiet-bi-form.component';

@Component({
  selector: 'app-thiet-bi-list',
  templateUrl: './thiet-bi-list.component.html',
})
export class ThietBiListComponent implements OnInit {
  loading = false;
  items: any[] = [];
  trangThaiThietBiOptions: any[] = [];

  keyword = '';
  trangThaiThietBiId: number | null = null;

  pageSize = 10;
  pageNumber = 1;
  totalItems = 0;

  constructor(
    private baoTriService: BaoTriService,
    private notification: NzNotificationService,
    private modal: NzModalService,
  ) {}

  ngOnInit(): void {
    this.loadTrangThais();
    this.load();
  }

  loadTrangThais(): void {
    this.baoTriService.getTrangThaiThietBiForSelector().subscribe((res) => {
      this.trangThaiThietBiOptions = res.result || [];
    });
  }

  load(): void {
    this.loading = true;
    const query = {
      keyword: this.keyword,
      trangThaiThietBiId: this.trangThaiThietBiId,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortCol: 'id',
      isAsc: false,
    };

    this.baoTriService.getThietBiList(query).subscribe({
      next: (res: ApiResponse<any>) => {
        this.items = res.result?.items || [];
        this.totalItems = res.result?.pagingInfo?.totalItems || 0;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải danh sách thiết bị');
      },
    });
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  openForm(item?: any): void {
    const modal = this.modal.create({
      nzTitle: item ? 'Cập nhật thiết bị' : 'Thêm thiết bị mới',
      nzContent: ThietBiFormComponent,
      nzComponentParams: { item },
      nzFooter: null,
      nzWidth: 800,
      nzBodyStyle: { 'max-height': '70vh', 'overflow': 'auto' },
    });

    modal.afterClose.subscribe((res) => {
      if (res) this.load();
    });
  }

  delete(item: any): void {
    this.modal.confirm({
      nzTitle: 'Xác nhận xóa',
      nzContent: `Bạn có chắc chắn muốn xóa thiết bị "${item.tenThietBi}"?`,
      nzOkDanger: true,
      nzOnOk: () => {
        this.baoTriService
          .deleteThietBi(item.id)
          .subscribe((res: ApiResponse<any>) => {
            if (res.isOk) {
              this.notification.success('Thành công', 'Đã xóa thiết bị');
              this.load();
            }
          });
      },
    });
  }

  getStatusColor(statusId: number): string {
    switch (statusId) {
      case 1:
        return 'green'; // Hoạt động
      case 2:
        return 'orange'; // Đang bảo trì
      case 3:
        return 'red'; // Hỏng
      case 4:
        return 'gray'; // Thanh lý
      default:
        return 'default';
    }
  }
}
