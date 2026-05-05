import { Injectable, signal, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthApi } from './auth-api';
import { AuthResponse } from '../interfaces/auth-response';
import { User } from '../interfaces/user';
import { LoginRequest } from '../interfaces/login-request';
import { RegisterRequest } from '../interfaces/register-request';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { APP_CONFIG } from '../injection-tokens/app-config.token';
import { LogoutScope } from '../enums/logout-scope';

/**
 * Authentication service that manages user state, tokens, and auth logic
 */
@Injectable({
  providedIn: 'root',
})
export class Auth {
  private readonly authApi = inject(AuthApi);
  private readonly router = inject(Router);
  private readonly config = inject(APP_CONFIG);

  // Token keys for localStorage
  private readonly ACCESS_TOKEN_KEY = 'accessToken';
  private readonly REFRESH_TOKEN_KEY = 'refreshToken';
  private readonly USER_KEY = 'user';
  private readonly TOKEN_EXPIRY_KEY = 'tokenExpiry';
  private readonly TOKEN_ISSUE_TIME_KEY = 'tokenIssueTime';
  private readonly REMEMBER_ME_KEY = 'rememberMe';
  private readonly REFRESH_TOKEN_EXPIRY_KEY = 'refreshTokenExpiry';

  // Timer management
  private idleTimer?: ReturnType<typeof setTimeout>;
  private refreshTimer?: ReturnType<typeof setTimeout>;
  private activityChannel?: BroadcastChannel;
  private lastActivityTime = Date.now();

  // Signals for reactive state management
  private readonly currentUserSignal = signal<User | null>(null);
  private readonly isAuthenticatedSignal = signal<boolean>(false);
  private readonly isRefreshingSignal = signal<boolean>(false);

  // Computed signals
  public readonly currentUser = computed(() => this.currentUserSignal());
  public readonly isAuthenticated = computed(() => this.isAuthenticatedSignal());
  public readonly isRefreshing = computed(() => this.isRefreshingSignal());
  public readonly isAdmin = computed(() =>
    this.currentUser()?.roles.includes('Admin') ?? false
  );
  public readonly isBasic = computed(() =>
    this.currentUser()?.roles.includes('Basic') ?? false
  );

  constructor() {
    this.initializeAuthState();
  }

  /**
   * Initialize authentication state from localStorage
   * Note: Does not handle navigation - auth guard will redirect if needed
   */
  private initializeAuthState(): void {
    const token = this.getAccessToken();
    const user = this.getUserFromStorage();
    const expiryTime = this.getTokenExpiryFromStorage();
    const refreshTokenExpiry = this.getRefreshTokenExpiryFromStorage();
    const rememberMe = this.getRememberMeFromStorage();

    if (!token || !user || !expiryTime) {
      return;
    }

    const now = Date.now();

    // Check if access token is expired
    if (now >= expiryTime) {
      // If RememberMe is OFF, logout immediately (don't use refresh token)
      if (!rememberMe) {
        this.logoutLocal();  // Token expired, just clear local state
        return;
      }

      // If RememberMe is ON, check if refresh token is valid
      if (!refreshTokenExpiry || now >= refreshTokenExpiry) {
        // Refresh token also expired, logout
        this.logoutLocal();  // Token expired, just clear local state
        return;
      }

      // Refresh token is still valid, try to refresh
      // Set refreshing state to true so guards can wait
      this.isRefreshingSignal.set(true);
      
      // Defer the refresh call to avoid circular dependency during service construction
      setTimeout(() => {
      this.refreshToken().subscribe({
        next: () => {
          // Success - handleAuthSuccess already updated the state
          this.isRefreshingSignal.set(false);
        },
          error: (error) => {
          // Failed - refreshToken() already cleared auth state
            console.error('Refresh token error:', error);
          this.isRefreshingSignal.set(false);
        }
      });
      }, 0);
      return;
    }

    // Access token is still valid
    this.currentUserSignal.set(user);
    this.isAuthenticatedSignal.set(true);
    this.startTimers();
  }

  /**
   * Login user
   */
  public login(request: LoginRequest): Observable<AuthResponse> {
    return this.authApi.login(request).pipe(
      tap((response) => this.handleAuthSuccess(response, request.rememberMe ?? false)),
      catchError((error) => {
        this.clearAuthState();
        return throwError(() => error);
      })
    );
  }

  /**
   * Register new user
   */
  public register(request: RegisterRequest): Observable<AuthResponse> {
    return this.authApi.register(request).pipe(
      tap((response) => this.handleAuthSuccess(response)),
      catchError((error) => {
        this.clearAuthState();
        return throwError(() => error);
      })
    );
  }

