import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DoiTacDto, HopDongDto } from './doi-tac.model';
import { environment } from '@env/environment';

@Injectable({ providedIn: 'root' })
export class DoiTacApiService {
  private base = (environment && environment.apiBaseUrl)
    ? `${environment.apiBaseUrl.replace(/\/$/, '')}/api/doi-tac`
    : '/api/doi-tac';

  constructor(private http: HttpClient) {}

  create(doiTac: Partial<DoiTacDto>): Observable<any> {
    return this.http.post<any>(this.base, doiTac);
  }

  update(doiTac: DoiTacDto): Observable<any> {
    return this.http.put<any>(this.base, doiTac);
  }

  delete(ids: number[]): Observable<any> {
    return this.http.request<any>('delete', this.base, { body: { ids } });
  }

  getList(payload: { keyword?: string; pageNumber: number; pageSize: number; sortCol?: string; isAsc?: boolean }): Observable<any> {
    return this.http.post<any>(`${this.base}/get-list`, payload);
  }

  getById(id: number): Observable<any> {
    return this.http.post<any>(`${this.base}/get-by-id`, { id });
  }

  uploadMedia(files: FileList | any[], target?: string): Observable<any> {
    const formData = new FormData();

    if (files instanceof FileList) {
      for (let i = 0; i < files.length; i++) {
        const f = files.item(i);
        if (f) formData.append('files', f);
      }
    } else if (Array.isArray(files)) {
      files.forEach(f => {
        if (!f) { return; }
        const real: any = (f as any).originFileObj || (f as any).file || (f as any).raw || f;
        if (real) formData.append('files', real as Blob);
      });
    }

    if (target) {
      const aliases = ['target', 'category', 'targetContainer', 'targetCategory', 'container', 'containerName', 'purpose'];
      for (const key of aliases) {
        try { formData.append(key, target); } catch (e) { }
      }
    }

    const apiRoot = (environment && environment.apiBaseUrl)
      ? `${environment.apiBaseUrl.replace(/\/$/, '')}/api`
      : '/api';

    return this.http.post<any>(`${apiRoot}/upload-media`, formData);
  }

  createContract(doiTacId: number, hopDong: HopDongDto): Observable<any> {
    const payload: any = { doiTacId: doiTacId, hopDong: hopDong };
    return this.http.post<any>(`${this.base}/hop-dong`, payload);
  }

  getTrangThaiHopDongForSelector(query?: any): Observable<any> {
    const apiRoot = (environment && environment.apiBaseUrl)
      ? `${environment.apiBaseUrl.replace(/\/$/, '')}/api`
      : '/api';
    return this.http.post<any>(`${apiRoot}/catalog/trang-thai-hop-dong-for-selector`, query || {});
  }

  revokeContracts(doiTacId: number, ids: number[]): Observable<any> {
    return this.http.request<any>('delete', `${this.base}/hop-dong/revoke`, { body: { doiTacId, ids } });
  }
}
