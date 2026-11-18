import { Component, inject, signal } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { Auth } from '../../services/auth';
import { Notification } from '../../services/notification';
import { LoginRequest } from '../../interfaces/login-request';

@Component({
  selector: 'app-login-page',
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
    MatCheckboxModule,
  ],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly notificationService = inject(Notification);

  public loginForm: FormGroup;
  public isLoading = signal(false);
  public hidePassword = signal(true);
  
  private returnUrl: string = '/sch/dashboard';

  constructor() {
    this.loginForm = this.formBuilder.nonNullable.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
      rememberMe: [false],
    });

    // Get returnUrl from query params with validation
    this.route.queryParams.subscribe(params => {
      this.returnUrl = this.sanitizeReturnUrl(params['returnUrl']);
    });
  }

  /**
   * Sanitize and validate returnUrl to prevent security issues
   */
  private sanitizeReturnUrl(url: string | null | undefined): string {
    const defaultUrl = '/sch/dashboard';
    
    // If no URL provided, use default
    if (!url) {
      return defaultUrl;
    }

    // Trim whitespace
    url = url.trim();

    // Must start with / (internal URL only, prevent open redirect)
    if (!url.startsWith('/')) {
      return defaultUrl;
    }

    // Prevent redirecting back to auth pages
    const authPages = ['/login', '/register'];
    if (authPages.some(page => url === page || url.startsWith(page + '/'))) {
      return defaultUrl;
    }

    // Prevent redirecting to error pages
    const errorPages = ['/notfound', '/unauthorized', '/servererror'];
    if (errorPages.some(page => url === page || url.startsWith(page + '/'))) {
      return defaultUrl;
    }

    // Valid internal URL
    return url;
  }

  /**
   * Get form field error message
   */
  public getErrorMessage(fieldName: string): string {
    const field = this.loginForm.get(fieldName);

    if (!field?.errors) {
      return '';
    }

    if (field.errors['required']) {
      return `${this.getFieldLabel(fieldName)} is required`;
    }

    return '';
  }

  /**
   * Get human-readable field label
   */
  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      username: 'Username or Email',
      password: 'Password',
    };
    return labels[fieldName] || fieldName;
  }

  /**
   * Handle form submission
   */
  public onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);

    const loginRequest: LoginRequest = this.loginForm.value;

    this.authService
      .login(loginRequest)
      .subscribe({
        next: (response) => {
          this.isLoading.set(false);

          this.notificationService.success(
            `Welcome back, ${response.user.firstName}!`
          );

          // Navigate to returnUrl or default dashboard
          this.router.navigateByUrl(this.returnUrl);
        },
        error: (error) => {
          this.isLoading.set(false);

          let errorMessage = 'Login failed. Please try again.';

          if (error.error?.message) {
            errorMessage = error.error.message;
          } else if (error.status === 401) {
            errorMessage = 'Invalid username or password';
          }

          this.notificationService.error(errorMessage);
        },
      });
  }

  /**
   * Navigate to register page
   */
  public navigateToRegister(): void {
    this.router.navigate(['/register']);
  }
}


