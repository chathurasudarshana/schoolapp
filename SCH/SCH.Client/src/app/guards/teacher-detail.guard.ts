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
import { Policy, Role } from '../enums';
import { HasPolicyData } from '../interfaces/has-policy-data';

/**
 * Teacher detail guard.
 * - Admin: always allowed.
 * - Teacher role: allowed only if route {id} matches own teacher record.
 * - Student role: always blocked → /unauthorized.
 * - Otherwise: /unauthorized.
 */
export const teacherDetailGuard: CanActivateFn = (
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

      const routeId = Number(route.paramMap.get('id'));
      const policyData: HasPolicyData = { teacherId: routeId };

      if (authService.hasPolicy(Policy.EditTeachers, policyData)) {
        return of(true);
      }

      // Student role and all others: block
      return of(router.createUrlTree(['/unauthorized']));
    })
  );
};
