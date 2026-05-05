import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

import { StudentCoursePage } from './student-course-page';
import { StudentApi } from '../../../services/student-api';
import { CourseApi } from '../../../../services/course-api';
import { ToastrService } from 'ngx-toastr';

describe('StudentCoursePage', () => {
  let component: StudentCoursePage;
  let fixture: ComponentFixture<StudentCoursePage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentCoursePage],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            params: of({ id: 1 }),
            snapshot: {},
            parent: { params: of({ id: 1 }) },
          },
        },
        {
          provide: StudentApi,
          useValue: {
            getCourses: () => of([]),
            insertCourse: () => of(void 0),
            deleteCourse: () => of(void 0),
          },
        },
        { provide: CourseApi, useValue: { getCourses: () => of([]) } },
        {
          provide: ToastrService,
          useValue: {
            success: () => {},
            error: () => {},
            warning: () => {},
            info: () => {},
          },
        },
      ],
    });

    TestBed.overrideComponent(StudentCoursePage, { set: { template: '' } });

    await TestBed.compileComponents();

    fixture = TestBed.createComponent(StudentCoursePage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