  /**
   * Logout user with specified scope
   * @param scope - Logout scope (CurrentSession, CurrentBrowser, AllDevices)
   * Note: Does not handle navigation - caller should navigate as needed
   */
  public logout(scope: LogoutScope = LogoutScope.CurrentBrowser): void {
    const token = this.getAccessToken();
    const refreshToken = this.getRefreshToken();
    
    // Only call backend if token is still valid (not expired)
    if (token && refreshToken && !this.isTokenExpired(token)) {
      // Token is valid, call backend to revoke
      this.authApi.logout(scope, refreshToken).subscribe({
        error: (error) => console.error('Logout API error:', error),
      });
    }
    // If token is expired, skip backend call (token already invalid)
    // Always clear local state regardless
    this.logoutLocal();
  }

  /**
   * Clear local auth state without calling backend
   * Private helper method
   */
  private logoutLocal(): void {
    this.clearAuthState();
  }

  /**
   * Record user activity (public for use in interceptor)
   */
  public recordActivity(): void {
    // Don't record activity if user is not authenticated (e.g., during logout)
    if (!this.isAuthenticated()) {
      return;
    }

    this.lastActivityTime = Date.now();
    
    // Safely post to channel - it might be closed during logout
    if (this.activityChannel) {
      try {
        this.activityChannel.postMessage({ type: 'activity', timestamp: Date.now() });
      } catch {
        // Channel is closed during logout, safe to ignore
      }
    }
    
    this.resetIdleTimer();
  }

  /**
   * Start all timers
   */
  private startTimers(): void {
    this.clearTimers();
    this.setupActivityDetection();
    this.startIdleTimer();
    this.startRefreshTimer();
  }

  /**
   * Setup activity detection with cross-tab support
   */
  private setupActivityDetection(): void {
    const events = ['mousedown', 'keydown', 'scroll', 'touchstart'];
    const handler = () => this.recordActivity();

    for (const event of events) {
      globalThis.addEventListener(event, handler, { passive: true });
    }

    // Cross-tab communication
    this.activityChannel = new BroadcastChannel('user_activity');
    this.activityChannel.onmessage = () => this.recordActivity();
  }

  /**
   * Start idle timer
   */
  private startIdleTimer(): void {
    // Validate config
    if (!this.config.idleLogoutTime || 
        this.config.idleLogoutTime <= 0 || 
        this.config.idleLogoutTime > 1) {
      return; // Skip idle timer if config is invalid
    }

    const expiryTime = this.getTokenExpiryFromStorage();
    const issueTime = this.getTokenIssueTimeFromStorage();
    if (!expiryTime || !issueTime) return;

    const tokenDurationMs = expiryTime - issueTime;
    const idleTimeout = tokenDurationMs * this.config.idleLogoutTime;

    this.idleTimer = globalThis.setTimeout(() => {
      this.logout(LogoutScope.CurrentBrowser);
      this.navigateToLogin({ reason: 'idle' });
    }, idleTimeout);
  }

  /**
   * Reset idle timer
   */
  private resetIdleTimer(): void {
    if (this.idleTimer) {
      clearTimeout(this.idleTimer);
    }
    this.startIdleTimer();
  }

  /**
   * Start refresh timer based on original token issue time
   */
  private startRefreshTimer(): void {
    // Validate config
    if (!this.config.refreshTokenTime || 
        this.config.refreshTokenTime <= 0 || 
        this.config.refreshTokenTime > 1) {
      return; // Skip refresh timer if config is invalid
    }

    const issueTime = this.getTokenIssueTimeFromStorage();
    const expiryTime = this.getTokenExpiryFromStorage();
    if (!issueTime || !expiryTime) return;

    const now = Date.now();
    const durationMs = expiryTime - issueTime;
    const refreshAtTime = issueTime + (durationMs * (1 - this.config.refreshTokenTime));
    const timeUntilRefresh = refreshAtTime - now;

    // If already past refresh time, refresh immediately
    if (timeUntilRefresh <= 0) {
      this.refreshToken().subscribe({
        error: (err) => {
          console.error('Auto-refresh failed:', err);
          this.navigateToLogin();
        }
      });
      return;
    }

    this.refreshTimer = globalThis.setTimeout(() => {
      this.refreshToken().subscribe({
        error: (err) => {
          console.error('Auto-refresh failed:', err);
          this.navigateToLogin();
        }
      });
    }, timeUntilRefresh);
  }

  /**
   * Clear all timers
   */
  private clearTimers(): void {
    if (this.idleTimer) clearTimeout(this.idleTimer);
    if (this.refreshTimer) clearTimeout(this.refreshTimer);
    this.activityChannel?.close();
  }

  /**
   * Navigate to login with current URL as returnUrl
   * Common method for logout scenarios
   */
  private navigateToLogin(queryParams?: Record<string, string>): void {
    const currentUrl = this.router.url;
    this.router.navigate(['/login'], {
      queryParams: {
        returnUrl: currentUrl,
        ...queryParams
      }
    });
  }

