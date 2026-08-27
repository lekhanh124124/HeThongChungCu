import { InjectionToken } from '@angular/core';

export const ACCESS_TOKEN_KEY = new InjectionToken<string>('ACCESS_TOKEN_KEY', {
  providedIn: 'root',
  factory: () => 'access_token'
});

export const REFRESH_TOKEN_KEY = new InjectionToken<string>('REFRESH_TOKEN_KEY', {
  providedIn: 'root',
  factory: () => 'refresh_token'
});
