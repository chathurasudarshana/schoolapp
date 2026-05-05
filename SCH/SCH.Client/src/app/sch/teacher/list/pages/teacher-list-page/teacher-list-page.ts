import { Component, Inject, signal } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import {
  AllCommunityModule,
  ModuleRegistry,
  ColDef,
  GridReadyEvent,
  CellClickedEvent,
} from 'ag-grid-community';
import { Teacher } from '../../../../../sch/interfaces/teacher';
import { ActivatedRoute, Router } from '@angular/router';
import { concat, Observable } from 'rxjs';
import { Notification } from '../../../../../services/notification';
import { ConfirmDialog } from '../../../../../selectors/confirm-dialog/confirm-dialog';
import { MatDialog } from '@angular/material/dialog';
import { TeacherApi } from '../../../../../sch/services/teacher-api';


ModuleRegistry.registerModules([AllCommunityModule]);

@Component({
  selector: 'sch-teacher-list-page',
  imports: [AgGridAngular],
  templateUrl: './teacher-list-page.html',
  styleUrl: './teacher-list-page.scss'
})
export class TeacherListPage {

  protected readonly columnDefs: ColDef<
    Teacher,
    number | string | Date | boolean | null
  >[] = [
      {
        headerName: 'ID',
        field: 'id',
        sortable: true,
        filter: true,
      },
      {
        headerName: 'Name',
        field: 'name',
        sortable: true,
        filter: true,
      },
      {
        headerName: 'Actions',
        cellRenderer: (params: any) => {
          return `
      <button type="button" class="edit-btn" data-action="edit">Edit</button>
      <button type="button" class="delete-btn" data-action="delete">Delete</button>
    `;
        },
        width: 200,
        suppressMovable: true,
      },
    ];

  protected readonly rowData = signal<Teacher[]>([]);

  protected readonly gridDataLoading = signal(false);
  protected readonly isDeleting = signal(false);


  constructor(
    private readonly router: Router,
    private readonly _avRoute: ActivatedRoute,
    private readonly teacherApi: TeacherApi,
    private readonly notification: Notification,
    @Inject(MatDialog) private readonly dialog: MatDialog
  ) { }


  protected onGridReady(params: GridReadyEvent) {
    this.setGridData();
  }

  private setGridData() {
    this.gridDataLoading.set(true);

    this.teacherApi
      .getTeachers()
      .subscribe((data) => {
        if (data?.length) {

          this.rowData.set(data);
        } else {
          this.rowData.set([]);
        }
      })
      .add(() => {
        this.gridDataLoading.set(false);
      });
  }

  protected onCellClicked(event: CellClickedEvent): void {
    if (event.colDef.headerName === 'Actions') {
      const target = event.event!.target as HTMLElement;
      if (target.dataset['action'] === 'edit') {
        this.onEdit(event.data);
      } else if (target.dataset['action'] === 'delete') {
        this.onDeletes([event.data]);
      }
    }
  }

  private onEdit(teacher: Teacher): void {
    this.router.navigate([`../detail/${teacher.id}`], {
      relativeTo: this._avRoute,
    });
  }

  protected onRemoveAll(): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        message: 'Are you sure you want to remove all teachers?',
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

  private onDeletes(teachers: Teacher[]): void {
    this.isDeleting.set(true);

    const teachersDeleteCalls: Observable<void>[] = [];

    for (const teacher of teachers) {
      const teachersDeleteCall = this.teacherApi.deleteTeacher(teacher.id);
      teachersDeleteCalls.push(teachersDeleteCall);
    }

    concat(...teachersDeleteCalls).subscribe({
      complete: () => {
        this.setGridData();
        this.notification.success('Teacher deleted successfully');
        this.isDeleting.set(false);
      },
      error: (err) => {
        this.notification.error('Failed to delete teacher');
        this.isDeleting.set(false);
      }
    });

  }

  protected onAdd(): void {
    this.router.navigate(['../detail/0'], { relativeTo: this._avRoute });
  }




}
