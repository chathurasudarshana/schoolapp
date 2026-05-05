import { inject } from '@angular/core';
import {
  Router,
  CanActivateFn,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Auth } from '../services/auth';
import { Observable, of } from 'rxjs';
import { filter, take, switchMap } from 'rxjs/operators';
import { toObservable } from '@angular/core/rxjs-interop';

/**
 * Auth guard to protect routes that require authentication
 * Waits for token refresh to complete if in progress
 */
export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): Observable<boolean | UrlTree> => {
  const authService = inject(Auth);
  const router = inject(Router);

  // Convert isRefreshing signal to observable
  const isRefreshing$ = toObservable(authService.isRefreshing);

  // Wait for refresh to complete if in progress
  return isRefreshing$.pipe(
    // Wait until isRefreshing becomes false
    filter(isRefreshing => !isRefreshing),
    // Take only the first emission (when refresh completes)
    take(1),
    // Then check authentication status
    switchMap(() => {
      if (authService.isAuthenticated()) {
        return of(true);
      } else {
        return of(router.createUrlTree(['/login'], {
          queryParams: { returnUrl: state.url },
        }));
      }
    })
  );
};


