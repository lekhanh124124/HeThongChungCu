import { Component, Input, OnInit } from '@angular/core';
import { KhaoSatService } from '@features/khao-sat/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-khao-sat-lich-su',
  templateUrl: './khao-sat-lich-su.component.html',
  styleUrls: ['./khao-sat-lich-su.component.scss']
})
export class KhaoSatLichSuComponent implements OnInit {
  @Input() canHoId!: number;
  @Input() khaoSatId?: number;

  loading = false;
  items: any[] = [];

  constructor(
    private svc: KhaoSatService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.svc.getLichSuBieuQuyet({
      canHoId: this.canHoId,
      khaoSatId: this.khaoSatId || undefined
    }).subscribe({
      next: (res: ApiResponse<any>) => {
        this.items = res.result || [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải lịch sử biểu quyết');
      }
    });
  }

  isExpanded(row: any, index: number): boolean {
    return row._expanded === true;
  }

  toggleExpand(row: any): void {
    row._expanded = !row._expanded;
  }
}
