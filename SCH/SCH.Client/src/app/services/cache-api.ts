import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfig } from '../interfaces/app-config';
import { APP_CONFIG } from '../injection-tokens/app-config.token';

@Injectable({
  providedIn: 'root',
})
export class CacheApi {
  private readonly apiUrl: string;

  constructor(
    @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
    private readonly http: HttpClient
  ) {
    this.apiUrl = appConfig.apiUrl;
  }

  public clearCache(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/cache/clear`, null);
  }

  public removeCacheEntry(key: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/cache/${encodeURIComponent(key)}`);
  }
}
