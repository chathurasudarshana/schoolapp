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
import { Policy } from '../enums';
import { HasPolicyData } from '../interfaces/has-policy-data';

/**
 * Student detail guard.
 * - Admin or Teacher role: always allowed.
 * - Student role: only allowed if route {id} matches own student record.
 * - Otherwise: redirect to /unauthorized.
 */
export const studentDetailGuard: CanActivateFn = (
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
      const policyData: HasPolicyData = { studentId: routeId };

      if (authService.hasPolicy(Policy.EditStudents, policyData)) {
        return of(true);
      }

      return of(router.createUrlTree(['/unauthorized']));
    })
  );
};
