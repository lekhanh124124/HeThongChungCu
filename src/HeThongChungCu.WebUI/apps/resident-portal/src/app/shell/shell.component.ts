import { Component, OnInit, OnDestroy } from '@angular/core';
import { SignalrService } from '@core/signalr';
import { AuthService } from '@core/auth/data-access';
import { environment } from '@env/environment';

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss']
})
export class ShellComponent implements OnInit, OnDestroy {
  isCollapsed = false;
  private _inited = false;

  constructor(private signalr: SignalrService, private auth: AuthService) {}

  ngOnInit(): void {
    // init SignalR after app starts; accessTokenFactory uses AuthService.ensureValidAccessToken
    try {
      this.signalr.init(environment.apiBaseUrl + '/notifications', () => this.auth.ensureValidAccessToken());
      this._inited = true;
    } catch (e) {
      console.warn('SignalR init failed on ShellComponent', e);
    }
  }

  ngOnDestroy(): void {
    if (this._inited) {
      this.signalr.stop().catch(() => {});
    }
  }
}
