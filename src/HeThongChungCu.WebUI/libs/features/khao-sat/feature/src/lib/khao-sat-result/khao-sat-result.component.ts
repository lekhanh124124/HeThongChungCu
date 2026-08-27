import { Component, OnInit } from '@angular/core';
import { KhaoSatService, KetQuaKhaoSatResponse } from '@features/khao-sat/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-khao-sat-result',
  templateUrl: './khao-sat-result.component.html',
  styleUrls: ['./khao-sat-result.component.scss']
})
export class KhaoSatResultComponent implements OnInit {
  id!: number; // Input from modal/drawer
  loading = false;
  data: KetQuaKhaoSatResponse | null = null;

  constructor(
    private khaoSatService: KhaoSatService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    if (this.id) {
      this.load();
    }
  }

  load(): void {
    this.loading = true;
    this.khaoSatService.getKetQua(this.id).subscribe({
      next: (res: ApiResponse<KetQuaKhaoSatResponse>) => {
        if (res.isOk && res.result) {
          this.data = res.result;
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải kết quả khảo sát');
      }
    });
  }

  getProgressBarStatus(percent: number): 'success' | 'exception' | 'active' | 'normal' {
    if (percent >= 50) return 'success';
    if (percent < 30) return 'exception';
    return 'normal';
  }
}
