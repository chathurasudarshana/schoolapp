import { Component, Inject, input, OnInit, signal } from '@angular/core';
import { StudentCourseMap } from '../../../../sch/interfaces/student-course-map';
import {
  AllCommunityModule,
  CellClickedEvent,
  ColDef,
  ModuleRegistry,
} from 'ag-grid-community';
import { StudentApi } from '../../services/student-api';
import { AgGridAngular } from 'ag-grid-angular';
import { Course } from '../../../../sch/interfaces/course';
import { CourseApi } from '../../../../sch/services/course-api';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { concat, forkJoin } from 'rxjs';
import { ConfirmDialog } from '../../../../selectors/confirm-dialog/confirm-dialog';
import { MatDialog } from '@angular/material/dialog';
import { Notification } from '../../../../services/notification';

ModuleRegistry.registerModules([AllCommunityModule]);

@Component({
  selector: 'sch-student-course',
  imports: [CommonModule, FormsModule, AgGridAngular],
  templateUrl: './student-course.html',
  styleUrl: './student-course.scss'
})
export class StudentCourse implements OnInit {
  studentId = input.required<number>();

  protected readonly columnDefs: ColDef<
    StudentCourseMap,
    number | string | Date | boolean | null
  >[] = [
    {
      headerName: 'StudentId',
      field: 'studentId',
      sortable: true,
      filter: true,
    },
    {
      headerName: 'CourseId',
      field: 'courseId',
      sortable: true,
      filter: true,
    },
    {
      headerName: 'Enrollment Date',
      field: 'enrollmentDate',
      sortable: true,
      filter: true,
    },
    {
      headerName: 'Student First Name',
      field: 'studentFirstName',
      sortable: true,
      filter: true,
    },
    {
      headerName: 'Student Last Name',
      field: 'studentLastName',
      sortable: true,
      filter: true,
    },
    {
      headerName: 'Course Name',
      field: 'courseName',
      sortable: true,
      filter: true,
    },
    {
      headerName: 'Actions',
      cellRenderer: (params: any) => {
        return `
      <button type="button" class="delete-btn" data-action="delete">Delete</button>
    `;
      },
      width: 200,
      suppressMovable: true,
    },
  ];

  protected readonly rowData = signal<StudentCourseMap[]>([]);
  private readonly courses = signal<Course[]>([]);
  protected readonly notSelectedCourses = signal<Course[]>([]);

  protected selectedCourseId: string | null = null;

  protected readonly dataLoading = signal(false);
  protected readonly gridDataLoading = signal(false);
  protected readonly isDeleting = signal(false);
  protected readonly isAdding = signal(false);
  protected readonly isEditing = signal(false);

  constructor(
    private readonly studentApi: StudentApi,
    private readonly courseApi: CourseApi,
    @Inject(MatDialog) private readonly dialog: MatDialog,
    private readonly notification: Notification
  ) {}

  ngOnInit(): void {
    this.setData();
  }

  private setData(): void {
    this.dataLoading.set(true);
  const courses$ = this.courseApi.getCourses();
  const studentCourses$ = this.studentApi.getCourses(this.studentId());

    forkJoin([courses$, studentCourses$]).subscribe(
      ([courses, studentCourses]) => {
        this.courses.set(courses);
        this.rowData.set(studentCourses);
        this.setNotSelectedCourses();
        this.dataLoading.set(false);
      }
    );
  }

  private resetGridData(): void {
    this.gridDataLoading.set(true);
  this.studentApi.getCourses(this.studentId()).subscribe((courses) => {
      this.rowData.set(courses);
      this.setNotSelectedCourses();
      this.gridDataLoading.set(false);
    });
  }

  protected onCellClicked(event: CellClickedEvent): void {
    if (event.colDef.headerName === 'Actions') {
      const target = event.event!.target as HTMLElement;
      if (target.dataset['action'] === 'delete') {
        this.onDeletes([event.data]);
      }
    }
  }

  protected onAddCourse(): void {
    const courseId = Number(this.selectedCourseId);
    if (courseId) {
      const course: StudentCourseMap = {
        studentId: this.studentId(),
        courseId: courseId,
        enrollmentDate: new Date(),
        studentFirstName: null,
        studentLastName: null,
        courseName: null,
      };

      this.isAdding.set(true);
      this.studentApi
        .insertCourse(this.studentId(), courseId, course)
        .subscribe(() => {
          this.resetGridData();
          this.selectedCourseId = null;
          this.notification.success('Course added successfully');
        })
        .add(() => {
          this.isAdding.set(false);
        });
    }
  }

  private setNotSelectedCourses(): void {
    this.notSelectedCourses.set(this.courses().filter(
      (course) =>
        !this.rowData().some(
          (studentCourse) => studentCourse.courseId === course.id
        )
    ));
  }

  protected onRemoveAll(): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        message: 'Are you sure you want to remove all courses?',
        cancelText: 'Cancel',
        confirmText: 'Delete',
      },
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.onDeletes(this.rowData());
      }
    });
  }

  protected onDeletes(courses: StudentCourseMap[]): void {
    const deleteRequests = courses.map((course) =>
      this.studentApi.deleteCourse(this.studentId(), course.courseId)
    );
    this.isDeleting.set(true);
    concat(...deleteRequests)
      .subscribe({
        complete: () => {
          this.resetGridData();
          this.notification.success('Course deleted successfully');
        },
        error: (err) => {
          this.notification.error('Failed to delete course');
        },
      })
      .add(() => {
        this.isDeleting.set(false);
      });
  }
}
