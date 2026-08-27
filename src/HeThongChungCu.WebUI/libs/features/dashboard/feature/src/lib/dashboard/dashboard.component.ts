import { Component, OnInit, HostListener, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AuthService } from '@core/auth/data-access';
import { ChangePasswordModalService } from '@features/profile/feature';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  isCollapsed = false;
  isMobile = false;
  currentTitle = 'Trang chủ';
  menuItems: any[] = [];
  username = '';
  role = '';
  avatarUrl = '';
  defaultAvatar = 'assets/images/avatar.png';

  private avatarSub?: Subscription;
  private userSub?: Subscription;

  constructor(private router: Router, private auth: AuthService, private changePwd: ChangePasswordModalService) { }

  ngOnInit(): void {
    this.checkMobile();
    this.buildDefaultMenu();
    this.updateTitleFromUrl(this.router.url);
    this.routerSub = this.router.events.pipe(filter(e => e instanceof NavigationEnd)).subscribe((e: any) => {
      const url = e.urlAfterRedirects || e.url;
      this.updateTitleFromUrl(url);
    });

    this.avatarSub = this.auth.avatar$.subscribe(url => {
      if (url) this.avatarUrl = url;
      else this.avatarUrl = this.auth.getAvatar() || '';
    });
    this.avatarUrl = this.auth.getAvatar() || this.avatarUrl;
    const stored = this.auth.getCurrentUser();
    if (stored) {
      this.username = stored.fullName || stored.username || '';
      this.role = stored.role || '';
      if (stored.anhDaiDienUrl) this.avatarUrl = stored.anhDaiDienUrl;
    }
    this.userSub = this.auth.currentUser$.subscribe(u => {
      if (u) {
        this.username = u.fullName || u.username || this.username;
        this.role = u.role || this.role;
        if (u.anhDaiDienUrl) this.avatarUrl = u.anhDaiDienUrl;
      } else {
        this.username = '';
        this.role = '';
      }
    });
  }

  @HostListener('window:resize')
  onResize() {
    this.checkMobile();
  }

  checkMobile() {
    this.isMobile = window.innerWidth <= 768;
    if (this.isMobile) this.isCollapsed = true;
  }

  buildDefaultMenu() {
    this.menuItems = [
      { title: 'Trang chủ', route: '/dashboard/home', icon: 'home' },
      { title: 'Quản lý chung cư', route: '/dashboard/resident', icon: 'bank' },
      { title: 'Quản lý quan hệ cư trú', route: '/dashboard/resident/quan-he-cu-tru', icon: 'team' },
      { title: 'Quản lý phương tiện', route: '/dashboard/resident/phuong-tien', icon: 'car' },
      { title: 'Quản lý nhân viên', route: '/dashboard/resident/nhan-vien', icon: 'user' },
      { title: 'Yêu cầu cư dân', route: '/dashboard/resident/requests', icon: 'form' },
      { title: 'Đối tác', route: '/dashboard/doi-tac', icon: 'appstore' },
      { title: 'Dịch vụ', children: [
        { title: 'Danh mục dịch vụ', route: '/dashboard/dich-vu', icon: 'unordered-list' },
        { title: 'Chỉ số tiêu thụ', route: '/dashboard/dich-vu/chi-so-tieu-thu', icon: 'dashboard' }
      ], icon: 'tool' },
      { title: 'Thanh toán', children: [
        { title: 'Đợt thanh toán', route: '/dashboard/thanh-toan/dot-thanh-toan', icon: 'calendar' },
        { title: 'Hóa đơn cư dân', route: '/dashboard/thanh-toan/hoa-don', icon: 'file-text' },
        { title: 'Hóa đơn đối tác', route: '/dashboard/thanh-toan/hoa-don-doi-tac', icon: 'solution' },
        { title: 'Quỹ Thu/Chi', route: '/dashboard/quy-thu-chi', icon: 'wallet' }
      ], icon: 'pay-circle' },
      { title: 'Bảo trì hạ tầng', children: [
        { title: 'Thiết bị hạ tầng', route: '/dashboard/bao-tri/thiet-bi', icon: 'database' },
        { title: 'Hạng mục bảo trì', route: '/dashboard/bao-tri/hang-muc', icon: 'project' },
        { title: 'Lịch bảo trì', route: '/dashboard/bao-tri/lich-bao-tri', icon: 'history' },
        { title: 'Phiếu bảo trì', route: '/dashboard/bao-tri/phieu-bao-tri', icon: 'file-done' }
      ], icon: 'setting' },
      { title: 'Khảo sát & Bầu cử', route: '/dashboard/khao-sat', icon: 'solution' },
      { title: 'Phản ánh cư dân', route: '/dashboard/phan-anh', icon: 'message' },
      { title: 'Tri thức Chatbot', route: '/dashboard/tri-thuc', icon: 'book' },
      { title: 'Sao lưu & Khôi phục', route: '/dashboard/sao-luu', icon: 'cloud-server' },
      // { title: 'Thông báo', route: '/dashboard/notifications', icon: 'notification' },
      // { title: 'Cài đặt', route: '/dashboard/settings', icon: 'setting' }
    ];
  }

  toggleSidebar() {
    this.isCollapsed = !this.isCollapsed;
  }

  ngOnDestroy(): void {
    this.routerSub?.unsubscribe();
    this.avatarSub?.unsubscribe();
    this.userSub?.unsubscribe();
  }

  private routerSub?: Subscription;

  private updateTitleFromUrl(url: string): void {
    const title = this.findTitleByUrl(url, this.menuItems) || 'Trang chủ';
    this.currentTitle = title;
  }

  private findTitleByUrl(url: string, items: any[]): string | null {
    if (!items) return null;
    let bestTitle: string | null = null;
    let bestLen = -1;

    const walk = (arr: any[]) => {
      if (!arr) return;
      for (const it of arr) {
        if (it.route && typeof it.route === 'string' && url.startsWith(it.route)) {
          const l = it.route.length;
          if (l > bestLen) {
            bestLen = l;
            bestTitle = it.title || null;
          }
        }
        if (it.children) walk(it.children);
      }
    };

    walk(items);
    return bestTitle;
  }

  logout() {
    this.auth?.logout().subscribe({
      next: () => {
        this.auth.clearTokens();
        this.router.navigate(['/login']);
      },
      error: () => {
        this.auth.clearTokens();
        this.router.navigate(['/login']);
      }
    });
  }

  onBreakpoint(broken: any) {
    this.isMobile = !!broken;
    if (this.isMobile) this.isCollapsed = true;
  }

  goHome() {
    this.router.navigate(['/dashboard']);
  }

  goProfile() {
    this.router.navigate(['/profile']);
  }

  goChangePassword() {
    this.changePwd.open();
  }

  onAvatarError(ev: any) {
    ev.target.src = this.defaultAvatar;
  }

}
