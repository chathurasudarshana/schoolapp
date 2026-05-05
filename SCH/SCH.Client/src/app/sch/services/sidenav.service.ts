import { Injectable, signal } from '@angular/core';

@Injectable()
export class SidenavService {
  private readonly _isCollapsed = signal<boolean>(false);
  public readonly isCollapsed = this._isCollapsed.asReadonly();

  constructor() {}

  toggleSidebar(): void {
    this._isCollapsed.update(collapsed => !collapsed);
  }

  setSidebarState(collapsed: boolean): void {
    this._isCollapsed.set(collapsed);
  }
}
