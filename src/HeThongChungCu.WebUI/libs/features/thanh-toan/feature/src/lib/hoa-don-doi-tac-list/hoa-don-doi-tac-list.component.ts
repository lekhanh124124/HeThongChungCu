import { Component, OnInit } from '@angular/core';
import { ThanhToanService } from '@features/thanh-toan/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';
import { DoiTacApiService } from '@features/doi-tac/data-access';
import { HoaDonDoiTacFormComponent } from './hoa-don-doi-tac-form.component';

@Component({
  selector: 'app-hoa-don-doi-tac-list',
  templateUrl: './hoa-don-doi-tac-list.component.html',
  styleUrls: ['./hoa-don-doi-tac-list.component.scss'],
})
export class HoaDonDoiTacListComponent implements OnInit {
  loading = false;
  items: any[] = [];

  doiTacId: number | null = null;
  thang: number | null = null;
  nam: number | null = null;
  trangThaiThanhToanId: number | null = null;

  doiTacOptions: any[] = [];

  pageSize = 10;
  pageNumber = 1;
  totalItems = 0;

  constructor(
    private thanhToanService: ThanhToanService,
    private doiTacApiService: DoiTacApiService,
    private notification: NzNotificationService,
    private modal: NzModalService,
  ) {}

  ngOnInit(): void {
    this.loadDoiTacs();
    this.load();
  }

  loadDoiTacs(): void {
    this.doiTacApiService
      .getList({ pageSize: 1000, pageNumber: 1 })
      .subscribe((res: ApiResponse<any>) => {
        this.doiTacOptions = res.result?.items || [];
      });
  }

  load(): void {
    this.loading = true;
    const query: any = {
      doiTacId: this.doiTacId || undefined,
      thang: this.thang || undefined,
      nam: this.nam || undefined,
      trangThaiThanhToanId: this.trangThaiThanhToanId || undefined,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
    };
    this.thanhToanService.getHoaDonDoiTacList(query).subscribe({
      next: (res: ApiResponse<any>) => {
        this.items = res.result?.items || [];
        this.totalItems = res.result?.pagingInfo?.totalItems || 0;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error(
          'Lỗi',
          'Không thể tải danh sách hóa đơn đối tác',
        );
      },
    });
  }

  onRefresh(): void {
    this.doiTacId = null;
    this.thang = null;
    this.nam = null;
    this.trangThaiThanhToanId = null;
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  openCreateModal(item?: any): void {
    const modal = this.modal.create({
      nzTitle: item ? 'Cập nhật hóa đơn đối tác' : 'Lập hóa đơn đối tác mới',
      nzContent: HoaDonDoiTacFormComponent,
      nzComponentParams: { item, doiTacOptions: this.doiTacOptions },
      nzFooter: null,
      nzWidth: 800,
      nzBodyStyle: { 'max-height': '70vh', 'overflow': 'auto' },
    });

    modal.afterClose.subscribe((res) => {
      if (res) this.load();
    });
  }

  getStatusColor(statusId: number): string {
    return statusId === 2 ? 'green' : 'orange';
  }

  confirmPayment(item: any): void {
    this.modal.confirm({
      nzTitle: 'Xác nhận thanh toán',
      nzContent: `Bạn đã thực hiện thanh toán ${item.soTien.toLocaleString()}đ cho đối tác ${item.tenDoiTac}?`,
      nzOnOk: () => {
        this.thanhToanService
          .xacNhanThanhToanDoiTac(item.id)
          .subscribe((res: ApiResponse<any>) => {
            if (res.isOk) {
              this.notification.success('Thành công', 'Đã xác nhận thanh toán');
              this.load();
            }
          });
      },
    });
  }

  delete(item: any): void {
    this.modal.confirm({
      nzTitle: 'Xác nhận xóa',
      nzContent: 'Bạn có chắc chắn muốn xóa hóa đơn đối tác này?',
      nzOkDanger: true,
      nzOnOk: () => {
        this.thanhToanService
          .deleteHoaDonDoiTac(item.id)
          .subscribe((res: ApiResponse<any>) => {
            if (res.isOk) {
              this.notification.success('Thành công', 'Đã xóa hóa đơn đối tác');
              this.load();
            }
          });
      },
    });
  }
}
