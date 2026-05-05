import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { StudentCourse } from './student-course';
import { StudentApi } from '../../services/student-api';
import { CourseApi } from '../../../services/course-api';
import { ToastrService } from 'ngx-toastr';

describe('StudentCourse', () => {
  let component: StudentCourse;
  let fixture: ComponentFixture<StudentCourse>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentCourse],
      providers: [
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
    }).compileComponents();

    fixture = TestBed.createComponent(StudentCourse);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('studentId', 1);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
