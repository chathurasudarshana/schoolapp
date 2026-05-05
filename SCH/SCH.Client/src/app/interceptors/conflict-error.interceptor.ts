import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Notification } from '../services/notification';
import { SCHErrorNumber } from '../enums';
import { AppErrorContent } from '../interfaces/app-error-content';

/**
 * Conflict Error Interceptor
 * Handles 409 Conflict errors based on SCHErrorNumber
 * Shows user-friendly messages for different conflict types
 */
export const conflictErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const notification = inject(Notification);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 409) {
        const errorContent = error.error as AppErrorContent;
        const schErrorNumber = errorContent?.data?.['SCHErrorNumber'];
        
        // Handle different conflict types based on error number
        if (schErrorNumber === SCHErrorNumber.ConcurrencyConflict) {
          notification.error(
            'The record you attempted to update was modified by another user. Please refresh and try again.',
            'Concurrency Conflict'
          );
        }
        // Add more conflict types here as needed with else if
        // else if (schErrorNumber === SCHErrorNumber.SomeOtherConflict) {
        //   notification.error('Some other conflict message', 'Conflict');
        // }
      }
      return throwError(() => error);
    })
  );
};
