import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { SCHPage } from './schpage';
import { SidenavService } from '../../services/sidenav.service';
import { APP_CONFIG } from '../../../injection-tokens/app-config.token';

describe('SCHPage', () => {
  let component: SCHPage;
  let fixture: ComponentFixture<SCHPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SCHPage],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: APP_CONFIG, useValue: { apiUrl: '' } },
        SidenavService
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SCHPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
