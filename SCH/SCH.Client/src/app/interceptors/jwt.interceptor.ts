import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';
import { catchError, tap, throwError } from 'rxjs';

/**
 * JWT Interceptor to automatically add authentication token to HTTP requests
 */
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(Auth);

  // Skip adding token for auth endpoints
  const isAuthEndpoint =
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/register') ||
    req.url.includes('/auth/refresh') ||
    req.url.includes('/auth/check-username') ||
    req.url.includes('/auth/check-email');

  if (isAuthEndpoint) {
    return next(req);
  }

  // Get access token
  const token = authService.getAccessToken();

  if (!token) {
    return next(req);
  }

  // Clone request and add Authorization header
  const clonedReq = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
  });

  return next(clonedReq).pipe(
    tap(() => {
      // Record API request as user activity
      authService.recordActivity();
    }),
    catchError((error) => {
      // If 401 Unauthorized, clear auth state 
      // (navigation handled by unauthorizedInterceptor)
      if (error.status === 401) {
        authService.logout();
      }

      return throwError(() => error);
    })
  );
};


