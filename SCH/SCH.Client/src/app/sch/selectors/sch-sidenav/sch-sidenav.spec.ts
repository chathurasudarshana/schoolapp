import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SCHSideNav } from './sch-sidenav';
import { SidenavService } from '../../services/sidenav.service';

describe('SCHSideNav', () => {
  let component: SCHSideNav;
  let fixture: ComponentFixture<SCHSideNav>;
  let sidenavService: SidenavService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SCHSideNav],
      providers: [
        provideRouter([]),
        SidenavService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SCHSideNav);
    component = fixture.componentInstance;
    sidenavService = TestBed.inject(SidenavService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with collapsed state from service', () => {
    expect(component.isCollapsed()).toBe(false);
  });

  it('should have navigation items', () => {
    expect(component.navigationItems).toBeDefined();
    expect(component.navigationItems.length).toBe(4);
    expect(component.navigationItems[0].label).toBe('Dashboard');
    expect(component.navigationItems[0].icon).toBe('bi-speedometer2');
  });

  it('should toggle sidebar through service', () => {
    expect(component.isCollapsed()).toBe(false);
    
    component.toggleSidebar();
    
    expect(component.isCollapsed()).toBe(true);
    expect(sidenavService.isCollapsed()).toBe(true);
  });

  it('should reflect service state changes', () => {
    expect(component.isCollapsed()).toBe(false);
    
    sidenavService.setSidebarState(true);
    
    expect(component.isCollapsed()).toBe(true);
  });

  it('should have correct navigation items structure', () => {
    expect(component.navigationItems).toBeDefined();
    expect(component.navigationItems.length).toBe(4);
    
    const expectedItems = [
      { label: 'Dashboard', route: '/sch/dashboard', icon: 'bi-speedometer2' },
      { label: 'Students', route: '/sch/student', icon: 'bi-people-fill' },
      { label: 'Teachers', route: '/sch/teacher', icon: 'bi-person-workspace' },
      { label: 'Courses', route: '/sch/course', icon: 'bi-book-half' }
    ];
    
    expectedItems.forEach((expectedItem, index) => {
      expect(component.navigationItems[index]).toEqual(expectedItem);
    });
  });

  it('should respond to service state changes correctly', () => {
    // Test initial state
    expect(component.isCollapsed()).toBe(false);
    
    // Test setting to collapsed
    sidenavService.setSidebarState(true);
    expect(component.isCollapsed()).toBe(true);
    
    // Test setting back to expanded
    sidenavService.setSidebarState(false);
    expect(component.isCollapsed()).toBe(false);
    
    // Test toggle functionality
    sidenavService.toggleSidebar();
    expect(component.isCollapsed()).toBe(true);
  });

  it('should hide labels when collapsed', () => {
    sidenavService.setSidebarState(true);
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    const labels = compiled.querySelectorAll('.nav-label');
    
    expect(labels.length).toBe(0);
  });
});
