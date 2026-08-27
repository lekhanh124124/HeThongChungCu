import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { SignalrService } from '@core/signalr';
import { ThongBaoService } from '@core/notification';
import { ChungCuService } from '@features/resident/data-access';

@Component({
  selector: 'app-notification-bell',
  templateUrl: './notification-bell.component.html',
  styleUrls: ['./notification-bell.component.scss']
})
export class NotificationBellComponent implements OnInit, OnDestroy {
  notifications: any[] = [];
  dropdownVisible = false;
  private sub?: Subscription;

  constructor(
    private signalr: SignalrService,
    private thongBao: ThongBaoService,
    private chungCu: ChungCuService,
    private router: Router
  ) {}

  ngOnInit(): void {
    try {
      this.loadInitialNotifications();

      this.sub = this.signalr.notifications().subscribe((n: any) => {
        if (!n) return;
        const payload = n;
        if (!payload) return;

        payload.phanBoThongBaoId = payload.id;
        payload._read = !!payload.isRead;
        payload.tieuDe = payload.tieuDe || payload.title || payload.ten || '';
        payload.noiDung = payload.noiDung || payload.content || '';
        payload.createdAt = payload.createdAt || n.createdAt || new Date().toISOString();

        const key = this.getNotifKey(payload);
        if (key != null && this.notifications.some(x => this.getNotifKey(x) === key)) return;
        this.notifications = [payload, ...this.notifications];
      });
    } catch (e) {}
  }

  private loadInitialNotifications(): void {
    try {
      const q = { pageNumber: 1, pageSize: 6 };
      this.chungCu.getThongBaoList(q).subscribe((r: any) => {
        if (r && r.isOk && r.result && Array.isArray(r.result.items)) {
          const items = r.result.items.map((it: any) => ({
            ...it,
            tieuDe: it.tieuDe || it.title || '',
            noiDung: it.noiDung || it.content || '',
            createdAt: it.createdAt || it.createdAt,
            phanBoThongBaoId: it.id,
            _read: !!(it.isRead || it.readAt)
          }));
          const existingKeys = new Set(this.notifications.map(x => this.getNotifKey(x)).filter(k => k != null));
          const filtered = items.filter((it: any) => {
            const k = this.getNotifKey(it);
            return k == null || !existingKeys.has(k);
          });
          this.notifications = filtered.concat(this.notifications);
        }
      });
    } catch (e) {}
  }

  private getNotifKey(n: any): any {
    if (!n) return null;
    return n.phanBoThongBaoId ?? n.id ?? null;
  }

  ngOnDestroy(): void {
    try { this.sub?.unsubscribe(); } catch (e) {}
  }

  get unreadCount(): number {
    return this.notifications.filter(x => !x._read && !x.isRead).length;
  }

  onDropdownVisibleChange(open: boolean): void {
    this.dropdownVisible = open;
    if (open) {
      try { this.loadInitialNotifications(); } catch (e) {}
    }
  }

  openDetail(n: any): void {
    if (!n) return;
    const phanBo = (n.phanBoThongBaoId ?? n.id) as number | undefined;
    if (phanBo != null) {
      try {
        this.thongBao.markRead(phanBo).subscribe({ next: () => { n._read = true; }, error: () => { n._read = true; } });
      } catch (e) { n._read = true; }
    } else { n._read = true; }

    let kind = n.loaiThongBaoId ?? null;

    let refId: number | null = null;
    if (n.referenceId != null) {
      const parsed = parseInt(n.referenceId, 10);
      refId = Number.isNaN(parsed) ? null : parsed;
    }

    if (typeof n.metadata === 'string') {
      try {
        const metaObj = JSON.parse(n.metadata);
        if (refId == null) {
          const mId = metaObj?.referenceId ?? metaObj?.id ?? metaObj?.Id;
          if (mId != null) {
            const p = parseInt(mId, 10);
            refId = Number.isNaN(p) ? null : p;
          }
        }
        if (kind == null) {
          kind = metaObj?.loaiThongBaoId ?? metaObj?.Kind ?? metaObj?.kind ?? null;
        }
      } catch {}
    }

    if (refId == null && typeof n.id === 'number') {
      refId = n.id;
    }

    const title = (n.tieuDe || n.title || '').toLowerCase();
    const content = (n.noiDung || n.content || '').toLowerCase();
    const k = Number(kind);

    let tabIndex = 0;
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

    try { this.dropdownVisible = false; } catch (e) {}

    if (isPhanAnh) {
      this.router.navigate(['/dashboard/phan-anh'], { queryParams: { id: refId } });
    } else {
      this.router.navigate(['/dashboard/resident/requests'], { queryParams: { id: refId, tab: tabIndex } });
    }
  }
}
