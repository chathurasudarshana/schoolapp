import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { of } from 'rxjs';

import { StudentListPage } from './student-list-page';
import { StudentApi } from '../../../services/student-api';
import { APP_CONFIG } from '../../../../../../app/injection-tokens/app-config.token';
import { ToastrService } from 'ngx-toastr';
import { ImageApi } from '../../../../../sch/services/image-api';

describe('StudentListPage', () => {
  let component: StudentListPage;
  let fixture: ComponentFixture<StudentListPage>;

  let studentApiMock: jasmine.SpyObj<StudentApi>;
  let imageApiMock: jasmine.SpyObj<ImageApi>;
  let routerMock: jasmine.SpyObj<Router>;
  let toastrMock: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {
    studentApiMock = jasmine.createSpyObj('StudentApi', [
      'getStudents',
      'deleteStudent',
    ]);
    imageApiMock = jasmine.createSpyObj('ImageApi', ['deleteStudentProfile']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    toastrMock = jasmine.createSpyObj('ToastrService', [
      'success',
      'error',
      'warning',
      'info',
    ]);

    await TestBed.configureTestingModule({
      imports: [StudentListPage],
      providers: [
        { provide: ActivatedRoute, useValue: { params: of({}), snapshot: { queryParams: {} } } },
        { provide: StudentApi, useValue: studentApiMock },
        { provide: APP_CONFIG, useValue: { apiUrl: 'http://test' } },
        { provide: ToastrService, useValue: toastrMock },
        { provide: ImageApi, useValue: imageApiMock },
        { provide: Router, useValue: routerMock },
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(StudentListPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('sets up server-side datasource on grid ready', () => {
    const gridApiMock = jasmine.createSpyObj('GridApi', ['setGridOption', 'paginationGetPageSize']);
    gridApiMock.paginationGetPageSize.and.returnValue(10);

    (component as any).onGridReady({ api: gridApiMock } as any);

    expect(gridApiMock.setGridOption).toHaveBeenCalledWith(
      'serverSideDatasource', jasmine.any(Object)
    );
  });

  it('onAdd navigates to create student', () => {
    (component as any).onAdd();
    expect(routerMock.navigate).toHaveBeenCalled();
  });
});
