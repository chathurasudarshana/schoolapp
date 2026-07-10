import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { DashboardPage } from './dashboard-page';
import { DashboardApi } from '../../../services/dashboard-api';
import { APP_CONFIG } from '../../../../injection-tokens/app-config.token';

describe('DashboardPage', () => {
  let component: DashboardPage;
  let fixture: ComponentFixture<DashboardPage>;
  let dashboardApiMock: jasmine.SpyObj<DashboardApi>;

  beforeEach(async () => {
    dashboardApiMock = jasmine.createSpyObj('DashboardApi', ['getCourseStudentCount']);
    dashboardApiMock.getCourseStudentCount.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [ DashboardPage ],
      providers: [
        { provide: DashboardApi, useValue: dashboardApiMock },
        { provide: APP_CONFIG, useValue: { apiUrl: '' } },
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
