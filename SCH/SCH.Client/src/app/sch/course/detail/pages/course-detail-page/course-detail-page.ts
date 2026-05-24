import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Course } from '../../../../../sch/interfaces/course';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CourseApi } from '../../../../../sch/services/course-api';
import { CommonModule } from '@angular/common';
import { Notification } from '../../../../../services/notification';
import { HasUnsavedChanges } from '../../../../../interfaces/has-unsaved-changes';


@Component({
  selector: 'sch-course-detail-page',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './course-detail-page.html',
  styleUrl: './course-detail-page.scss'
})
export class CourseDetailPage implements OnInit, HasUnsavedChanges {
  protected readonly courseId = signal(0);

  protected readonly course = signal<Course | null>(null);

  protected readonly isCourseLoading = signal(false);
  protected readonly isCourseSaving = signal(false);

  protected courseForm: FormGroup;

  constructor(
    private readonly _avRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly fb: FormBuilder,
    private readonly courseApi: CourseApi,
    private readonly notification: Notification
  ) {
    this.courseForm = this.fb.group({
      id: [0],
      name: ['', [Validators.required, Validators.minLength(2)]],
    });
  }

  ngOnInit(): void {
    this._avRoute.params.subscribe((params) => {
      this.courseId.set(+params['id'] || 0);

      this.setCourse();
    });
  }

  private reset(): void {
    this.course.set(null);
    this.courseForm.reset({
      id: 0,
      name: '',
    });
  }

  private setCourse(): void {
    this.reset();
    if (this.courseId()) {
      this.isCourseLoading.set(true);
      this.courseApi
        .getCourse(this.courseId())
        .subscribe({
          next: (course) => {
            if (course) {
              this.course.set(course);

              this.setFormData();
            } else {
              this.router.navigate(['../', 0], { relativeTo: this._avRoute });
            }
          },
          error: (error) => {
            if (error.status === 404) {
              this.router.navigate(['../', 0], { relativeTo: this._avRoute });
            }
          },
        })
        .add(() => {
          this.isCourseLoading.set(false);
        });
    } else {
      this.setFormData();
    }
  }

  private setFormData(): void {
    const course = this.course();
    if (course) {
      this.courseForm.setValue({
        id: course.id,
        name: course.name,
      });
    }
  }

  protected onSubmit() {
    if (this.courseForm.valid) {

      this.saveCourse();
    } else {
      this.validateAllFormFields(this.courseForm);
    }
  }




  private saveCourse(): void {

    const course: Course = {
      id: this.courseForm.value.id,
      name: this.courseForm.value.name,
      rowVersion: this.course()?.rowVersion, // Include rowVersion for concurrency check
    };

    if (course.id > 0) {
      this.isCourseSaving.set(true);
      this.courseApi
        .updateCourse(course)
        .subscribe({
          next: () => {
            this.setCourse();
            this.notification.success('Course updated successfully');
          },
          error: (error) => {
            // Generic error message - specific errors handled by interceptor
            this.notification.error('Failed to update course');
          },
        })
        .add(() => {
          this.isCourseSaving.set(false);
        });
    } else {
      this.isCourseSaving.set(true);
      this.courseApi
        .insertCourse(course)
        .subscribe({
          next: (id) => {
            this.reset();
            this.router.navigate(['../', id], { relativeTo: this._avRoute });
            this.notification.success('Course added successfully');
          },
          error: (error) => {
            this.notification.error('Failed to add course');
          },
        })
        .add(() => {
          this.isCourseSaving.set(false);
        });
    }
  }

  public onBack(): void {
    this.router.navigate(['../../list'], { relativeTo: this._avRoute });
  }

  private validateAllFormFields(formGroup: FormGroup) {
    for (const field of Object.keys(formGroup.controls)) {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsTouched({ onlySelf: true });
      } else if (control instanceof FormGroup) {
        this.validateAllFormFields(control);
      }
    }
  }

  public hasUnsavedChanges(): boolean {

    return this.courseForm.dirty;
  }

  protected get formControls() {
    return this.courseForm.controls;
  }
}
