import { Component, OnInit } from '@angular/core';
import { ChiSoTieuThuService } from '@features/dich-vu/data-access';
import { ChungCuService } from '@features/resident/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { Router } from '@angular/router';

@Component({
  selector: 'app-chi-so-tieu-thu-manual',
  templateUrl: './chi-so-tieu-thu-manual.component.html',
  styleUrls: ['./chi-so-tieu-thu-manual.component.scss']
})
export class ChiSoTieuThuManualComponent implements OnInit {
  loading = false;
  saving = false;

  // Filter for generating list
  thang: number | null = null;
  nam: number | null = null;
  ngayGhiNhan = new Date().toISOString();
  toaNhaId: number | null = null;
  tangId: number | null = null;
  dichVuId: number | null = null;

  toaNhaOptions: any[] = [];
  tangOptions: any[] = [];
  dichVuOptions: any[] = [];

  // Data to enter
  items: any[] = [];

  constructor(
    private chiSoService: ChiSoTieuThuService,
    private chungCuService: ChungCuService,
    private notification: NzNotificationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSelectors();
  }

  loadSelectors(): void {
    this.chungCuService.getToaNhaList({ pageSize: 1000 }).subscribe(res => {
      this.toaNhaOptions = res.result?.items || [];
    });

    this.chiSoService.getListDichVuTieuThu().subscribe(res => {
      this.dichVuOptions = res.result || [];
    });
  }

  onToaNhaChange(val: number): void {
    this.tangId = null;
    this.tangOptions = [];
    if (val) {
      this.chungCuService.getTangList({ toaNhaId: val, pageSize: 1000 }).subscribe(res => {
        this.tangOptions = res.result?.items || [];
      });
    }
  }

  generateList(): void {
    if (!this.dichVuId || !this.tangId) {
      this.notification.warning('Cảnh báo', 'Vui lòng chọn Tòa nhà, Tầng và Dịch vụ');
      return;
    }
    if (!this.thang || !this.nam) {
      this.notification.warning('Cảnh báo', 'Vui lòng chọn tháng và năm');
      return;
    }

    this.loading = true;
    // Get list of apartments in this floor and existing indices if any
    this.chiSoService.getList({
      thang: this.thang,
      nam: this.nam,
      tangId: this.tangId,
      dichVuId: this.dichVuId,
      pageSize: 1000
    }).subscribe(res => {
      const existingItems = res.result?.items || [];

      this.chungCuService.getCanHoList({ tangId: this.tangId, pageSize: 1000 }).subscribe(resCanHo => {
        const canHos = resCanHo.result?.items || [];

        this.items = canHos.map(ch => {
          const existing = existingItems.find((e: any) => e.canHoId === ch.id);
          return {
            canHoId: ch.id,
            maCanHo: ch.maCanHo,
            dichVuId: this.dichVuId,
            tenDichVu: this.dichVuOptions.find((d: any) => d.dichVuId === this.dichVuId)?.tenDichVu,
            chiSoCu: existing ? existing.chiSoCu : ((ch as any).chiSoCu || 0),
            chiSoMoi: existing ? existing.chiSoMoi : null,
            ghiChu: existing ? existing.ghiChu : '',
            anhDongHoId: existing ? existing.anhDongHoId : 0,
            anhDongHoUrl: existing ? existing.anhDongHoUrl : null
          };
        });
        this.loading = false;
      });
    });
  }

  save(): void {
    const dataToSave = this.items.filter(it => it.chiSoMoi !== null && it.chiSoMoi !== undefined);
    if (dataToSave.length === 0) {
      this.notification.warning('Cảnh báo', 'Chưa có chỉ số mới nào được nhập');
      return;
    }
    if (!this.thang || !this.nam) {
      this.notification.warning('Cảnh báo', 'Vui lòng chọn tháng và năm');
      return;
    }

    this.saving = true;
    const payload = {
      items: dataToSave,
      thang: this.thang,
      nam: this.nam,
      ngayGhiNhan: this.ngayGhiNhan
    };

    this.chiSoService.bulkCreate(payload).subscribe({
      next: (res) => {
        if (res.isOk) {
          this.notification.success('Thành công', 'Đã lưu danh sách chỉ số');
          this.router.navigate(['/dashboard/dich-vu/chi-so-tieu-thu']);
        }
        this.saving = false;
      },
      error: () => {
        this.saving = false;
        this.notification.error('Lỗi', 'Không thể lưu dữ liệu');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/dashboard/dich-vu/chi-so-tieu-thu']);
  }

  onChiSoMoiChange(item: any): void {
    if (item.chiSoMoi !== null && item.chiSoMoi < item.chiSoCu) {
      // Maybe show a warning but let them enter?
    }
  }
}
