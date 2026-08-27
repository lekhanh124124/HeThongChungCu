import { Component, Input, OnInit, Optional, ViewChild, TemplateRef } from '@angular/core';
import { BaoTriService } from '@features/bao-tri/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { DoiTacApiService } from '@features/doi-tac/data-access';
import { ChungCuService } from '@features/resident/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { NzDrawerRef } from 'ng-zorro-antd/drawer';

@Component({
  selector: 'app-phieu-bao-tri-detail',
  templateUrl: './phieu-bao-tri-detail.component.html',
})
export class PhieuBaoTriDetailComponent implements OnInit {
  @Input() phieuId!: number;

  loading = false;
  data: any = null;

  activeTab = 0;

  @ViewChild('completeModal', { static: true }) completeModal!: TemplateRef<any>;
  @ViewChild('cancelModal', { static: true }) cancelModal!: TemplateRef<any>;

  // For Assignment
  nhanVienOptions: any[] = [];
  selectedNhanVienId: number | null = null;

  // For Progress Update
  submittingProgress = false;

  constructor(
    private baoTriService: BaoTriService,
    private doiTacService: DoiTacApiService,
    private chungCuService: ChungCuService,
    private notification: NzNotificationService,
    private modal: NzModalService,
    @Optional() public modalRef?: NzModalRef,
    @Optional() public drawerRef?: NzDrawerRef,
  ) {}

  ngOnInit(): void {
    this.load();
    this.loadEmployees();
  }

  close(changed = false): void {
    if (this.modalRef) {
      this.modalRef.destroy();
    } else if (this.drawerRef) {
      this.drawerRef.close(changed);
    }
  }

  load(): void {
    this.loading = true;
    this.baoTriService.getPhieuBaoTriById(this.phieuId).subscribe({
      next: (res: ApiResponse<any>) => {
        this.data = res.result;
        if (!this.data.vatTus) this.data.vatTus = [];
        this.selectedNhanVienId = this.data.nhanVienId || null;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  loadEmployees(): void {
    this.chungCuService.getNhanVienList({ loaiNhanVienId: 1, pageSize: 1000, pageNumber: 1 }).subscribe((res: ApiResponse<any>) => {
      this.nhanVienOptions = res.result?.items || [];
    });
  }

  assign(): void {
    if (!this.selectedNhanVienId) {
      this.notification.warning('Cảnh báo', 'Vui lòng chọn nhân viên kỹ thuật');
      return;
    }

    const nv = this.nhanVienOptions.find((x: any) => x.id === this.selectedNhanVienId);
    const payload = {
      id: this.phieuId,
      ngayDuKien: this.data.ngayDuKien,
      nhanSus: nv ? [{
        nhanVienId: nv.id,
        hoTen: nv.hoTen,
        soCCCD: nv.soCCCD || '',
        soDienThoai: nv.soDienThoai || '',
        vaiTro: 'Kỹ thuật viên nội bộ',
      }] : [],
    };

    this.baoTriService.phanCongPhieuBaoTri(payload).subscribe((res) => {
      if (res.isOk) {
        this.notification.success('Thành công', 'Đã phân công công việc');
        this.close(true);
      }
    });
  }

  updateProgress(): void {
    const payload = {
      id: this.phieuId,
      ghiChuXuLy: this.data.ghiChuXuLy,
      checklists: this.data.checklists.map((c: any) => ({
        checklistId: c.id,
        isDatYeuCau: c.datYeuCau,
        ghiChuThucTe: c.ghiChuThucTe,
        anhMinhHoaId: c.anhMinhHoaId,
      })),
      vatTus: this.data.vatTus.map((v: any) => ({
        tenVatTu: v.tenVatTu,
        soLuong: v.soLuong,
        donGia: v.donGia,
      })),
    };

    this.submittingProgress = true;
    this.baoTriService.capNhatTienDoPhieuBaoTri(payload).subscribe({
      next: (res) => {
        if (res.isOk) {
          this.notification.success(
            'Thành công',
            'Đã cập nhật tiến độ thực tế',
          );
          this.load();
        }
        this.submittingProgress = false;
      },
      error: () => (this.submittingProgress = false),
    });
  }

  complete(): void {
    const modalRef: any = this.modal.create({
      nzTitle: 'Xác nhận hoàn thành',
      nzContent: this.completeModal,
      nzFooter: [
        {
          label: 'Hủy',
          type: 'default' as const,
          onClick: () => modalRef.destroy(),
        },
        {
          label: 'Hoàn thành',
          type: 'primary' as const,
          onClick: () => {
            const note = (document.querySelector('#completeNote') as HTMLTextAreaElement)?.value?.trim() || '';
            this.baoTriService
              .kiemDuyetPhieuBaoTri({ id: this.phieuId, isDuyet: true, ghiChuXuLy: note })
              .subscribe((res) => {
                if (res.isOk) {
                  this.notification.success('Thành công', 'Đã hoàn thành phiếu bảo trì');
                  modalRef.destroy();
                  this.close(true);
                }
              });
          },
        },
      ],
    });
  }

  cancelTicket(): void {
    const modalRef: any = this.modal.create({
      nzTitle: 'Xác nhận hủy phiếu',
      nzContent: this.cancelModal,
      nzFooter: [
        {
          label: 'Không hủy',
          type: 'default' as const,
          onClick: () => modalRef.destroy(),
        },
        {
          label: 'Xác nhận hủy',
          type: 'primary' as const,
          danger: true,
          onClick: () => {
            const reason = (document.querySelector('#cancelReason') as HTMLTextAreaElement)?.value?.trim() || '';
            if (!reason) {
              this.notification.warning('Cảnh báo', 'Vui lòng nhập lý do hủy');
              return;
            }
            this.baoTriService
              .huyPhieuBaoTri({ id: this.phieuId, lyDo: reason })
              .subscribe((res) => {
                if (res.isOk) {
                  this.notification.success('Thành công', 'Đã hủy phiếu bảo trì');
                  modalRef.destroy();
                  this.close(true);
                }
              });
          },
        },
      ],
    });
  }

  addVatTu(): void {
    this.data.vatTus = [...(this.data.vatTus || []), { tenVatTu: '', soLuong: 1, donGia: 0 }];
  }

  removeVatTu(index: number): void {
    this.data.vatTus = this.data.vatTus.filter((_: any, i: number) => i !== index);
  }

  getTotalChiPhi(): number {
    return (this.data?.vatTus || []).reduce(
      (acc: number, cur: any) => acc + cur.soLuong * cur.donGia,
      0,
    );
  }

  getStatusColor(statusId: number): string {
    switch (statusId) {
      case 1: return 'blue';
      case 2: return 'orange';
      case 3: return 'cyan';
      case 4: return 'green';
      case 5: return 'red';
      default: return 'default';
    }
  }
}
