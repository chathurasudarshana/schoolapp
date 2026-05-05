import { TestBed } from '@angular/core/testing';
import { SidenavService } from './sidenav.service';

describe('SidenavService', () => {
  let service: SidenavService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SidenavService]
    });
    service = TestBed.inject(SidenavService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should initialize with collapsed state as false', () => {
    expect(service.isCollapsed()).toBe(false);
  });

  it('should toggle sidebar state from false to true', () => {
    expect(service.isCollapsed()).toBe(false);
    
    service.toggleSidebar();
    
    expect(service.isCollapsed()).toBe(true);
  });

  it('should toggle sidebar state from true to false', () => {
    service.setSidebarState(true);
    expect(service.isCollapsed()).toBe(true);
    
    service.toggleSidebar();
    
    expect(service.isCollapsed()).toBe(false);
  });

  it('should set sidebar state to true', () => {
    service.setSidebarState(true);
    
    expect(service.isCollapsed()).toBe(true);
  });

  it('should set sidebar state to false', () => {
    service.setSidebarState(true); // First set to true
    service.setSidebarState(false); // Then set to false
    
    expect(service.isCollapsed()).toBe(false);
  });

  it('should maintain state consistency across multiple operations', () => {
    // Initial state
    expect(service.isCollapsed()).toBe(false);
    
    // Toggle to true
    service.toggleSidebar();
    expect(service.isCollapsed()).toBe(true);
    
    // Set to false
    service.setSidebarState(false);
    expect(service.isCollapsed()).toBe(false);
    
    // Toggle to true again
    service.toggleSidebar();
    expect(service.isCollapsed()).toBe(true);
    
    // Toggle back to false
    service.toggleSidebar();
    expect(service.isCollapsed()).toBe(false);
  });

  it('should provide readonly signal that reflects internal state', () => {
    const readonlySignal = service.isCollapsed;
    
    expect(readonlySignal()).toBe(false);
    
    service.toggleSidebar();
    expect(readonlySignal()).toBe(true);
    
    service.setSidebarState(false);
    expect(readonlySignal()).toBe(false);
  });
});
