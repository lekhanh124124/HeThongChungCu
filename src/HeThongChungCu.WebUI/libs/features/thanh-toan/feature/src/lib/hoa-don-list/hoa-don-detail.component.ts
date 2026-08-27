import { Component, Input, OnInit } from '@angular/core';
import { ThanhToanService } from '@features/thanh-toan/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-hoa-don-detail',
  templateUrl: './hoa-don-detail.component.html',
  styleUrls: ['./hoa-don-detail.component.scss']
})
export class HoaDonDetailComponent implements OnInit {
  @Input() hoaDonId!: number;
  loading = false;
  item: any;

  chiTietVisible = false;
  chiTietLoading = false;
  selectedChiTiet: any;
  chiTietData: any[] = [];

  constructor(
    private thanhToanService: ThanhToanService,
    private notification: NzNotificationService,
    private modal: NzModalService,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.thanhToanService.getHoaDonById(this.hoaDonId).subscribe({
      next: (res: ApiResponse<any>) => {
        this.item = res.result;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải chi tiết hóa đơn');
      }
    });
  }

  getLoaiDinhGiaLabel(id: number): string {
    const maps: any = { 1: 'Giá cố định', 2: 'Giá lũy tiến', 3: 'Theo diện tích', 4: 'Theo khung giờ' };
    return maps[id] || 'Khác';
  }

  openChiTiet(chiTiet: any): void {
    this.selectedChiTiet = chiTiet;
    this.chiTietData = [];
    this.chiTietVisible = true;
    this.chiTietLoading = true;

    const id = chiTiet.id;
    let obs;

    switch (chiTiet.loaiDinhGiaId) {
      case 1:
        obs = this.thanhToanService.getChiTietCoDinh(id);
        break;
      case 2:
        obs = this.thanhToanService.getChiTietLuyTien(id);
        break;
      case 3:
        obs = this.thanhToanService.getChiTietDienTich(id);
        break;
      case 4:
        obs = this.thanhToanService.getChiTietKhungGio(id);
        break;
      default:
        this.chiTietLoading = false;
        this.notification.warning('Thông báo', 'Loại định giá chưa được hỗ trợ');
        return;
    }

    obs.subscribe({
      next: (res: ApiResponse<any>) => {
        this.chiTietData = res.result?.items || res.result ? [res.result] : [];
        this.chiTietLoading = false;
      },
      error: () => {
        this.chiTietLoading = false;
        this.notification.error('Lỗi', 'Không thể tải chi tiết mục phí');
      }
    });
  }
}
