import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

@Injectable({ providedIn: 'root' })
export class TriThucApiService {
  private base = (environment && environment.apiBaseUrl)
    ? `${environment.apiBaseUrl.replace(/\/$/, '')}/api/tri-thuc-chatbot`
    : '/api/tri-thuc-chatbot';

  private apiRoot = (environment && environment.apiBaseUrl)
    ? `${environment.apiBaseUrl.replace(/\/$/, '')}/api`
    : '/api';

  constructor(private http: HttpClient) {}

  create(payload: any): Observable<any> {
    return this.http.post<any>(this.base, payload);
  }

  update(payload: any): Observable<any> {
    return this.http.put<any>(this.base, payload);
  }

  delete(ids: number[]): Observable<any> {
    return this.http.request<any>('delete', this.base, { body: { ids } });
  }

  getList(payload: any): Observable<any> {
    return this.http.post<any>(`${this.base}/get-list`, payload);
  }

  getById(id: number): Observable<any> {
    return this.http.post<any>(`${this.base}/get-by-id`, { id });
  }

  getDanhMuc(): Observable<any> {
    return this.http.post<any>(`${this.base}/get-danh-muc`, {});
  }

  activate(id: number): Observable<any> {
    return this.http.put<any>(`${this.base}/activate`, { id });
  }

  deactivate(id: number): Observable<any> {
    return this.http.put<any>(`${this.base}/deactivate`, { id });
  }

  sync(): Observable<any> {
    return this.http.post<any>(`${this.base}/sync`, {});
  }

  importFile(formData: FormData): Observable<any> {
    return this.http.post<any>(`${this.base}/import`, formData);
  }
}
