
import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { StudentCourse } from '../../../selectors/student-course/student-course';


@Component({
  selector: 'sch-student-course-page',
  imports: [StudentCourse],
  templateUrl: './student-course-page.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './student-course-page.scss',
})
export class StudentCoursePage implements OnInit {
  protected studentId = 0;

  constructor(
    private readonly _avRoute: ActivatedRoute,
  ) {}

  ngOnInit(): void {
    this._avRoute.params.subscribe((params) => {
      this.studentId = +params['id'] || 0;
      
    });

  }

}
