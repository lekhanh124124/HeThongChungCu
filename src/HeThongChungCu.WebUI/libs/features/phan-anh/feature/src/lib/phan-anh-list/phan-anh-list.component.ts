import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { PhanAnhService, PhanAnhResponse } from '@features/phan-anh/data-access';
import { ApiResponse } from '@features/dich-vu/data-access';
import { SignalrService } from '@core/signalr';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzDrawerService } from 'ng-zorro-antd/drawer';
import { PhanAnhDetailComponent } from '../phan-anh-detail/phan-anh-detail.component';

@Component({
  selector: 'app-phan-anh-list',
  templateUrl: './phan-anh-list.component.html',
  styleUrls: ['./phan-anh-list.component.scss']
})
export class PhanAnhListComponent implements OnInit, OnDestroy {
  loading = false;
  items: PhanAnhResponse[] = [];
  
  // Filters
  trangThaiId: number | null = null;
  loaiPhanAnhId: number | null = null;
  keyword = '';
  
  pageSize = 10;
  pageNumber = 1;
  totalItems = 0;

  highlightId?: number;
  private destroy$ = new Subject<void>();

  trangThaiPhanAnhOptions: { id: number; name: string }[] = [];
  loaiPhanAnhOptions: { id: number; name: string }[] = [];
  advancedVisible = false;

  constructor(
    private phanAnhService: PhanAnhService,
    private notification: NzNotificationService,
    private modal: NzModalService,
    private drawerService: NzDrawerService,
    private route: ActivatedRoute,
    private router: Router,
    private signalr: SignalrService,
  ) {}

  ngOnInit(): void {
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe((params: any) => {
      this.highlightId = params.id != null
        ? (params.id === '' ? undefined : +params.id)
        : undefined;
      this.load();
    });

    this.setupSignalR();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSignalR(): void {
    try {
      this.signalr.notifications()
        .pipe(takeUntil(this.destroy$))
        .subscribe((n: any) => {
          if (!n) return;
          this.load();
        });
    } catch (e) {}
  }

  load(): void {
    this.loading = true;
    const query = {
      trangThaiPhanAnhId: this.trangThaiId,
      loaiPhanAnhId: this.loaiPhanAnhId,
      keyword: this.keyword,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      sortCol: 'CreatedAt',
      isAsc: false
    };

    this.phanAnhService.getList(query).subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.isOk && res.result) {
          this.items = res.result.items;
          this.totalItems = res.result.pagingInfo.totalItems;
          this.trangThaiPhanAnhOptions = [
            ...new Map((res.result.items as any[]).map((i: any) => [i.trangThaiPhanAnhId, { id: i.trangThaiPhanAnhId, name: i.trangThaiPhanAnhTen }])).values()
          ];
          this.loaiPhanAnhOptions = [
            ...new Map((res.result.items as any[]).map((i: any) => [i.loaiPhanAnhId, { id: i.loaiPhanAnhId, name: i.loaiPhanAnhTen }])).values()
          ];
          this.applyHighlight();
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notification.error('Lỗi', 'Không thể tải danh sách phản ánh');
      }
    });
  }

  private applyHighlight(): void {
    if (!this.highlightId) return;
    const it = this.items.find(x => x.id === this.highlightId);
    if (it) {
      (it as any)._highlight = true;
      setTimeout(() => { (it as any)._highlight = false; }, 6000);
      this.openDetail(it);
    }
    this.highlightId = undefined;
  }

  onRefresh(): void {
    this.keyword = '';
    this.trangThaiId = null;
    this.loaiPhanAnhId = null;
    this.pageNumber = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.load();
  }

  getStatusColor(id: number): string {
    switch (id) {
      case 1: return 'default'; // Wait
      case 2: return 'blue';    // Processing
      case 3: return 'orange';  // BQL Replied
      case 4: return 'gold';    // Res Replied
      case 5: return 'purple';  // Wait Evaluation
      case 6: return 'green';   // Completed
      case 7: return 'red';     // Cancelled
      default: return 'gray';
    }
  }

  openDetail(item: PhanAnhResponse): void {
    const drawerRef = this.drawerService.create({
      nzTitle: 'Chi tiết phản ánh & Trao đổi',
      nzContent: PhanAnhDetailComponent,
      nzContentParams: { id: item.id },
      nzWidth: 900
    });

    drawerRef.afterClose.subscribe(() => {
      this.load();
    });
  }
}
