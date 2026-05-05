import { Pipe, PipeTransform, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { APP_CONFIG } from '../injection-tokens/app-config.token';

/**
 * Pipe to load images through HttpClient with JWT authentication
 * 
 * Usage:
 * <img [src]="imageName | secureImage:'image/getStudentProfile' | async" alt="Student">
 * <img [src]="imageName | secureImage:'image/getTeacherProfile' | async" alt="Teacher">
 * 
 * This pipe:
 * 1. Fetches the image as a Blob through HttpClient (JWT added by interceptor)
 * 2. Converts the Blob to an object URL
 * 3. Returns it as a SafeUrl for Angular's security
 */
@Pipe({
  name: 'secureImage'
})
export class SecureImage implements PipeTransform {
  private readonly apiUrl: string;
  private readonly cache = new Map<string, string>();
  private readonly http = inject(HttpClient);
  private readonly sanitizer = inject(DomSanitizer);

  constructor() {
    const appConfig = inject(APP_CONFIG);
    this.apiUrl = appConfig.apiUrl;
  }

  transform(
    imageName: string | null | undefined,
    endpoint: string
  ): Observable<SafeUrl> {
    if (!imageName) {
      return of('');
    }

    // Create cache key with endpoint to avoid collisions
    const cacheKey = `${endpoint}:${imageName}`;

    // Check cache first
    const cached = this.cache.get(cacheKey);
    if (cached) {
      return of(this.sanitizer.bypassSecurityTrustUrl(cached));
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
        
        // Return as SafeUrl
        return this.sanitizer.bypassSecurityTrustUrl(objectUrl);
      }),
      catchError(error => {
        console.error('Failed to load image:', imageName, 'from', endpoint, error);
        // Return empty string on error (you could return a placeholder image URL)
        return of('');
      })
    );
  }
}


