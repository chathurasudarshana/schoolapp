import { Component, OnInit, inject, signal } from '@angular/core';
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
import { Auth } from '../../../../../services/auth';
import { IdentityUserApi } from '../../../../../sch/services/identity-user-api';
import { UserLookup } from '../../../../../sch/interfaces/user-lookup';
import { Observable, forkJoin, of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';


@Component({
  selector: 'sch-teacher-detail-page',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './teacher-detail-page.html',
  styleUrl: './teacher-detail-page.scss'
})
export class TeacherDetailPage implements OnInit, HasUnsavedChanges {
  protected readonly auth = inject(Auth);
  protected readonly teacherId = signal(0);
  protected readonly teacher = signal<Teacher | null>(null);
  protected readonly isTeacherLoading = signal(false);
  protected readonly isTeacherSaving = signal(false);
  protected readonly availableUsers = signal<UserLookup[]>([]);
  protected readonly isUsersLoading = signal(false);

  protected teacherForm: FormGroup;

  constructor(
    private readonly _avRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly fb: FormBuilder,
    private readonly teacherApi: TeacherApi,
    private readonly identityUserApi: IdentityUserApi,
    private readonly notification: Notification
  ) {
    this.teacherForm = this.fb.group({
      id: [0],
      name: ['', [Validators.required, Validators.minLength(2)]],
      userId: [null],
    });

    if (!this.auth.isAdmin()) {
      this.teacherForm.get('userId')!.disable();
    }
  }

  ngOnInit(): void {
    this._avRoute.params.subscribe((params) => {
      this.teacherId.set(+params['id'] || 0);
      this.loadData();
    });
  }

  private reset(): void {
    this.teacher.set(null);
    this.teacherForm.reset({
      id: 0,
      name: '',
      userId: null,
    });
  }

  private setTeacher(): Observable<Teacher | null> {
    this.reset();
    if (!this.teacherId()) {
      this.setFormData();
      return of(null);
    }
    this.isTeacherLoading.set(true);
    return this.teacherApi.getTeacher(this.teacherId()).pipe(
      tap((teacher) => {
        if (teacher) {
          this.teacher.set(teacher);
          this.setFormData();
        } else {
          this.router.navigate(['../', 0], { relativeTo: this._avRoute });
        }
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.router.navigate(['../', 0], { relativeTo: this._avRoute });
        }
        return of(null);
      }),
      finalize(() => this.isTeacherLoading.set(false))
    );
  }

  private getBasicUsers(): Observable<UserLookup[]> {
    if (!this.auth.isAdmin()) {
      return of([]);
    }
    this.isUsersLoading.set(true);
    return this.identityUserApi.getBasicOnlyUsers().pipe(
      catchError(() => of([])),
      finalize(() => this.isUsersLoading.set(false))
    );
  }

  private loadData(): void {
    forkJoin({
      teacher: this.setTeacher(),
      users: this.getBasicUsers(),
    }).subscribe({
      next: ({ teacher, users }) => {
        const currentUser: UserLookup[] = teacher?.user ? [teacher.user] : [];
        const merged = [
          ...currentUser,
          ...users.filter((u) => !currentUser.some((c) => c.id === u.id)),
        ];
        this.availableUsers.set(merged);
      },
      error: () => this.availableUsers.set([]),
    });
  }

  private setFormData(): void {
    const teacher = this.teacher();
    if (teacher) {
      this.teacherForm.setValue({
        id: teacher.id,
        name: teacher.name,
        userId: teacher.userId ?? null,
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
      userId: this.auth.isAdmin() ? (this.teacherForm.value.userId ?? null) : undefined,
      rowVersion: this.teacher()?.rowVersion, // Include rowVersion for concurrency check
    };

    if (teacher.id > 0) {
      this.isTeacherSaving.set(true);
      this.teacherApi
        .updateTeacher(teacher)
        .subscribe({
          next: () => {
            this.loadData();
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
            this.reset();
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
