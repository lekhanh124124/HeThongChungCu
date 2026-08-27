import { Component, OnInit } from '@angular/core';
import { BaoTriService } from '@features/bao-tri/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';
import { HangMucFormComponent } from './hang-muc-form.component';

@Component({
  selector: 'app-hang-muc-list',
  templateUrl: './hang-muc-list.component.html',
})
export class HangMucListComponent implements OnInit {
  loading = false;
  items: any[] = [];

  keyword = '';

  pageSize = 10;
  pageNumber = 1;
  totalItems = 0;

  constructor(
    private baoTriService: BaoTriService,
    private notification: NzNotificationService,
    private modal: NzModalService,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    const query = {
      keyword: this.keyword,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortCol: 'id',
      isAsc: false,
    };

    this.baoTriService.getHangMucList(query).subscribe({
      next: (res: ApiResponse<any>) => {
        this.items = res.result?.items || [];
        this.totalItems = res.result?.pagingInfo?.totalItems || 0;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải danh sách hạng mục');
      },
    });
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  openForm(item?: any): void {
    const modal = this.modal.create({
      nzTitle: item ? 'Cập nhật hạng mục' : 'Thêm hạng mục mới',
      nzContent: HangMucFormComponent,
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
      nzContent: `Bạn có chắc chắn muốn xóa hạng mục "${item.tenHangMuc}"?`,
      nzOkDanger: true,
      nzOnOk: () => {
        this.baoTriService
          .deleteHangMuc(item.id)
          .subscribe((res: ApiResponse<any>) => {
            if (res.isOk) {
              this.notification.success('Thành công', 'Đã xóa hạng mục');
              this.load();
            }
          });
      },
    });
  }
}
