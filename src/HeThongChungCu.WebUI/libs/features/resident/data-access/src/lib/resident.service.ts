import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Resident } from './resident.model';

@Injectable({ providedIn: 'root' })
export class ResidentService {
  getResidents(): Observable<Resident[]> {
    return of([]);
  }
}
