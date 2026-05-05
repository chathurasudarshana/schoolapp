import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  AsyncValidatorFn,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, catchError, debounceTime, switchMap, first } from 'rxjs/operators';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthApi } from '../../services/auth-api';
import { Auth } from '../../services/auth';
import { Notification } from '../../services/notification';
import { RegisterRequest } from '../../interfaces/register-request';

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './register-page.html',
  styleUrl: './register-page.scss',
})
export class RegisterPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authApi = inject(AuthApi);
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);
  private readonly notificationService = inject(Notification);

  public registerForm: FormGroup;
  public isLoading = signal(false);
  public hidePassword = signal(true);
  public hideConfirmPassword = signal(true);

  constructor() {
    this.registerForm = this.formBuilder.nonNullable.group(
      {
        username: [
          '',
          [
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(50),
          ],
          [this.usernameAvailableValidator()],
        ],
        email: [
          '',
          [Validators.required, Validators.email],
          [this.emailAvailableValidator()],
        ],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(8),
            Validators.maxLength(100),
            this.hasLowercaseValidator,
            this.hasUppercaseValidator,
            this.hasDigitValidator,
            this.hasSpecialCharValidator,
          ],
        ],
        confirmPassword: ['', [Validators.required]],
        firstName: ['', [Validators.required, Validators.maxLength(100)]],
        lastName: ['', [Validators.required, Validators.maxLength(100)]],
      },
      {
        validators: this.passwordMatchValidator,
      }
    );
  }

  /**
   * Async validator: Check if username is available
   */
  private usernameAvailableValidator(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      if (!control.value) {
        return of(null);
      }

      return of(control.value).pipe(
        debounceTime(500), // Wait 500ms after user stops typing
        switchMap((username) =>
          this.authApi.checkUsername(username).pipe(
            map((response) =>
              response.isAvailable ? null : { usernameTaken: true }
            ),
            catchError(() => of(null))
          )
        ),
        first()
      );
    };
  }

  /**
   * Async validator: Check if email is available
   */
  private emailAvailableValidator(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      if (!control.value) {
        return of(null);
      }

      return of(control.value).pipe(
        debounceTime(500), // Wait 500ms after user stops typing
        switchMap((email) =>
          this.authApi.checkEmail(email).pipe(
            map((response) =>
              response.isAvailable ? null : { emailTaken: true }
            ),
            catchError(() => of(null))
          )
        ),
        first()
      );
    };
  }

  /**
   * Validator: Password must contain at least one lowercase letter
   */
  private hasLowercaseValidator(
    control: AbstractControl
  ): ValidationErrors | null {
    const value = control.value;
    if (!value) {
      return null;
    }
    const hasLowercase = /[a-z]/.test(value);
    return hasLowercase ? null : { hasLowercase: true };
  }

  /**
   * Validator: Password must contain at least one uppercase letter
   */
  private hasUppercaseValidator(
    control: AbstractControl
  ): ValidationErrors | null {
    const value = control.value;
    if (!value) {
      return null;
    }
    const hasUppercase = /[A-Z]/.test(value);
    return hasUppercase ? null : { hasUppercase: true };
  }

  /**
   * Validator: Password must contain at least one digit
   */
  private hasDigitValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) {
      return null;
    }
    const hasDigit = /\d/.test(value);
    return hasDigit ? null : { hasDigit: true };
  }

  /**
   * Validator: Password must contain at least one special character
   */
  private hasSpecialCharValidator(
    control: AbstractControl
  ): ValidationErrors | null {
    const value = control.value;
    if (!value) {
      return null;
    }
    const hasSpecialChar = /[^a-zA-Z0-9]/.test(value);
    return hasSpecialChar ? null : { hasSpecialChar: true };
  }

  /**
   * Custom validator to check if password and confirmPassword match
   */
  private passwordMatchValidator(
    control: AbstractControl
  ): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value
      ? null
      : { passwordMismatch: true };
  }

  /**
   * Get form field error message
   */
  public getErrorMessage(fieldName: string): string {
    const field = this.registerForm.get(fieldName);

    if (!field?.errors) {
      return '';
    }

    if (field.errors['required']) {
      return `${this.getFieldLabel(fieldName)} is required`;
    }
    if (field.errors['email']) {
      return 'Invalid email address';
    }
    if (field.errors['minlength']) {
      const minLength = field.errors['minlength'].requiredLength;
      return `${this.getFieldLabel(
        fieldName
      )} must be at least ${minLength} characters`;
    }
    if (field.errors['maxlength']) {
      const maxLength = field.errors['maxlength'].requiredLength;
      return `${this.getFieldLabel(
        fieldName
      )} cannot exceed ${maxLength} characters`;
    }
    if (field.errors['hasLowercase']) {
      return 'Password must contain at least one lowercase letter (a-z)';
    }
    if (field.errors['hasUppercase']) {
      return 'Password must contain at least one uppercase letter (A-Z)';
    }
    if (field.errors['hasDigit']) {
      return 'Password must contain at least one digit (0-9)';
    }
    if (field.errors['hasSpecialChar']) {
      return 'Password must contain at least one special character';
    }
    if (field.errors['usernameTaken']) {
      return 'Username is already taken';
    }
    if (field.errors['emailTaken']) {
      return 'Email is already registered';
    }

    return '';
  }

  /**
   * Get password match error message
   */
  public getPasswordMatchError(): string {
    if (
      this.registerForm.errors?.['passwordMismatch'] &&
      this.registerForm.get('confirmPassword')?.touched
    ) {
      return 'Passwords do not match';
    }
    return '';
  }

  /**
   * Get human-readable field label
   */
  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      username: 'Username',
      email: 'Email',
      password: 'Password',
      confirmPassword: 'Confirm Password',
      firstName: 'First Name',
      lastName: 'Last Name',
    };
    return labels[fieldName] || fieldName;
  }

  /**
   * Handle form submission
   */
  public onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      this.notificationService.error('Please correct the form errors.');
      return;
    }

    this.isLoading.set(true);

    const registerRequest: RegisterRequest = this.registerForm.value;

    this.authService
      .register(registerRequest)
      .subscribe({
        next: (response) => {
          this.isLoading.set(false);

          this.notificationService.success(
            `Welcome ${response.user.firstName}! Registration successful.`
          );

          // Navigate to dashboard or home page
          this.router.navigate(['/sch/dashboard']);
        },
        error: (error) => {
          this.isLoading.set(false);

          let errorMessage = 'Registration failed. Please try again.';

          if (error.error?.message) {
            errorMessage = error.error.message;
          } else if (error.error?.errors) {
            // Handle validation errors from backend
            const errors = error.error.errors;
            const firstError = Object.values(errors)[0];
            if (Array.isArray(firstError) && firstError.length > 0) {
              errorMessage = firstError[0] as string;
            }
          }

          this.notificationService.error(errorMessage);
        },
      });
  }

  /**
   * Navigate to login page
   */
  public navigateToLogin(): void {
    this.router.navigate(['/login']);
  }
}
