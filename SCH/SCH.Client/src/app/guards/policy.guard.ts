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
import { Policy } from '../enums/policy';

/**
 * Policy-based auth guard.
 * Usage: canActivate: [policyGuard], data: { policy: Policy.ViewStudents }
 * Waits for token refresh to complete if in progress.
 */
export const policyGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): Observable<boolean | UrlTree> => {
  const authService = inject(Auth);
  const router = inject(Router);

  const isRefreshing$ = toObservable(authService.isRefreshing);

  return isRefreshing$.pipe(
    filter((isRefreshing) => !isRefreshing),
    take(1),
    switchMap(() => {
      if (!authService.isAuthenticated()) {
        return of(router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } }));
      }

      const policy = route.data['policy'] as Policy | undefined;
      if (!policy || authService.hasPolicy(policy)) {
        return of(true);
      }

      return of(router.createUrlTree(['/unauthorized']));
    })
  );
};
