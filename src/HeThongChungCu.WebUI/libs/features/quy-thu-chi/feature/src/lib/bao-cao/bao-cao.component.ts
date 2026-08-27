import { Component, OnInit } from '@angular/core';
import { QuyThuChiService } from '@features/quy-thu-chi/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';

@Component({
  selector: 'app-bao-cao',
  templateUrl: './bao-cao.component.html',
  styleUrls: ['./bao-cao.component.scss']
})
export class BaoCaoComponent implements OnInit {
  // Tab state
  selectedTab = 0;

  // --- Tab 1: Báo cáo Thu-Chi ---
  bcTuNgay: Date = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
  bcDenNgay: Date = new Date();
  bcLoading = false;
  bcData: any = null;

  // --- Tab 2: Công nợ căn hộ ---
  cnThang = new Date().getMonth() + 1;
  cnNam = new Date().getFullYear();
  cnToaNhaId: number | null = null;
  cnLoading = false;
  cnItems: any[] = [];
  cnExportLoading = false;

  // --- Tab 3: Công nợ tòa nhà ---
  tnThang = new Date().getMonth() + 1;
  tnNam = new Date().getFullYear();
  tnLoading = false;
  tnItems: any[] = [];
  tnExportLoading = false;

  // Summary stats
  tongThu = 0;
  tongChi = 0;
  tongNo = 0;

  constructor(
    private svc: QuyThuChiService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    this.loadBaoCaoThuChi();
    this.loadCongNoCanHo();
    this.loadCongNoToaNha();
  }

  onTabChange(index: number | undefined): void {
    this.selectedTab = index ?? 0;
  }

  // === Tab 1: Báo cáo Thu-Chi ===
  loadBaoCaoThuChi(): void {
    this.bcLoading = true;
    this.svc.baoCaoThuChi({
      tuNgay: this.bcTuNgay.toISOString(),
      denNgay: this.bcDenNgay.toISOString()
    }).subscribe({
      next: (res: ApiResponse<any>) => {
        this.bcData = res.result;
        this.bcLoading = false;
      },
      error: () => {
        this.bcLoading = false;
        this.notification.error('Lỗi', 'Không thể tải báo cáo thu-chi');
      }
    });
  }

  // === Tab 2: Công nợ căn hộ ===
  loadCongNoCanHo(): void {
    this.cnLoading = true;
    this.svc.baoCaoCongNoCanHo({
      toaNhaId: this.cnToaNhaId || undefined,
      thang: this.cnThang,
      nam: this.cnNam
    }).subscribe({
      next: (res: ApiResponse<any>) => {
        this.cnItems = res.result || [];
        this.cnLoading = false;
      },
      error: () => {
        this.cnLoading = false;
        this.notification.error('Lỗi', 'Không thể tải báo cáo công nợ căn hộ');
      }
    });
  }

  exportCongNoCanHo(): void {
    this.cnExportLoading = true;
    this.svc.exportCongNoCanHo({
      toaNhaId: this.cnToaNhaId || undefined,
      thang: this.cnThang,
      nam: this.cnNam
    }).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `cong-no-can-ho_T${this.cnThang}_${this.cnNam}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.cnExportLoading = false;
        this.notification.success('Thành công', 'Xuất file Excel thành công');
      },
      error: () => {
        this.cnExportLoading = false;
        this.notification.error('Lỗi', 'Xuất Excel thất bại');
      }
    });
  }

  // === Tab 3: Công nợ tòa nhà ===
  loadCongNoToaNha(): void {
    this.tnLoading = true;
    this.svc.baoCaoCongNoToaNha({
      thang: this.tnThang,
      nam: this.tnNam
    }).subscribe({
      next: (res: ApiResponse<any>) => {
        this.tnItems = res.result || [];
        this.tnLoading = false;
      },
      error: () => {
        this.tnLoading = false;
        this.notification.error('Lỗi', 'Không thể tải báo cáo công nợ tòa nhà');
      }
    });
  }

  exportCongNoToaNha(): void {
    this.tnExportLoading = true;
    this.svc.exportCongNoToaNha({
      thang: this.tnThang,
      nam: this.tnNam
    }).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `cong-no-toa-nha_T${this.tnThang}_${this.tnNam}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.tnExportLoading = false;
        this.notification.success('Thành công', 'Xuất file Excel thành công');
      },
      error: () => {
        this.tnExportLoading = false;
        this.notification.error('Lỗi', 'Xuất Excel thất bại');
      }
    });
  }

  formatCurrency(val: number): string {
    if (val == null) return '0';
    return val.toLocaleString('vi-VN');
  }

  getTyLeColor(tyLe: number): string {
    if (tyLe >= 80) return 'green';
    if (tyLe >= 50) return 'orange';
    return 'red';
  }
}
