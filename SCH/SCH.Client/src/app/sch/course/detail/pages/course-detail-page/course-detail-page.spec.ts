import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { CourseDetailPage } from './course-detail-page';
import { CourseApi } from '../../../../../sch/services/course-api';
import { APP_CONFIG } from '../../../../../injection-tokens/app-config.token';
import { Notification } from '../../../../../services/notification';

describe('CourseDetailPage', () => {
  let component: CourseDetailPage;
  let fixture: ComponentFixture<CourseDetailPage>;

  let courseApiMock: jasmine.SpyObj<CourseApi>;
  let routerMock: jasmine.SpyObj<Router>;
  let notificationMock: jasmine.SpyObj<Notification>;

  async function setupWithParams(params: any) {
    courseApiMock = jasmine.createSpyObj('CourseApi', [
      'getCourse',
      'insertCourse',
      'updateCourse',
    ]);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    notificationMock = jasmine.createSpyObj('Notification', [
      'success',
      'error',
    ]);

    await TestBed.configureTestingModule({
      imports: [CourseDetailPage],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: { params: of(params), snapshot: {} },
        },
        { provide: CourseApi, useValue: courseApiMock },
        { provide: Router, useValue: routerMock },
        { provide: Notification, useValue: notificationMock },
        { provide: APP_CONFIG, useValue: { apiUrl: 'http://test' } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CourseDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  it('should create', async () => {
    await setupWithParams({ id: 0 });
    expect(component).toBeTruthy();
  });

  it('loads course when id > 0 and sets form values', async () => {
    await setupWithParams({ id: 5 });
    courseApiMock.getCourse.and.returnValue(
      of({ id: 5, name: 'Course A' } as any)
    );

    // Recreate to apply spy behavior before detectChanges
    fixture = TestBed.createComponent(CourseDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(courseApiMock.getCourse).toHaveBeenCalledWith(5);
    expect((component as any).courseForm.value).toEqual({
      id: 5,
      name: 'Course A',
    });
  });

  it('navigates to create when course not found', async () => {
    await setupWithParams({ id: 3 });
    courseApiMock.getCourse.and.returnValue(of(null as any));

    fixture = TestBed.createComponent(CourseDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(routerMock.navigate).toHaveBeenCalled();
  });

  it('navigates to create on 404', async () => {
    await setupWithParams({ id: 7 });
    courseApiMock.getCourse.and.returnValue(
      throwError(() => ({ status: 404 }))
    );

    fixture = TestBed.createComponent(CourseDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(routerMock.navigate).toHaveBeenCalled();
  });

  it('updates existing course on submit', async () => {
    await setupWithParams({ id: 9 });
    courseApiMock.getCourse.and.returnValue(of({ id: 9, name: 'Old' } as any));
    courseApiMock.updateCourse.and.returnValue(of(void 0));

    fixture = TestBed.createComponent(CourseDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    (component as any).courseForm.setValue({ id: 9, name: 'New' });
    (component as any).onSubmit();

    expect(courseApiMock.updateCourse).toHaveBeenCalledWith({
      id: 9,
      name: 'New',
    } as any);
    expect(notificationMock.success).toHaveBeenCalled();
  });

  it('inserts new course on submit', async () => {
    await setupWithParams({ id: 0 });
    courseApiMock.insertCourse.and.returnValue(of(11));

    (component as any).courseForm.setValue({ id: 0, name: 'Created' });
    (component as any).onSubmit();

    expect(courseApiMock.insertCourse).toHaveBeenCalledWith({
      id: 0,
      name: 'Created',
    } as any);
    expect(routerMock.navigate).toHaveBeenCalled();
    expect(notificationMock.success).toHaveBeenCalled();
  });
});
