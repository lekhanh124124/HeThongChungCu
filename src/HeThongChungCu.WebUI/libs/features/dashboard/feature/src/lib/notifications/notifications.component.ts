import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { SignalrService } from '@core/signalr';
import { ThongBaoService } from '@core/notification';
import { Router } from '@angular/router';
import { ChungCuService } from '@features/resident/data-access';

@Component({
  selector: 'app-notifications-page',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit, OnDestroy {
  notifications: any[] = [];
  private sub?: Subscription;
  unreadCount = 0;
  // UI state
  keyword = '';
  filterType: 'all' | 'unread' | 'important' = 'all';
  pageNumber = 1;
  pageSize = 10;

  constructor(
    private signalr: SignalrService,
    private thongBao: ThongBaoService,
    private router: Router,
    private chungCu: ChungCuService
  ) {}

  ngOnInit(): void {
    // accumulate incoming notifications
    try {
      this.loadInitialNotifications();

      this.sub = this.signalr.notifications().subscribe((n: any) => {
        if (!n) return;
        const payload = n;
        if (!payload) return;

        payload.phanBoThongBaoId = payload.id;
        payload._read = !!payload.isRead;
        payload.tieuDe = payload.tieuDe || payload.title || '';
        payload.noiDung = payload.noiDung || payload.content || '';
        payload.createdAt = payload.createdAt || n.createdAt || new Date().toISOString();

        this.notifications = [payload, ...this.notifications];
        this.updateUnreadCount();
      });
    } catch (e) {}
  }

  private loadInitialNotifications(): void {
    try {
      const q = { pageNumber: 1, pageSize: 10 };
      this.chungCu.getThongBaoList(q).subscribe((r: any) => {
        if (r && r.isOk && r.result && Array.isArray(r.result.items)) {
          // normalize items similar to signalr payload
          const items = r.result.items.map((it: any) => ({
            ...it,
            tieuDe: it.tieuDe || it.title || '',
            noiDung: it.noiDung || it.content || '',
            createdAt: it.createdAt || it.createdAt,
            phanBoThongBaoId: it.id,
            _read: !!(it.isRead || it.readAt)
          }));
          this.notifications = items.concat(this.notifications);
          this.updateUnreadCount();
        }
      });
    } catch (e) {}
  }

  // UI helpers: client-side filter + pagination
  get filteredNotifications(): any[] {
    const kw = (this.keyword || '').toLowerCase().trim();
    return (this.notifications || []).filter(n => {
      if (this.filterType === 'unread' && (n._read || n.isRead)) return false;
      if (this.filterType === 'important' && !(n.isImportant || n.priority === 'high' || n.loaiThongBaoId === 99)) return false;
      if (!kw) return true;
      return ((n.tieuDe || n.title || '') + ' ' + (n.noiDung || n.content || '')).toLowerCase().includes(kw);
    });
  }

  get pagedNotifications(): any[] {
    const all = this.filteredNotifications;
    const start = (this.pageNumber - 1) * this.pageSize;
    return all.slice(start, start + this.pageSize);
  }

  onPageChange(p: number): void { this.pageNumber = p; }
  onPageSizeChange(s: number): void { this.pageSize = s; this.pageNumber = 1; }

  reload(): void { this.pageNumber = 1; this.loadInitialNotifications(); }

  onSearch(): void { this.pageNumber = 1; }

  private updateUnreadCount(): void {
    try {
      this.unreadCount = (this.notifications || []).filter(n => !n._read && !n.isRead).length;
    } catch (e) { this.unreadCount = 0; }
  }

  ngOnDestroy(): void {
    try { this.sub?.unsubscribe(); } catch (e) {}
  }

  openDetail(n: any): void {
    if (!n) return;
    const phanBo = (n.phanBoThongBaoId ?? n.id) as number | undefined;
    if (phanBo != null) {
      try { this.thongBao.markRead(phanBo).subscribe({ next: () => { n._read = true; this.updateUnreadCount(); }, error: () => { n._read = true; this.updateUnreadCount(); } }); } catch (e) { n._read = true; this.updateUnreadCount(); }
    } else { n._read = true; this.updateUnreadCount(); }

    let kind = n.loaiThongBaoId ?? null;
    let refId: number | null = null;

    if (n.referenceId != null) {
      const parsed = parseInt(n.referenceId, 10);
      refId = Number.isNaN(parsed) ? null : parsed;
    }

    if (typeof n.metadata === 'string') {
      try {
        const metaObj = JSON.parse(n.metadata);
        const mId = metaObj?.referenceId ?? metaObj?.id ?? metaObj?.Id;
        if (mId != null && refId == null) {
          const p = parseInt(mId, 10);
          refId = Number.isNaN(p) ? null : p;
        }
        // Bổ sung: Lấy kind từ metadata nếu ở ngoài không có
        if (kind == null) {
          kind = metaObj?.loaiThongBaoId ?? metaObj?.Kind ?? metaObj?.kind ?? null;
        }
      } catch {}
    }

    if (refId == null && typeof n.id === 'number') {
      refId = n.id;
    }

    let tabIndex = 0;
    const k = Number(kind);
    const title = (n.tieuDe || n.title || '').toLowerCase();
    const content = (n.noiDung || n.content || '').toLowerCase();

    const isPhanAnh = k === 5
      || title.includes('phản ánh') || title.includes('khiếu nại') || title.includes('góp ý')
      || content.includes('phản ánh') || content.includes('khiếu nại') || content.includes('góp ý');

    if (!isPhanAnh) {
      if (k === 2 || title.includes('phương tiện') || content.includes('phương tiện')) {
        tabIndex = 1;
      } else if (k === 10 || k === 3 || title.includes('sửa chữa') || content.includes('sửa chữa')) {
        tabIndex = 2;
      } else if (k === 11 || k === 4 || title.includes('thi công') || content.includes('thi công')) {
        tabIndex = 3;
      }
    }

    if (isPhanAnh) {
      this.router.navigate(['/dashboard/phan-anh'], { queryParams: { id: refId } });
    } else {
      this.router.navigate(['/dashboard/resident/requests'], { 
        queryParams: { 
          id: refId, 
          tab: tabIndex, 
          loaiThongBaoId: k || (tabIndex === 1 ? 2 : tabIndex === 2 ? 10 : tabIndex === 3 ? 11 : 1) 
        } 
      });
    }
  }
}
