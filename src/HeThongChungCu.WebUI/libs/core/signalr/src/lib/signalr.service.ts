import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel, HttpTransportType } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';

export type AccessTokenGetter = () => string | Promise<string | null> | null;

@Injectable({ providedIn: 'root' })
export class SignalrService implements OnDestroy {
  private hub?: HubConnection;
  private notificationSubject = new BehaviorSubject<any | null>(null);
  private connectionState = new BehaviorSubject<'disconnected'|'connecting'|'connected'>('disconnected');
  private _baseHubUrl?: string;
  private _getToken?: AccessTokenGetter;

  constructor() {}

  /**
   * Initialize hub connection. call once (eg. after login) or in AppComponent.
   * baseHubUrl: full url to hub endpoint, e.g. https://api.example.com/notifications
   * getToken: function to retrieve current access token (can return Promise when refresh is needed)
   */
  init(baseHubUrl: string, getToken: AccessTokenGetter) {
    if (this.hub) return;
    this._baseHubUrl = baseHubUrl;
    this._getToken = getToken;

    this.hub = new HubConnectionBuilder()
      .withUrl(baseHubUrl, {
        accessTokenFactory: async () => {
          try {
            const t = await Promise.resolve(this._getToken && this._getToken());
            return t ?? '';
          } catch {
            return '';
          }
        }
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    this.hub.onclose((err) => {
      this.connectionState.next('disconnected');
    });

    this.hub.onreconnected((id) => {
      this.connectionState.next('connected');
    });

    // Listen to notifications from server
    this.hub.on('ReceiveNotification', (payload: any) => {
      this.notificationSubject.next(payload);
    });

    // Start connection
    this.start().catch(err => console.warn('SignalR start error', err));
  }

  async start(maxRetries = 3): Promise<void> {
    if (!this.hub) throw new Error('SignalR hub not initialized. Call init() first.');
    if (this.hub.state === 'Connected') {
      this.connectionState.next('connected');
      return;
    }

    this.connectionState.next('connecting');

    // ensure token is fresh before attempting start
    try {
      await Promise.resolve(this._getToken && this._getToken()).catch(() => null);
    } catch {}

    let attempt = 0;
    while (attempt < maxRetries) {
      attempt++;
      try {
        await this.hub.start();
        this.connectionState.next('connected');
        return;
      } catch (err: any) {
        console.warn(`SignalR start attempt ${attempt} failed`, err && err.message ? err.message : err);
        // if unauthorized, try refreshing token via getter and retry
        const msg = (err && (err.message || String(err))).toLowerCase();
        if (msg.includes('401') || msg.includes('unauthorized') || msg.includes('access denied')) {
          try {
            // call token getter to force refresh if it performs refresh
            await Promise.resolve(this._getToken && this._getToken()).catch(() => null);
          } catch {}
          // small delay before retry
          await new Promise(res => setTimeout(res, 500 * attempt));
          continue;
        }
        // non-auth errors: retry with backoff
        await new Promise(res => setTimeout(res, 300 * attempt));
      }
    }

    this.connectionState.next('disconnected');
    throw new Error('SignalR failed to start after retries');
  }

  async stop(): Promise<void> {
    if (!this.hub) return;
    try {
      await this.hub.stop();
    } finally {
      this.connectionState.next('disconnected');
    }
  }

  notifications(): Observable<any | null> {
    return this.notificationSubject.asObservable();
  }

  status(): Observable<'disconnected'|'connecting'|'connected'> {
    return this.connectionState.asObservable();
  }

  /** Attach a custom handler for a server method */
  on(method: string, handler: (...args: any[]) => void) {
    this.hub?.on(method, handler);
  }

  ngOnDestroy(): void {
    this.stop().catch(() => {});
  }
}
