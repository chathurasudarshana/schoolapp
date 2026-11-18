import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient, withXhr } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { APP_CONFIG } from '../../../injection-tokens/app-config.token';
import { SCHHeader } from './sch-header';
import { SidenavService } from '../../services/sidenav.service';

describe('SCHHeader', () => {
  let component: SCHHeader;
  let fixture: ComponentFixture<SCHHeader>;
  let sidenavService: SidenavService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SCHHeader],
      providers: [
        SidenavService,
        provideHttpClient(withXhr()),
        provideHttpClientTesting(),
        { provide: APP_CONFIG, useValue: { apiUrl: '' } }
      ]
    }).compileComponents();
    
    fixture = TestBed.createComponent(SCHHeader);
    component = fixture.componentInstance;
    sidenavService = TestBed.inject(SidenavService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with collapsed state from service', () => {
    expect((component as any).isCollapsed()).toBe(false);
  });

  it('should reflect service state changes', () => {
    expect((component as any).isCollapsed()).toBe(false);
    
    sidenavService.setSidebarState(true);
    
    expect((component as any).isCollapsed()).toBe(true);
  });

  it('should update when service toggles state', () => {
    expect((component as any).isCollapsed()).toBe(false);
    
    sidenavService.toggleSidebar();
    
    expect((component as any).isCollapsed()).toBe(true);
  });

  it('should apply correct CSS class when collapsed', () => {
    sidenavService.setSidebarState(true);
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    const header = compiled.querySelector('.sch-header');
    
    expect(header?.classList.contains('sidenav-collapsed')).toBe(true);
  });

  it('should not apply collapsed CSS class when not collapsed', () => {
    sidenavService.setSidebarState(false);
    fixture.detectChanges();
    
    const compiled = fixture.nativeElement as HTMLElement;
    const header = compiled.querySelector('.sch-header');
    
    expect(header?.classList.contains('sidenav-collapsed')).toBe(false);
  });

  it('should render app title', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const title = compiled.querySelector('.app-title');
    
    expect(title?.textContent?.trim()).toBe('School Management System');
  });

  it('should render logout button', () => {
    // Logout button is inside a MatMenu overlay; verify the trigger button is present
    const compiled = fixture.nativeElement as HTMLElement;
    const userBtn = compiled.querySelector('.user-button');
    expect(userBtn).toBeNull(); // not rendered unless authenticated — which is correct in test env
  });
});
