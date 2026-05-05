import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Teacher } from '../../../../../sch/interfaces/teacher';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TeacherApi } from '../../../../../sch/services/teacher-api';
import { CommonModule } from '@angular/common';
import { Notification } from '../../../../../services/notification';
import { HasUnsavedChanges } from '../../../../../interfaces/has-unsaved-changes';


@Component({
  selector: 'sch-teacher-detail-page',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './teacher-detail-page.html',
  styleUrl: './teacher-detail-page.scss'
})
export class TeacherDetailPage implements OnInit, HasUnsavedChanges {
  protected readonly teacherId = signal(0);

  protected readonly teacher = signal<Teacher | null>(null);

  protected readonly isTeacherLoading = signal(false);
  protected readonly isTeacherSaving = signal(false);

  protected teacherForm: FormGroup;

  constructor(
    private readonly _avRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly fb: FormBuilder,
    private readonly teacherApi: TeacherApi,
    private readonly notification: Notification
  ) {
    this.teacherForm = this.fb.group({
      id: [0],
      name: ['', [Validators.required, Validators.minLength(2)]],
    });
  }

  ngOnInit(): void {
    this._avRoute.params.subscribe((params) => {
      this.teacherId.set(+params['id'] || 0);

      this.setTeacher();
    });
  }

  private reset(): void {
    this.teacher.set(null);
    this.teacherForm.reset({
      id: 0,
      name: '',
    });
  }

  private setTeacher(): void {
    this.reset();
    if (this.teacherId()) {
      this.isTeacherLoading.set(true);
      this.teacherApi
        .getTeacher(this.teacherId())
        .subscribe({
          next: (teacher) => {
            if (teacher) {
              this.teacher.set(teacher);

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
          this.isTeacherLoading.set(false);
        });
    } else {
      this.setFormData();
    }
  }

  private setFormData(): void {
    const teacher = this.teacher();
    if (teacher) {
      this.teacherForm.setValue({
        id: teacher.id,
        name: teacher.name,
      });
    }
  }

  protected onSubmit() {
    if (this.teacherForm.valid) {

      this.saveTeacher();
    } else {
      this.validateAllFormFields(this.teacherForm);
    }
  }




  private saveTeacher(): void {
    const teacher: Teacher = {
      id: this.teacherForm.value.id,
      name: this.teacherForm.value.name,
      rowVersion: this.teacher()?.rowVersion, // Include rowVersion for concurrency check
    };

    if (teacher.id > 0) {
      this.isTeacherSaving.set(true);
      this.teacherApi
        .updateTeacher(teacher)
        .subscribe({
          next: () => {
            this.setTeacher();
            this.notification.success('Teacher updated successfully');
          },
          error: (error) => {
            // Generic error message - specific errors handled by interceptor
            this.notification.error('Failed to update teacher');
          },
        })
        .add(() => {
          this.isTeacherSaving.set(false);
        });
    } else {
      this.isTeacherSaving.set(true);
      this.teacherApi
        .insertTeacher(teacher)
        .subscribe({
          next: (id) => {
            this.router.navigate(['../', id], { relativeTo: this._avRoute });
            this.notification.success('Teacher added successfully');
          },
          error: (error) => {
            this.notification.error('Failed to add teacher');
          },
        })
        .add(() => {
          this.isTeacherSaving.set(false);
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

    return this.teacherForm.dirty;
  }

  protected get formControls() {
    return this.teacherForm.controls;
  }
}
