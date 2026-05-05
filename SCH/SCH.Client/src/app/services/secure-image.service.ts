import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { APP_CONFIG } from '../injection-tokens/app-config.token';
import { AppConfig } from '../interfaces/app-config';

/**
 * Service to load images securely through HttpClient with JWT authentication
 * Useful for AG Grid and other scenarios where pipes can't be used
 */
@Injectable({
  providedIn: 'root'
})
export class SecureImageService {
  private readonly apiUrl: string;
  private readonly cache = new Map<string, string>();

  constructor(
    @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
    private readonly http: HttpClient
  ) {
    this.apiUrl = this.appConfig.apiUrl;
  }

  /**
   * Load an image securely and return as an object URL
   * @param imageName The image file name
   * @param endpoint The API endpoint path
   * @returns Observable<string> The object URL or empty string on error
   */
  loadImageAsObjectUrl(
    imageName: string,
    endpoint: string
  ): Observable<string> {
    if (!imageName) {
      return of('');
    }

    // Create cache key with endpoint to avoid collisions
    const cacheKey = `${endpoint}:${imageName}`;

    // Check cache first
    const cached = this.cache.get(cacheKey);
    if (cached) {
      return of(cached);
    }

    // Fetch image through HttpClient (JWT token added by interceptor)
    return this.http.get(
      `${this.apiUrl}/${endpoint}/${imageName}`,
      { responseType: 'blob' }
    ).pipe(
      map(blob => {
        // Create object URL from blob
        const objectUrl = URL.createObjectURL(blob);
        
        // Cache the URL with endpoint-specific key
        this.cache.set(cacheKey, objectUrl);
        
        return objectUrl;
      }),
      catchError(error => {
        console.error('Failed to load image:', imageName, 'from', endpoint, error);
        return of('');
      })
    );
  }

  /**
   * Clear the cache and revoke all object URLs to free memory
   */
  clearCache(): void {
    for (const url of this.cache.values()) {
      URL.revokeObjectURL(url);
    }
    this.cache.clear();
  }

  /**
   * Revoke a specific object URL and remove from cache
   */
  revokeImageUrl(imageName: string): void {
    const url = this.cache.get(imageName);
    if (url) {
      URL.revokeObjectURL(url);
      this.cache.delete(imageName);
    }
  }
}

