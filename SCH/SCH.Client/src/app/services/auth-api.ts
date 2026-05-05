import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfig } from '../interfaces/app-config';
import { APP_CONFIG } from '../injection-tokens/app-config.token';
import { AuthResponse } from '../interfaces/auth-response';
import { ChangePasswordRequest } from '../interfaces/change-password-request';
import { LoginRequest } from '../interfaces/login-request';
import { RefreshTokenRequest } from '../interfaces/refresh-token-request';
import { RegisterRequest } from '../interfaces/register-request';
import { SessionInfo } from '../interfaces/session-info';
import { User } from '../interfaces/user';
import { LogoutScope } from '../enums/logout-scope';

@Injectable({
  providedIn: 'root'
})
export class AuthApi {

  private readonly apiUrl: string;

  constructor(
    @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
    private readonly http: HttpClient
  ) {
    this.apiUrl = this.appConfig.apiUrl;
  }

  /**
   * Login with username and password
   */
  public login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, request);
  }

  /**
   * Register a new user
   */
  public register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/register`, request);
  }

  /**
   * Refresh access token using refresh token
   */
  public refreshToken(request: RefreshTokenRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/refresh`, request);
  }

  /**
   * Logout with specified scope
   * @param scope - Logout scope (CurrentSession, CurrentBrowser, AllDevices)
   * @param refreshToken - Optional refresh token to send with request
   */
  public logout(
    scope: LogoutScope = LogoutScope.CurrentBrowser,
    refreshToken?: string
  ): Observable<{ message: string }> {
    const body = refreshToken ? { refreshToken } : {};
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/auth/logout?scope=${scope}`,
      body
    );
  }

  /**
   * Get current user information
   */
  public getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/auth/me`);
  }

  /**
   * Get all active sessions for the current user
   */
  public getActiveSessions(): Observable<SessionInfo[]> {
    return this.http.get<SessionInfo[]>(`${this.apiUrl}/auth/sessions`);
  }

  /**
   * Revoke a specific session (refresh token)
   */
  public revokeSession(sessionId: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/auth/sessions/${sessionId}`);
  }

  /**
   * Change password for the current user
   */
  public changePassword(request: ChangePasswordRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/auth/change-password`, request);
  }

  /**
   * Test endpoint to verify authentication
   */
  public testAuth(): Observable<{ message: string }> {
    return this.http.get<{ message: string }>(`${this.apiUrl}/auth/test-auth`);
  }

  /**
   * Test endpoint to verify admin role
   */
  public testAdmin(): Observable<{ message: string }> {
    return this.http.get<{ message: string }>(`${this.apiUrl}/auth/test-admin`);
  }

  /**
   * Check if username is available
   */
  public checkUsername(username: string): Observable<{ isAvailable: boolean }> {
    return this.http.get<{ isAvailable: boolean }>(
      `${this.apiUrl}/auth/check-username/${encodeURIComponent(username)}`
    );
  }

  /**
   * Check if email is available
   */
  public checkEmail(email: string): Observable<{ isAvailable: boolean }> {
    return this.http.get<{ isAvailable: boolean }>(
      `${this.apiUrl}/auth/check-email/${encodeURIComponent(email)}`
    );
  }
}

