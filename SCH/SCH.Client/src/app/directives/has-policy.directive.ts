import {
  Directive,
  TemplateRef,
  ViewContainerRef,
  inject,
  effect,
  input,
} from '@angular/core';
import { Auth } from '../services/auth';
import { Policy } from '../enums/policy';

/**
 * Structural directive that conditionally renders its host element
 * based on whether the current user satisfies the given policy.
 *
 * Usage:
 *   <button *hasPolicy="Policy.DeleteStudents">Delete</button>
 *
 * The element is removed from the DOM (not just hidden) when the policy fails.
 */
@Directive({
  selector: '[hasPolicy]',
  standalone: true,
})
export class HasPolicy {
  readonly hasPolicy = input<Policy | null>(null);

  private hasView = false;

  private readonly templateRef = inject(TemplateRef<unknown>);
  private readonly viewContainer = inject(ViewContainerRef);
  private readonly auth = inject(Auth);

  constructor() {
    effect(() => {
      // Reading both signals registers them as dependencies so the effect
      // re-runs whenever the policy input or the current user changes.
      const policy = this.hasPolicy();
      this.auth.currentUser(); // read signal to register as dependency
      this.updateView(policy);
    });
  }

  private updateView(policy: Policy | null): void {
    const allowed = policy ? this.auth.hasPolicy(policy) : true;
    if (allowed && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!allowed && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
}
