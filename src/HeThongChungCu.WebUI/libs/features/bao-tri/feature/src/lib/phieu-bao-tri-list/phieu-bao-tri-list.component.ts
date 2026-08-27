import { Component, OnInit } from '@angular/core';
import { BaoTriService } from '@features/bao-tri/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzDrawerService } from 'ng-zorro-antd/drawer';
import { PhieuBaoTriDetailComponent } from './phieu-bao-tri-detail.component';
import { PhieuBaoTriFormComponent } from './phieu-bao-tri-form.component';

@Component({
  selector: 'app-phieu-bao-tri-list',
  templateUrl: './phieu-bao-tri-list.component.html',
})
export class PhieuBaoTriListComponent implements OnInit {
  loading = false;
  exportLoading = false;
  items: any[] = [];
  trangThaiPhieuBaoTriOptions: any[] = [];

  keyword = '';
  trangThaiPhieuBaoTriId: number | null = null;
  thietBiId: number | null = null;

  pageSize = 10;
  pageNumber = 1;
  totalItems = 0;

  thietBiOptions: any[] = [];
  advancedVisible = false;

  constructor(
    private baoTriService: BaoTriService,
    private notification: NzNotificationService,
    private modal: NzModalService,
    private drawerService: NzDrawerService
  ) {}

  ngOnInit(): void {
    this.loadThietBis();
    this.loadTrangThais();
    this.load();
  }

  loadTrangThais(): void {
    this.baoTriService.getTrangThaiPhieuBaoTriForSelector().subscribe((res) => {
      this.trangThaiPhieuBaoTriOptions = res.result || [];
    });
  }

  loadThietBis(): void {
    this.baoTriService
      .getThietBiList({
        pageSize: 1000,
        pageNumber: 1,
        sortCol: 'id',
        isAsc: false,
      })
      .subscribe((res) => {
        this.thietBiOptions = res.result?.items || [];
      });
  }

  load(): void {
    this.loading = true;
    const query = {
      keyword: this.keyword,
      trangThaiPhieuBaoTriId: this.trangThaiPhieuBaoTriId,
      thietBiId: this.thietBiId,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortCol: 'id',
      isAsc: false,
    };

    this.baoTriService.getPhieuBaoTriList(query).subscribe({
      next: (res: ApiResponse<any>) => {
        this.items = res.result?.items || [];
        this.totalItems = res.result?.pagingInfo?.totalItems || 0;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải danh sách phiếu bảo trì');
      },
    });
  }

  onRefresh(): void {
    this.keyword = '';
    this.trangThaiPhieuBaoTriId = null;
    this.thietBiId = null;
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  openDetail(item: any): void {
    const drawerRef = this.drawerService.create({
      nzTitle: 'Phiếu bảo trì: ' + item.maPhieu,
      nzContent: PhieuBaoTriDetailComponent,
      nzContentParams: { phieuId: item.id },
      nzWidth: 1000,
    });

    drawerRef.afterClose.subscribe(() => {
      this.load();
    });
  }

  openCreate(): void {
    const modal = this.modal.create({
      nzTitle: 'Lập phiếu bảo trì mới',
      nzContent: PhieuBaoTriFormComponent,
      nzFooter: null,
      nzWidth: 800,
      nzBodyStyle: { 'max-height': '70vh', 'overflow': 'auto' },
    });

    modal.afterClose.subscribe((res) => {
      if (res) this.load();
    });
  }

  getStatusColor(statusId: number): string {
    switch (statusId) {
      case 1:
        return 'blue';
      case 2:
        return 'orange';
      case 3:
        return 'cyan';
      case 4:
        return 'green';
      case 5:
        return 'red';
      default:
        return 'default';
    }
  }

  exportExcel(id: number): void {
    this.exportLoading = true;
    this.baoTriService.exportPhieuBaoTri(id).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `phieu-bao-tri_${id}_${new Date().toISOString().slice(0, 10)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.exportLoading = false;
        this.notification.success('Thành công', 'Xuất Excel phiếu bảo trì thành công');
      },
      error: () => {
        this.exportLoading = false;
        this.notification.error('Lỗi', 'Xuất Excel thất bại');
      },
    });
  }
}
