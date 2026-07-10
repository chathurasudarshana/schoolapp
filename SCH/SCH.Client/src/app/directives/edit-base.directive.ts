import { Directive, HostListener } from '@angular/core';
import { HasUnsavedChanges } from '../interfaces/has-unsaved-changes';

@Directive()
export abstract class EditBase implements HasUnsavedChanges {
  abstract hasUnsavedChanges(): boolean;

  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: BeforeUnloadEvent): void {
    if (this.hasUnsavedChanges()) {
      event.preventDefault();
    }
  }
}
