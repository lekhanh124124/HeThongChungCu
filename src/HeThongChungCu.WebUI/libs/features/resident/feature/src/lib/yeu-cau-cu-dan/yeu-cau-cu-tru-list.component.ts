import { Component, OnInit, ViewChild, TemplateRef, OnDestroy } from '@angular/core';
import { SignalrService } from '@core/signalr';
import { ThongBaoService } from '@core/notification';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ChungCuService } from '@features/resident/data-access';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';

@Component({
  selector: 'app-yeu-cau-cu-tru-list',
  templateUrl: './yeu-cau-cu-tru-list.component.html',
  styleUrls: ['./yeu-cau-cu-tru-list.component.scss']
})
export class YeuCauCuTruListComponent implements OnInit, OnDestroy {
  items: any[] = [];
  loading = false;
  pageSize = 10;
  pageNumber = 1;
  total = 0;

  keyword = '';
  sortCol: string | null = null;
  isAsc = true;
  // filters from tree/query params
  toaNhaId?: number;
  tangId?: number;
  canHoId?: number;
  // highlight an item when navigated from a notification
  highlightId?: number | undefined;
  // additional filters
  loaiYeuCauId?: number;
  trangThaiId?: number;

  loaiYeuCauOptions: any[] = [];
  trangThaiOptions: any[] = [];
  advancedVisible = false;

  openAdvanced(): void {
    this.advancedVisible = true;
  }

  closeAdvanced(): void {
    this.advancedVisible = false;
  }

  applyAdvanced(): void {
    this.pageNumber = 1;
    this.advancedVisible = false;
    this.load();
  }

  // selection
  listOfCurrentPageData: readonly any[] = [];
  setOfCheckedId = new Set<number>();
  checked = false;
  indeterminate = false;

  // chi tiết yêu cầu
  detailVisible = false;
  detailLoading = false;
  selectedRequest: any | null = null;
  // reject modal
  @ViewChild('rejectTpl') rejectTpl?: TemplateRef<any>;
  rejectModalRef: any = null;
  rejectReason = '';
  rejectingId?: number | null = null;