  /**
   * Get token expiry from localStorage
   */
  private getTokenExpiryFromStorage(): number | null {
    const expiry = localStorage.getItem(this.TOKEN_EXPIRY_KEY);
    return expiry ? Number.parseInt(expiry, 10) : null;
  }

  /**
   * Get token issue time from localStorage
   */
  private getTokenIssueTimeFromStorage(): number | null {
    const issueTime = localStorage.getItem(this.TOKEN_ISSUE_TIME_KEY);
    return issueTime ? Number.parseInt(issueTime, 10) : null;
  }

  /**
   * Get remember me from localStorage
   */
  private getRememberMeFromStorage(): boolean {
    const rememberMe = localStorage.getItem(this.REMEMBER_ME_KEY);
    return rememberMe === 'true';
  }

  /**
   * Get refresh token expiry from localStorage
   */
  private getRefreshTokenExpiryFromStorage(): number | null {
    const expiry = localStorage.getItem(this.REFRESH_TOKEN_EXPIRY_KEY);
    return expiry ? Number.parseInt(expiry, 10) : null;
  }

  /**
   * Refresh access token
   * Note: Does not handle navigation - caller should navigate as needed
   */
  public refreshToken(): Observable<AuthResponse> {
    const accessToken = this.getAccessToken();
    const refreshToken = this.getRefreshToken();
    const refreshTokenExpiry = this.getRefreshTokenExpiryFromStorage();
    const rememberMe = this.getRememberMeFromStorage();

    if (!accessToken || !refreshToken) {
      this.clearAuthState();
      return throwError(() => new Error('No tokens available'));
    }

    // Check if refresh token is expired before calling backend
    if (refreshTokenExpiry && Date.now() >= refreshTokenExpiry) {
      this.clearAuthState();
      return throwError(() => new Error('Refresh token expired'));
    }

    return this.authApi.refreshToken({ accessToken, refreshToken }).pipe(
      tap((response) => this.handleAuthSuccess(response, rememberMe ?? false)),
      catchError((error) => {
        // Don't call logout() here - it would make another HTTP call creating circular dependency
        // Just clear local state since refresh failed (backend unreachable or tokens invalid)
        this.clearAuthState();
        return throwError(() => error);
      })
    );
  }

  /**
   * Get access token from localStorage
   */
  public getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  /**
   * Get refresh token from localStorage
   */
  public getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Check if user has a specific role
   */
  public hasRole(role: string): boolean {
    return this.currentUser()?.roles.includes(role) ?? false;
  }

  /**
   * Check if user has any of the specified roles
   */
  public hasAnyRole(roles: string[]): boolean {
    const userRoles = this.currentUser()?.roles ?? [];
    return roles.some((role) => userRoles.includes(role));
  }

  /**
   * Handle successful authentication
   */
  private handleAuthSuccess(response: AuthResponse, rememberMe: boolean = false): void {
    const now = Date.now();
    const expiryTime = now + (response.expiresIn * 1000);
    const refreshTokenExpiryTime = now + (response.refreshTokenExpiresIn * 1000);

    // Store tokens and timing info
    localStorage.setItem(this.ACCESS_TOKEN_KEY, response.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));
    localStorage.setItem(this.TOKEN_EXPIRY_KEY, expiryTime.toString());
    localStorage.setItem(this.TOKEN_ISSUE_TIME_KEY, now.toString());
    localStorage.setItem(this.REMEMBER_ME_KEY, rememberMe.toString());
    localStorage.setItem(this.REFRESH_TOKEN_EXPIRY_KEY, refreshTokenExpiryTime.toString());

    // Update state
    this.currentUserSignal.set(response.user);
    this.isAuthenticatedSignal.set(true);

    // Start timers
    this.startTimers();
  }

  /**
   * Clear authentication state
   */
  private clearAuthState(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.TOKEN_EXPIRY_KEY);
    localStorage.removeItem(this.TOKEN_ISSUE_TIME_KEY);
    localStorage.removeItem(this.REMEMBER_ME_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_EXPIRY_KEY);

    this.currentUserSignal.set(null);
    this.isAuthenticatedSignal.set(false);
    this.clearTimers();
  }

  /**
   * Get user from localStorage
   */
  private getUserFromStorage(): User | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    if (!userJson) {
      return null;
    }

    try {
      return JSON.parse(userJson) as User;
    } catch {
      return null;
    }
  }

  /**
   * Check if token is expired (basic check - you might want to use a JWT library)
   */
  public isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiryTime = payload.exp * 1000; // Convert to milliseconds
      return Date.now() >= expiryTime;
    } catch {
      return true;
    }
  }
}



