import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { StudentDetailPage } from './student-detail-page';
import { StudentApi } from '../../../services/student-api';
import { ImageApi } from '../../../../../sch/services/image-api';
import { APP_CONFIG } from '../../../../../injection-tokens/app-config.token';
import { Notification } from '../../../../../services/notification';

describe('StudentDetailPage', () => {
  let component: StudentDetailPage;
  let fixture: ComponentFixture<StudentDetailPage>;

  let studentApiMock: jasmine.SpyObj<StudentApi>;
  let imageApiMock: jasmine.SpyObj<ImageApi>;
  let routerMock: jasmine.SpyObj<Router>;
  let notificationMock: jasmine.SpyObj<Notification>;

  async function setupWithParams(params: any) {
    studentApiMock = jasmine.createSpyObj('StudentApi', [
      'getStudent',
      'insertStudent',
      'updateStudent',
    ]);
    imageApiMock = jasmine.createSpyObj('ImageApi', [
      'uploadStudentProfile',
      'deleteStudentProfile',
    ]);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    notificationMock = jasmine.createSpyObj('Notification', [
      'success',
      'error',
    ]);

    await TestBed.configureTestingModule({
      imports: [StudentDetailPage],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: { params: of(params), snapshot: {} },
        },
        { provide: StudentApi, useValue: studentApiMock },
        { provide: ImageApi, useValue: imageApiMock },
        { provide: Router, useValue: routerMock },
        { provide: Notification, useValue: notificationMock },
        { provide: APP_CONFIG, useValue: { apiUrl: 'http://test' } },
      ],
    }).compileComponents();
  }

  it('should create', async () => {
    await setupWithParams({ id: 0 });
    fixture = TestBed.createComponent(StudentDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('loads student when id > 0 and sets form', async () => {
    await setupWithParams({ id: 4 });
    studentApiMock.getStudent.and.returnValue(
      of({
        id: 4,
        firstName: 'John',
        lastName: null,
        email: null,
        phoneNumber: null,
        ssn: '11',
        startDate: null,
      } as any)
    );

    fixture = TestBed.createComponent(StudentDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(studentApiMock.getStudent).toHaveBeenCalledWith(4);
    expect((component as any).studentForm.value.id).toBe(4);
  });

  it('navigates to create on 404', async () => {
    await setupWithParams({ id: 8 });
    studentApiMock.getStudent.and.returnValue(
      throwError(() => ({ status: 404 }))
    );

    fixture = TestBed.createComponent(StudentDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(routerMock.navigate).toHaveBeenCalled();
  });

  it('updates existing student on submit', async () => {
    await setupWithParams({ id: 2 });
    studentApiMock.getStudent.and.returnValue(
      of({
        id: 2,
        firstName: 'Old',
        lastName: null,
        email: null,
        phoneNumber: null,
        ssn: '11',
        startDate: null,
      } as any)
    );
    studentApiMock.updateStudent.and.returnValue(of(void 0));

    fixture = TestBed.createComponent(StudentDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    (component as any).studentForm.patchValue({
      id: 2,
      firstName: 'New',
      ssn: '22',
      email: null,
      phoneNumber: null,
      lastName: null,
      startDate: null,
    });
    (component as any).onSubmit();

    expect(studentApiMock.updateStudent).toHaveBeenCalled();
    expect(notificationMock.success).toHaveBeenCalled();
  });

  it('inserts new student on submit', async () => {
    await setupWithParams({ id: 0 });
    studentApiMock.insertStudent.and.returnValue(of(33));

    fixture = TestBed.createComponent(StudentDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    (component as any).studentForm.patchValue({
      id: 0,
      firstName: 'AA',
      ssn: '11',
      email: null,
      phoneNumber: null,
      lastName: null,
      startDate: null,
    });
    (component as any).onSubmit();

    expect(studentApiMock.insertStudent).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalled();
    expect(notificationMock.success).toHaveBeenCalled();
  });

  it('uploads image before save when image changed', async () => {
    await setupWithParams({ id: 0 });
    imageApiMock.uploadStudentProfile.and.returnValue(
      of({ filename: 'pic.jpg' })
    );
    studentApiMock.insertStudent.and.returnValue(of(1));

    fixture = TestBed.createComponent(StudentDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();

    (component as any).profileImageFile = new File(
      [new Blob(['a'])],
      'pic.jpg',
      { type: 'image/jpeg' }
    );
    (component as any).isImageChanged = true;
    (component as any).studentForm.patchValue({
      id: 0,
      firstName: 'AA',
      ssn: '11',
      email: null,
      phoneNumber: null,
      lastName: null,
      startDate: null,
    });

    (component as any).onSubmit();

    expect(imageApiMock.uploadStudentProfile).toHaveBeenCalled();
    expect(studentApiMock.insertStudent).toHaveBeenCalled();
  });
});