  constructor(
    private chungCu: ChungCuService,
    private notification: NzNotificationService,
    private modal: NzModalService,
    private route: ActivatedRoute,
    private signalr: SignalrService,
    private thongBao: ThongBaoService,
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const tabRaw = params['tab'] ?? params['loaiThongBaoId'] ?? null;
      const tab = tabRaw != null ? Number(tabRaw) : null;
      const handleHighlight = tab == null || isNaN(tab) || tab === 0;

      this.highlightId = handleHighlight && params.id != null
        ? (params.id === '' ? undefined : +params.id)
        : undefined;
      this.toaNhaId = params.toaNhaId != null ? (params.toaNhaId === '' ? undefined : +params.toaNhaId) : undefined;
      this.tangId = params.tangId != null ? (params.tangId === '' ? undefined : +params.tangId) : undefined;
      this.canHoId = params.canHoId != null ? (params.canHoId === '' ? undefined : +params.canHoId) : undefined;
      if (params.filterScope === 'root') {
        this.loaiYeuCauId = undefined;
        this.trangThaiId = undefined;
      }
      this.pageNumber = 1;
      this.load();
    });

    this.loadSelectors();

    try {
      this._notifSub = this.signalr.notifications().subscribe((n: any) => {
        if (!n) return;
        try {
          const kind = n.loaiThongBaoId ?? (n.metadata && n.metadata.loaiThongBaoId) ?? null;
          if (kind === 1) {
            const payload = n.metadat7a || n.result || n;
            if (payload) {
              const phanBoId = (n.thongBaoId ?? n.id ?? n.thongBao?.id ?? n.notificationId) as number | undefined;
              if (phanBoId != null) payload.phanBoThongBaoId = phanBoId;
              this.items = [payload, ...this.items];
              this.total = (this.total || 0) + 1;
            }
          }
        } catch (e) { }
      });
    } catch (e) { }
  }

  private _notifSub?: Subscription;

  ngOnDestroy(): void {
    try { this._notifSub?.unsubscribe(); } catch (e) {}
  }

  private loadSelectors(): void {
    // Loại yêu cầu
    this.chungCu.getLoaiYeuCauForSelector().subscribe({
      next: res => { this.loaiYeuCauOptions = (res && res.isOk && Array.isArray(res.result)) ? res.result : []; },
      error: () => { this.loaiYeuCauOptions = []; }
    });

    // Trạng thái yêu cầu cư trú
    this.chungCu.getTrangThaiYeuCauForSelector().subscribe({
      next: res => { this.trangThaiOptions = (res && res.isOk && Array.isArray(res.result)) ? res.result : []; },
      error: () => { this.trangThaiOptions = []; }
    });
  }

  load(): void {
    const query: any = {
      toaNhaId: this.toaNhaId != null ? this.toaNhaId : undefined,
      tangId: this.tangId != null ? this.tangId : undefined,
      canHoId: this.canHoId != null ? this.canHoId : undefined,
      loaiYeuCauId: this.loaiYeuCauId != null ? this.loaiYeuCauId : undefined,
      trangThaiId: this.trangThaiId != null ? this.trangThaiId : undefined,
      keyword: this.keyword || undefined,
      sortCol: this.sortCol || undefined,
      isAsc: this.sortCol ? this.isAsc : undefined,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };

    this.loading = true;
    this.chungCu.getYeuCauCuTruList(query).subscribe({
      next: res => {
        this.loading = false;
        if (res && res.isOk && res.result) {
          this.items = res.result.items || [];
          const pi = res.result.pagingInfo;
          if (pi) {
            this.pageSize = pi.pageSize || this.pageSize;
            this.pageNumber = pi.pageNumber || this.pageNumber;
            this.total = pi.totalItems || 0;
          } else {
            this.total = this.items.length;
          }
          // if highlightId specified, move it to top and mark
          try { this.applyHighlight(); } catch (e) {}
        } else {
          this.items = [];
          this.total = 0;
        }
      },
      error: () => {
        this.loading = false;
        this.items = [];
        this.total = 0;
        this.notification.error('Lỗi', 'Không lấy được danh sách yêu cầu cư trú');
      }
    });
  }

  private applyHighlight(): void {
    const id = this.highlightId;
    if (!id) return;
    const idx = this.items.findIndex(x => x.id === id || x.quanHeCuTruId === id || x.userId === id);
    if (idx !== -1) {
      const item = this.items[idx];
      // move to top
      this.items = [item, ...this.items.filter((_, i) => i !== idx)];
      // mark for highlight
      try { this.items[0]._highlight = true; } catch (e) {}
      // remove highlight after 6s
      setTimeout(() => { try { delete this.items[0]._highlight; this.items = [...this.items]; } catch (e) {} }, 6000);
      return;
    }

    try {
      this.chungCu.getYeuCauCuTruById(id).subscribe({
        next: res => {
          if (res && res.isOk && res.result) {
            const item = res.result;
            item._highlight = true;
            this.items = [item, ...this.items];
            this.total = (this.total || 0) + 1;
            setTimeout(() => { try { delete this.items[0]._highlight; this.items = [...this.items]; } catch (e) {} }, 6000);
          }
        },
        error: () => {}
      });
    } catch (e) {}
  }


  onSearch(): void {
    this.pageNumber = 1;
    this.load();
  }

  onRefresh(): void {
    this.keyword = '';
    this.loaiYeuCauId = undefined;
    this.trangThaiId = undefined;
    this.sortCol = '';
    this.isAsc = true;
    this.pageNumber = 1;
    this.setOfCheckedId.clear();
    this.load();
  }

  onSortSelect(col: string | null): void {
    if (!col) {
      this.sortCol = null;
      this.isAsc = true;
      this.pageNumber = 1;
      this.load();
      return;
    }
    if (this.sortCol === col) this.isAsc = !this.isAsc;
    else {
      this.sortCol = col;
      this.isAsc = true;
    }
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNumber = 1;
    this.load();
  }

  // selection helpers
  updateCheckedSet(id: number | undefined, checked: boolean): void {
    if (id == null) return;
    if (checked) this.setOfCheckedId.add(id);
    else this.setOfCheckedId.delete(id);
  }

  onItemChecked(id: number | undefined, checked: boolean): void {
    if (id == null) return;
    this.updateCheckedSet(id, checked);
    this.refreshCheckedStatus();
  }

  onAllChecked(value: boolean): void {
    this.listOfCurrentPageData.forEach(item => this.updateCheckedSet((item as any).id, value));
    this.refreshCheckedStatus();
  }

  onCurrentPageDataChange($event: readonly any[]): void {
    this.listOfCurrentPageData = $event;
    this.refreshCheckedStatus();
  }

  refreshCheckedStatus(): void {
    this.checked = this.listOfCurrentPageData.length > 0 && this.listOfCurrentPageData.every(item => this.setOfCheckedId.has((item as any).id));
    this.indeterminate = this.listOfCurrentPageData.some(item => this.setOfCheckedId.has((item as any).id)) && !this.checked;
  }

  // chi tiết yêu cầu
  openDetail(it: any): void {
    if (!it || it.id == null) return;
    this.detailVisible = true;
    this.detailLoading = true;
    this.selectedRequest = null;
    this.chungCu.getYeuCauCuTruById(it.id).subscribe({
      next: res => {
        this.detailLoading = false;
        if (res && res.isOk && res.result) {
          this.selectedRequest = res.result;
        } else {
          this.notification.error('Lỗi', 'Không lấy được chi tiết yêu cầu');
        }
      },
      error: () => {
        this.detailLoading = false;
        this.notification.error('Lỗi', 'Không lấy được chi tiết yêu cầu');
      }
    });
  }

  closeDetail(): void {
    this.detailVisible = false;
    this.selectedRequest = null;
  }

  // phê duyệt / từ chối
  approve(it: any): void {
    if (!it || it.id == null) return;
    this.modal.confirm({
      nzTitle: 'Phê duyệt yêu cầu cư trú',
      nzContent: 'Bạn có chắc chắn muốn phê duyệt yêu cầu này?',
      nzOkText: 'Phê duyệt',
      nzOkType: 'primary',
      nzCancelText: 'Hủy',
      nzOnOk: () => {
        this.chungCu.pheDuyetYeuCauCuTru(it.id).subscribe({
          next: res => {
            if (res && res.isOk) {
              this.notification.success('Thành công', 'Đã phê duyệt yêu cầu cư trú');
              this.applyStatusUpdate(it.id, 2, res.result);
            } else {
              this.notification.error('Lỗi', 'Phê duyệt yêu cầu thất bại');
            }
          },
          error: () => this.notification.error('Lỗi', 'Phê duyệt yêu cầu thất bại')
        });
      }
    });
  }

  reject(it: any): void {
    if (!it || it.id == null) return;
    this.rejectingId = it.id;
    this.rejectReason = '';
    try {
      this.rejectModalRef = this.modal.create({
        nzTitle: 'Từ chối yêu cầu cư trú',
        nzContent: this.rejectTpl,
        nzFooter: null,
        nzWidth: 520
      });
    } catch (e) {
      // fallback to prompt if modal creation fails
      const reason = window.prompt('Vui lòng nhập lý do từ chối yêu cầu cư trú:');
      if (!reason || !reason.trim()) {
        this.notification.warning('Thông báo', 'Bạn cần nhập lý do từ chối.');
        return;
      }
      this.submitReject(reason.trim());
    }
  }

  cancelReject(): void {
    try { this.rejectModalRef?.destroy(); } catch (e) { }
    this.rejectingId = undefined;
    this.rejectReason = '';
  }

  submitReject(reason?: string): void {
    const payloadReason = (reason !== undefined) ? reason : (this.rejectReason || '').trim();
    if (!payloadReason) { this.notification.warning('Thông báo', 'Bạn cần nhập lý do từ chối.'); return; }
    const id = this.rejectingId;
    if (!id) { this.notification.error('Lỗi', 'ID yêu cầu không hợp lệ'); return; }
    this.chungCu.tuChoiYeuCauCuTru(id, payloadReason).subscribe({
      next: res => {
        if (res && res.isOk) {
          this.notification.success('Thành công', 'Đã từ chối yêu cầu cư trú');
          try { this.rejectModalRef?.destroy(); } catch (e) { }
          this.rejectingId = undefined;
          this.rejectReason = '';
          this.applyStatusUpdate(id, 3, { lyDo: payloadReason, result: res.result });
        } else {
          this.notification.error('Lỗi', 'Từ chối yêu cầu thất bại');
        }
      },
      error: () => this.notification.error('Lỗi', 'Từ chối yêu cầu thất bại')
    });
  }

  private applyStatusUpdate(id: number, trangThaiId: number, extra?: any): void {
    try {
      if (this.selectedRequest && (this.selectedRequest.id === id || this.selectedRequest.quanHeCuTruId === id)) {
        this.selectedRequest.trangThaiId = trangThaiId;
        if (extra && extra.lyDo) this.selectedRequest.lyDo = extra.lyDo;
        if (extra && extra.result) {
          const r = extra.result;
          if (r.trangThaiId != null) this.selectedRequest.trangThaiId = r.trangThaiId;
          if (r.tenTrangThai) this.selectedRequest.tenTrangThai = r.tenTrangThai;
           if (r.tenNguoiXuLy) this.selectedRequest.tenNguoiXuLy = r.tenNguoiXuLy;
           if (r.nguoiXuLyId != null) this.selectedRequest.nguoiXuLyId = r.nguoiXuLyId;
        } else {
          this.selectedRequest.tenTrangThai = this.trangThaiLabelById(trangThaiId);
        }
        this.selectedRequest.ngayXuLy = new Date().toISOString();
      }
    } catch (e) { }

    try {
      const idx = this.items.findIndex(x => x.id === id || x.quanHeCuTruId === id || x.userId === id);
      if (idx !== -1) {
        this.items[idx].trangThaiId = trangThaiId;
        this.items[idx].tenTrangThai = (extra && extra.result && extra.result.tenTrangThai) ? extra.result.tenTrangThai : this.trangThaiLabelById(trangThaiId);
        if (extra && extra.lyDo) this.items[idx].lyDo = extra.lyDo;
          if (extra && extra.result) {
            const r = extra.result;
            if (r.tenNguoiXuLy) this.items[idx].tenNguoiXuLy = r.tenNguoiXuLy;
            if (r.nguoiXuLyId != null) this.items[idx].nguoiXuLyId = r.nguoiXuLyId;
          }
        this.items = [...this.items];
      }
    } catch (e) { }
  }

  private trangThaiLabelById(id: number): string {
    switch (id) {
      case 1: return 'Đang chờ duyệt';
      case 2: return 'Đã duyệt';
      case 3: return 'Từ chối';
      case 4: return 'Đã lưu';
      case 5: return 'Đã thu hồi';
      case 6: return 'Hết hiệu lực';
      default: return String(id);
    }
  }

  // Xóa yêu cầu
  deleteSelected(): void {
    const ids = Array.from(this.setOfCheckedId);
    if (!ids.length) {
      this.notification.warning('Thông báo', 'Chưa chọn yêu cầu nào');
      return;
    }

    this.modal.confirm({
      nzTitle: 'Xóa yêu cầu đã chọn',
      nzContent: `Bạn có chắc chắn muốn xóa ${ids.length} yêu cầu?`,
      nzOkText: 'Xóa',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => {
        this.chungCu.deleteYeuCau(ids as number[]).subscribe({
          next: res => {
            if (res && res.isOk) this.notification.success('Thành công', 'Xóa yêu cầu thành công');
            else this.notification.warning('Cảnh báo', 'Một số yêu cầu có thể chưa được xóa');
            this.setOfCheckedId.clear();
            this.load();
          },
          error: () => this.notification.error('Lỗi', 'Xóa yêu cầu thất bại')
        });
      }
    });
  }

  deleteOne(id?: number): void {
    if (id == null) return;
    this.modal.confirm({
      nzTitle: 'Xóa yêu cầu',
      nzContent: 'Bạn có chắc chắn muốn xóa yêu cầu này?',
      nzOkText: 'Xóa',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzCancelText: 'Hủy',
      nzOnOk: () => this.chungCu.deleteYeuCau([id!]).subscribe({
        next: res => {
          if (res && res.isOk) this.notification.success('Thành công', 'Xóa yêu cầu thành công');
          else this.notification.error('Lỗi', 'Xóa yêu cầu thất bại');
          this.load();
        },
        error: () => this.notification.error('Lỗi', 'Xóa yêu cầu thất bại')
      })
    });
  }

  getTrangThaiBadgeClass(trangThaiId: number | string | undefined | null): string {
    const id = typeof trangThaiId === 'string' ? +trangThaiId : trangThaiId;
    switch (id) {
      case 1: // Đang chờ duyệt
        return 'bg-amber-500';
      case 2: // Đã duyệt
        return 'bg-green-600';
      case 3: // Từ chối
        return 'bg-red-500';
      case 4: // Đã lưu
        return 'bg-indigo-600';
      case 5: // Đã thu hồi
        return 'bg-gray-500';
      case 6: // Hết hiệu lực
        return 'bg-orange-500';
    }
    // Nếu API trả về giá trị ngoài 1-3 thì coi như "đang chờ duyệt"
    return 'bg-amber-500';
  }

  getTrangThaiIcon(trangThaiId: number | string | undefined | null): string {
    const id = typeof trangThaiId === 'string' ? +trangThaiId : trangThaiId;
    switch (id) {
      case 1: // Đang chờ duyệt
        return 'clock-circle';
      case 2: // Đã duyệt
        return 'check-circle';
      case 3: // Từ chối
        return 'close-circle';
      case 4: // Đã lưu
        return 'save';
      case 5: // Đã thu hồi
        return 'rollback';
      case 6: // Hết hiệu lực
        return 'exclamation-circle';
    }
    // Ngoài 1-3 thì dùng icon "đang chờ duyệt"
    return 'clock-circle';
  }

  getTrangThaiBgColor(trangThaiId: number | string | undefined | null): string {
    const id = typeof trangThaiId === 'string' ? +trangThaiId : trangThaiId;
    switch (id) {
      case 1: // Đang chờ duyệt
        return '#f59e0b';
      case 2: // Đã duyệt
        return '#16a34a';
      case 3: // Từ chối
        return '#ef4444';
      case 4: // Đã lưu
        return '#2563eb';
      case 5: // Đã thu hồi
        return '#6b7280';
      case 6: // Hết hiệu lực
        return '#f97316';
    }
    return '#f59e0b';
  }

  getLoaiYeuCauClass(loaiId: number | string | undefined | null): string {
    const id = typeof loaiId === 'string' ? +loaiId : loaiId;
    switch (id) {
      case 1: // Thêm
        return 'type-add-text';
      case 2: // Sửa
        return 'type-edit-text';
      case 3: // Xóa
        return 'type-delete-text';
    }
    return 'type-add-text';
  }
}
