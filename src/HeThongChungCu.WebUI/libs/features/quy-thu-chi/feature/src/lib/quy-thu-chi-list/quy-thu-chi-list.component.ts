import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { QuyThuChiService, GiaoDichItem } from '@features/quy-thu-chi/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';
import { Router } from '@angular/router';
import { PhieuThuFormComponent } from '../phieu-thu-form/phieu-thu-form.component';
import { PhieuChiFormComponent } from '../phieu-chi-form/phieu-chi-form.component';

@Component({
  selector: 'app-quy-thu-chi-list',
  templateUrl: './quy-thu-chi-list.component.html',
  styleUrls: ['./quy-thu-chi-list.component.scss']
})
export class QuyThuChiListComponent implements OnInit {
  loading = false;
  items: GiaoDichItem[] = [];

  loaiGiaoDichId: number | null = null;
  dichVuId: number | null = null;
  tuNgay: Date | null = null;
  denNgay: Date | null = null;
  keyword = '';

  pageSize = 10;
  pageNumber = 1;
  totalItems = 0;

  detailDrawerVisible = false;
  selectedItem: GiaoDichItem | null = null;
  detailLoading = false;
  detailItem: any = null;

  exportLoading = false;
  advancedVisible = false;
  loaiThuChiOptions: any[] = [];

  constructor(
    private svc: QuyThuChiService,
    private notification: NzNotificationService,
    private modal: NzModalService,
    public router: Router
  ) {}

  ngOnInit(): void {
    this.load();
    this.loadLoaiThuChiOptions();
  }

  loadLoaiThuChiOptions(): void {
    this.svc.getLoaiThuChiForSelector().subscribe({
      next: (res: ApiResponse<any[]>) => {
        this.loaiThuChiOptions = res.result || [];
      }
    });
  }

  load(): void {
    this.loading = true;
    const query: any = {
      loaiGiaoDichId: this.loaiGiaoDichId || undefined,
      tuNgay: this.tuNgay ? (this.tuNgay as Date).toISOString() : undefined,
      denNgay: this.denNgay ? (this.denNgay as Date).toISOString() : undefined,
      keyword: this.keyword || undefined,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
    };
    this.svc.getList(query).subscribe({
      next: (res: ApiResponse<any>) => {
        this.items = res.result?.items || [];
        this.totalItems = res.result?.pagingInfo?.totalItems || 0;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải danh sách giao dịch');
      }
    });
  }

  onRefresh(): void {
    this.keyword = '';
    this.loaiGiaoDichId = null;
    this.tuNgay = null;
    this.denNgay = null;
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  openTaoPhieuThu(): void {
    const modal = this.modal.create({
      nzTitle: 'Tạo phiếu thu',
      nzContent: PhieuThuFormComponent,
      nzFooter: null,
      nzWidth: 700,
      nzBodyStyle: { 'max-height': '75vh', 'overflow': 'auto' }
    });
    modal.afterClose.subscribe((res: any) => {
      if (res) this.load();
    });
  }

  openTaoPhieuChi(): void {
    const modal = this.modal.create({
      nzTitle: 'Tạo phiếu chi',
      nzContent: PhieuChiFormComponent,
      nzFooter: null,
      nzWidth: 700,
      nzBodyStyle: { 'max-height': '75vh', 'overflow': 'auto' }
    });
    modal.afterClose.subscribe((res: any) => {
      if (res) this.load();
    });
  }

  openDetail(item: GiaoDichItem): void {
    this.selectedItem = item;
    this.detailDrawerVisible = true;
    this.detailLoading = true;
    this.detailItem = null;
    this.svc.getById(item.id).subscribe({
      next: (res: ApiResponse<any>) => {
        this.detailItem = res.result;
        this.detailLoading = false;
      },
      error: () => {
        this.detailLoading = false;
        this.notification.error('Lỗi', 'Không thể tải chi tiết giao dịch');
      }
    });
  }

  closeDetail(): void {
    this.detailDrawerVisible = false;
    this.selectedItem = null;
    this.detailItem = null;
  }

  exportExcel(): void {
    this.exportLoading = true;
    const query: any = {
      loaiGiaoDichId: this.loaiGiaoDichId || undefined,
      tuNgay: this.tuNgay ? (this.tuNgay as Date).toISOString() : undefined,
      denNgay: this.denNgay ? (this.denNgay as Date).toISOString() : undefined,
      keyword: this.keyword || undefined
    };
    this.svc.exportExcel(query).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `nhat-ky-thu-chi_${new Date().toISOString().slice(0, 10)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.exportLoading = false;
        this.notification.success('Thành công', 'Xuất file Excel thành công');
      },
      error: () => {
        this.exportLoading = false;
        this.notification.error('Lỗi', 'Xuất Excel thất bại');
      }
    });
  }

  getLoaiGiaoDichColor(loaiId: number): string {
    return loaiId === 1 ? 'green' : 'red';
  }

  getLoaiGiaoDichLabel(loaiId: number): string {
    return loaiId === 1 ? 'Thu' : 'Chi';
  }
}
