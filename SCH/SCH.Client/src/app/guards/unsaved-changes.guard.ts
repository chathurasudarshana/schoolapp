import { inject } from '@angular/core';
import { CanDeactivateFn } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { ConfirmDialog } from '../selectors/confirm-dialog/confirm-dialog';
import { HasUnsavedChanges } from '../interfaces/has-unsaved-changes';


/**
 * Guard that prompts the user to confirm navigation away from a page
 * with unsaved form changes.
 */
export const unsavedChangesGuard: CanDeactivateFn<HasUnsavedChanges> = (
  component
): boolean | Observable<boolean> => {

  if (!component.hasUnsavedChanges()) {
    return true;
  }

  const dialog = inject(MatDialog);

  return dialog
    .open(ConfirmDialog, {
      data: {
        message: 'You have unsaved changes. Are you sure you want to leave?',
        cancelText: 'Stay',
        confirmText: 'Leave',
      },
    })
    .afterClosed();
};
