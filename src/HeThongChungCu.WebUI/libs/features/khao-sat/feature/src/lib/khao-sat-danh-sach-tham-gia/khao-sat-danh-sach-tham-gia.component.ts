import { Component, Input, OnInit } from '@angular/core';
import { KhaoSatService } from '@features/khao-sat/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-khao-sat-danh-sach-tham-gia',
  templateUrl: './khao-sat-danh-sach-tham-gia.component.html',
  styleUrls: ['./khao-sat-danh-sach-tham-gia.component.scss']
})
export class KhaoSatDanhSachThamGiaComponent implements OnInit {
  @Input() khaoSatId!: number;
  @Input() tieuDe!: string;

  loading = false;
  items: any[] = [];
  pageSize = 10;
  pageNumber = 1;
  totalItems = 0;

  constructor(
    private svc: KhaoSatService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.svc.getDanhSachThamGia({
      khaoSatId: this.khaoSatId,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    }).subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.isOk) {
          this.items = res.result.items || [];
          this.totalItems = res.result.pagingInfo?.totalItems || 0;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải danh sách tham gia');
      }
    });
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }
}
