import { Component, Inject, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { Student } from '../../../../../sch/interfaces/student';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { StudentApi } from '../../../services/student-api';
import { APP_CONFIG } from '../../../../../injection-tokens/app-config.token';
import { AppConfig } from '../../../../../interfaces/app-config';
import { ImageApi } from '../../../../../sch/services/image-api';
import { CommonModule, formatDate } from '@angular/common';
import { Notification } from '../../../../../services/notification';
import { SecureImage } from '../../../../../pipes/secure-image';
import { Auth } from '../../../../../services/auth';
import { IdentityUserApi } from '../../../../../sch/services/identity-user-api';
import { UserLookup } from '../../../../../sch/interfaces/user-lookup';
import { Observable, forkJoin, of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { EditBase } from '../../../../../directives/edit-base.directive';
import { CourseApi } from '../../../../../sch/services/course-api';
import { Course } from '../../../../../sch/interfaces/course';
import { StudentCourseMap } from '../../../../interfaces/student-course-map';

type CourseFormControls = {
  courseId: FormControl<number | null>;
  courseName: FormControl<string | null>;
  enrollmentDate: FormControl<Date>;
};

type CourseFormValue = Pick<StudentCourseMap, 'courseId' | 'courseName' | 'enrollmentDate'>;

type StudentFormControls = {
  id: FormControl<number>;
  firstName: FormControl<string>;
  lastName: FormControl<string | null>;
  email: FormControl<string | null>;
  phoneNumber: FormControl<string | null>;
  ssn: FormControl<string | null>;
  startDate: FormControl<string | null>;
  userId: FormControl<number | null>;
  courses: FormArray<FormGroup<CourseFormControls>>;
};

@Component({
  selector: 'sch-student-detail-page',
  imports: [RouterOutlet, CommonModule, ReactiveFormsModule, SecureImage],
  templateUrl: './student-detail-page.html',
  styleUrl: './student-detail-page.scss',
})
export class StudentDetailPage extends EditBase implements OnInit {
  protected readonly auth = inject(Auth);
  protected readonly studentId = signal(0);
  protected readonly student = signal<Student | null>(null);
  protected readonly isStudentLoading = signal(false);
  protected readonly isStudentSaving = signal(false);
  protected readonly isUploadingImage = signal(false);
  protected readonly isDeletingImage = signal(false);
  protected readonly isImageChanged = signal(false);
  protected readonly profileImage = signal('');
  protected readonly availableUsers = signal<UserLookup[]>([]);
  protected readonly isUsersLoading = signal(false);
  protected readonly availableCourses = signal<Course[]>([]);

  protected studentForm: FormGroup<StudentFormControls>;
  private profileImageFile: File | null = null;

  constructor(
    private readonly _avRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly fb: FormBuilder,
    private readonly studentApi: StudentApi,
    private readonly imageApi: ImageApi,
    private readonly identityUserApi: IdentityUserApi,
    private readonly courseApi: CourseApi,
    @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
    private readonly notification: Notification
  ) {
    super();
    this.studentForm = new FormGroup<StudentFormControls>({
      id: new FormControl<number>(0, { nonNullable: true }),
      firstName: new FormControl<string>('', { nonNullable: true, validators: [Validators.required, Validators.minLength(2)] }),
      lastName: new FormControl<string | null>(null, { validators: Validators.minLength(2) }),
      email: new FormControl<string | null>(null, { validators: Validators.email }),
      phoneNumber: new FormControl<string | null>(null, { validators: Validators.pattern('^[0-9]{10}$') }),
      ssn: new FormControl<string | null>(null, { validators: [Validators.required, Validators.minLength(2)] }),
      startDate: new FormControl<string | null>(null),
      userId: new FormControl<number | null>(null),
      courses: new FormArray<FormGroup<CourseFormControls>>([]),
    });
    // Course row structure — controls and validators defined once here
    // Data is applied separately via setValue in setFormData

    // Disable userId for non-admin users
    if (!this.auth.isAdmin()) {
      this.studentForm.get('userId')!.disable();
    }
  }

  ngOnInit(): void {
    this._avRoute.params.subscribe((params) => {
      this.studentId.set(+params['id'] || 0);
      this.loadData();
    });

    this.coursesArray.valueChanges.subscribe(() => {
      this.resetCourseNames();
    });
  }

  private reset(): void {
    this.student.set(null);
    this.profileImageFile = null;
    this.profileImage.set('');
    this.isImageChanged.set(false);
    this.coursesArray.clear();
    this.studentForm.reset({
      id: 0,
      firstName: '',
      lastName: null,
      email: null,
      phoneNumber: null,
      ssn: null,
      startDate: null,
      userId: null,
    });
  }

  private setStudent(): Observable<Student | null> {
    this.reset();
    if (!this.studentId()) {
      this.setFormData();
      return of(null);
    }
    this.isStudentLoading.set(true);
    return this.studentApi.getStudent(this.studentId()).pipe(
      tap((student) => {
        if (student) {
          if (student.startDate) {
            student.startDate = new Date(student.startDate);
          }
          this.student.set(student);
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
      finalize(() => this.isStudentLoading.set(false))
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
      student: this.setStudent(),
      users: this.getBasicUsers(),
      courses: this.courseApi.getCourses().pipe(catchError(() => of([]))),
    }).subscribe({
      next: ({ student, users, courses }) => {
        this.availableCourses.set(courses);
        const currentUser: UserLookup[] = student?.user ? [student.user] : [];
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
    const student = this.student();
    if (student) {
      let date: string | null = null;
      if (student.startDate) {
        date = formatDate(student.startDate, 'yyyy-MM-dd', 'en');
      }

      const courses = (student.courses ?? []).map((c) => ({
        courseId: c.courseId,
        courseName: c.courseName ?? null,
        enrollmentDate: c.enrollmentDate ? new Date(c.enrollmentDate) : new Date(),
      }));

      this.coursesArray.clear();
      courses.forEach(() => this.coursesArray.push(this.createCourseGroup()));

      this.studentForm.setValue({
        id: student.id,
        firstName: student.firstName,
        lastName: student.lastName,
        email: student.email,
        phoneNumber: student.phoneNumber,
        ssn: student.ssn,
        startDate: date,
        userId: student.userId ?? null,
        courses,
      });
    }
  }

  protected onSubmit() {
    if (this.studentForm.valid) {
      if (this.isImageChanged() && this.profileImageFile) {
        this.uploadImage();
      } else {
        let image: string | null = null;

        const student = this.student();
        if (student) {
          image = student.image;
        }

        this.saveStudent(image);
      }
    } else {
      this.validateAllFormFields(this.studentForm);
    }
  }

  private uploadImage(): void {
    this.isUploadingImage.set(true);

    this.imageApi
      .uploadStudentProfile(this.profileImageFile!)
      .subscribe((data) => {
        this.saveStudent(data.filename);
      })
      .add(() => {
        this.isUploadingImage.set(false);
      });
  }

  private deleteImage(fileName: string): void {
    this.isDeletingImage.set(true);
    this.imageApi
      .deleteStudentProfile(fileName)
      .subscribe()
      .add(() => {
        this.isDeletingImage.set(false);
      });
  }

  private saveStudent(image: string | null): void {
    const student: Student = {
      id: this.studentForm.value.id ?? 0,
      firstName: this.studentForm.value.firstName!,
      lastName: this.studentForm.value.lastName ?? '',
      email: this.studentForm.value.email ?? '',
      phoneNumber: this.studentForm.value.phoneNumber ?? '',
      ssn: this.studentForm.value.ssn ?? '',
      startDate: new Date(this.studentForm.value.startDate ?? ''),
      image: image,
      isActive: true,
      userId: this.auth.isAdmin() ? (this.studentForm.value.userId ?? null) : undefined,
      rowVersion: this.student()?.rowVersion, // Include rowVersion for concurrency check
      courses: this.coursesArray.controls
        .filter((g) => g.value.courseId !== null && g.value.courseId !== undefined)
        .map((g) => ({
          studentId: this.studentForm.value.id!,
          courseId: g.value.courseId!,
          enrollmentDate: g.value.enrollmentDate!,
          studentFirstName: null,
          studentLastName: null,
          courseName: null,
        })),
    };

    if (student.id > 0) {
      this.isStudentSaving.set(true);
      this.studentApi
        .updateStudent(student)
        .subscribe({
          next: () => {
            const currentStudent = this.student();
            if (this.isImageChanged() && currentStudent?.image) {
              this.deleteImage(currentStudent.image);
            }
            this.loadData();
            this.notification.success('Student updated successfully');
          },
          error: (error) => {
            // Generic error message - specific errors handled by interceptor
            this.notification.error('Failed to update student');
          },
        })
        .add(() => {
          this.isStudentSaving.set(false);
        });
    } else {
      this.isStudentSaving.set(true);
      this.studentApi
        .insertStudent(student)
        .subscribe({
          next: (id) => {
            this.reset();
            this.router.navigate(['../', id], { relativeTo: this._avRoute });
            this.notification.success('Student added successfully');
          },
          error: (error) => {
            this.notification.error('Failed to add student');
          },
        })
        .add(() => {
          this.isStudentSaving.set(false);
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

  protected onFileChange(event: any) {
    if (event.target.files.length) {
      const file: File = event.target.files[0];
      const mimeType = file.type;
      const fileName = file.name;
      const fileSize = file.size;
      const supportedImageTypes = [
        '.jpg',
        '.png',
        '.jpeg',
        '.JPG',
        '.PNG',
        '.JPEG',
      ];
      const type = supportedImageTypes.find((t) => fileName.endsWith(t));
      const mimeTypePattern = /image\/*/;
      const bannerImageFileMaxSize = 2097152;
      if (mimeTypePattern.exec(mimeType) === null || !type) {
        this.notification.error('File type should be JPG, JPEG or PNG');
      } else if (fileSize > bannerImageFileMaxSize) {
        this.notification.error('Please upload a file that not exceed 2MB');
      } else {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onloadend = () => {
          this.profileImage.set(reader.result as string);
        };
        this.isImageChanged.set(true);
        this.profileImageFile = file;
      }
    }
  }

  public hasUnsavedChanges(): boolean {
    return this.studentForm.dirty || this.isImageChanged();
  }

  private resetCourseNames(): void {
    this.coursesArray.controls.forEach((group) => {
      if (!group.value.courseName) {
        const courseId = group.value.courseId;
        const course = this.availableCourses().find((c) => c.id === courseId);
        group.get('courseName')!.setValue(course ? course.name : null, { emitEvent: false });
      }

    });
  }

  private createCourseGroup(): FormGroup<CourseFormControls> {
    return this.fb.group({
      courseId: new FormControl<number | null>(null, { validators: Validators.required, nonNullable: false }),
      courseName: new FormControl<string | null>(null),
      enrollmentDate: new FormControl<Date>(new Date(), { nonNullable: true }),
    });
  }

  protected addCourseRow(): void {
    this.coursesArray.push(this.createCourseGroup());
    this.studentForm.markAsDirty();
  }

  protected removeCourseRow(index: number): void {
    this.coursesArray.removeAt(index);
    this.studentForm.markAsDirty();
  }

  protected get coursesArray(): FormArray<FormGroup<CourseFormControls>> {
    return this.studentForm.get('courses') as FormArray<FormGroup<CourseFormControls>>;
  }

  protected getAvailableCoursesForRow(index: number): Course[] {
    const usedIds = new Set(
      this.coursesArray.controls
        .filter((_, i) => i !== index)
        .map((g) => g.value.courseId)
        .filter(Boolean)
    );
    return this.availableCourses().filter((c) => !usedIds.has(c.id));
  }

  protected get formControls(): StudentFormControls {
    return this.studentForm.controls;
  }
}
