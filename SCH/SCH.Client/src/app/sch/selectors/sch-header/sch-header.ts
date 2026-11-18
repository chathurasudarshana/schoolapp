import { Component, computed, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { finalize } from 'rxjs';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { SidenavService } from '../../services/sidenav.service';
import { HighlightDirective } from '../../../directives/highlight.directive';
import { Auth } from '../../../services/auth';
import { CacheApi } from '../../../services/cache-api';
import { HasPolicy } from '../../../directives/has-policy.directive';
import { LogoutScope } from '../../../enums/logout-scope';
import { Policy } from '../../../enums/policy';

@Component({
  selector: 'sch-header',
  imports: [MatButtonModule, MatDividerModule, MatIconModule, MatMenuModule, HighlightDirective, HasPolicy],
  templateUrl: './sch-header.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './sch-header.scss',
})
export class SCHHeader {
  private readonly sidenavService = inject(SidenavService);
  private readonly authService = inject(Auth);
  private readonly cacheApi = inject(CacheApi);
  private readonly router = inject(Router);

  protected readonly Policy = Policy;

  protected readonly isCollapsed = computed(() => this.sidenavService.isCollapsed());
  protected readonly currentUser = computed(() => this.authService.currentUser());
  protected readonly isAuthenticated = computed(() => this.authService.isAuthenticated());
  protected readonly headerHighlighted = signal(false);
  protected readonly clearingCache = signal(false);

  protected onHeaderHighlight(value: boolean): void {
    this.headerHighlighted.set(value);
  }

  public clearCache(): void {
    this.clearingCache.set(true);
    this.cacheApi.clearCache().pipe(
      finalize(() => this.clearingCache.set(false))
    ).subscribe();
  }

  /**
   * Logout from current browser (all tabs in this browser)
   */
  public logout(): void {
    this.authService.logout(LogoutScope.CurrentBrowser);
    this.router.navigate(['/login']);
  }

  /**
   * Logout from all devices
   */
  public logoutEverywhere(): void {
    this.authService.logout(LogoutScope.AllDevices);
    this.router.navigate(['/login']);
  }

  /**
   * Get user initials for avatar
   */
  public getUserInitials(): string {
    const user = this.currentUser();
    if (!user) {
      return '';
    }
    const firstInitial = user.firstName?.charAt(0) || '';
    const lastInitial = user.lastName?.charAt(0) || '';
    return `${firstInitial}${lastInitial}`.toUpperCase();
  }

  /**
   * Get user full name
   */
  public getUserFullName(): string {
    const user = this.currentUser();
    if (!user) {
      return 'Guest';
    }
    return user.fullName || `${user.firstName} ${user.lastName}`;
  }
}
