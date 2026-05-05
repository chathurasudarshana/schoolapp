import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

import { StudentCoursePage } from './student-course-page';

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
            parent: { params: of({ id: 1 }) },
            params: of({ id: 1 }),
            snapshot: {},
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
