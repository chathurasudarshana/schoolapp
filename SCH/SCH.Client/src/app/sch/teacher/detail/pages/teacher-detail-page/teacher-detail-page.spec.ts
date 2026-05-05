import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { TeacherDetailPage } from './teacher-detail-page';
import { TeacherApi } from '../../../../../sch/services/teacher-api';
import { APP_CONFIG } from '../../../../../injection-tokens/app-config.token';
import { Notification } from '../../../../../services/notification';

describe('TeacherDetailPage', () => {
  let component: TeacherDetailPage;
  let fixture: ComponentFixture<TeacherDetailPage>;

  let teacherApiMock: jasmine.SpyObj<TeacherApi>;
  let routerMock: jasmine.SpyObj<Router>;
  let notificationMock: jasmine.SpyObj<Notification>;

  async function setupWithParams(params: any) {
    teacherApiMock = jasmine.createSpyObj('TeacherApi', [
      'getTeacher',
      'insertTeacher',
      'updateTeacher',
    ]);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    notificationMock = jasmine.createSpyObj('Notification', [
      'success',
      'error',
    ]);

    await TestBed.configureTestingModule({
      imports: [TeacherDetailPage],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: { params: of(params), snapshot: {} },
        },
        { provide: TeacherApi, useValue: teacherApiMock },
        { provide: Router, useValue: routerMock },
        { provide: Notification, useValue: notificationMock },
        { provide: APP_CONFIG, useValue: { apiUrl: 'http://test' } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TeacherDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  it('should create', async () => {
    await setupWithParams({ id: 0 });
    expect(component).toBeTruthy();
  });

  it('loads teacher when id > 0 and sets form values', async () => {
    await setupWithParams({ id: 5 });
    teacherApiMock.getTeacher.and.returnValue(
      of({ id: 5, name: 'Teacher A' } as any)
    );

    // Recreate to apply spy behavior before detectChanges
    fixture = TestBed.createComponent(TeacherDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(teacherApiMock.getTeacher).toHaveBeenCalledWith(5);
    expect((component as any).teacherForm.value).toEqual({
      id: 5,
      name: 'Teacher A',
    });
  });

  it('navigates to create when teacher not found', async () => {
    await setupWithParams({ id: 3 });
    teacherApiMock.getTeacher.and.returnValue(of(null as any));

    fixture = TestBed.createComponent(TeacherDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(routerMock.navigate).toHaveBeenCalled();
  });

  it('navigates to create on 404', async () => {
    await setupWithParams({ id: 7 });
    teacherApiMock.getTeacher.and.returnValue(
      throwError(() => ({ status: 404 }))
    );

    fixture = TestBed.createComponent(TeacherDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(routerMock.navigate).toHaveBeenCalled();
  });

  it('updates existing teacher on submit', async () => {
    await setupWithParams({ id: 9 });
    teacherApiMock.getTeacher.and.returnValue(of({ id: 9, name: 'Old' } as any));
    teacherApiMock.updateTeacher.and.returnValue(of(void 0));

    fixture = TestBed.createComponent(TeacherDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    (component as any).teacherForm.setValue({ id: 9, name: 'New' });
    (component as any).onSubmit();

    expect(teacherApiMock.updateTeacher).toHaveBeenCalledWith({
      id: 9,
      name: 'New',
    } as any);
    expect(notificationMock.success).toHaveBeenCalled();
  });

  it('inserts new teacher on submit', async () => {
    await setupWithParams({ id: 0 });
    teacherApiMock.insertTeacher.and.returnValue(of(11));

    (component as any).teacherForm.setValue({ id: 0, name: 'Created' });
    (component as any).onSubmit();

    expect(teacherApiMock.insertTeacher).toHaveBeenCalledWith({
      id: 0,
      name: 'Created',
    } as any);
    expect(routerMock.navigate).toHaveBeenCalled();
    expect(notificationMock.success).toHaveBeenCalled();
  });
});
