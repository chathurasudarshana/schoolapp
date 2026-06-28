import { Component, Inject, inject, signal } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import {
  AllCommunityModule,
  ModuleRegistry,
  ColDef,
  GridReadyEvent,
  CellClickedEvent,
  GridApi,
  GridState,
  PaginationChangedEvent,
} from 'ag-grid-community';
import {
  ServerSideRowModelModule,
  ServerSideRowModelApiModule,
  IServerSideDatasource,
  IServerSideGetRowsParams,
  SetFilterModule,
} from 'ag-grid-enterprise';
import { Student } from '../../../../../sch/interfaces/student';
import { StudentApi } from '../../../services/student-api';
import { ActivatedRoute, Router } from '@angular/router';
import { AppConfig } from '../../../../../interfaces/app-config';
import { APP_CONFIG } from '../../../../../injection-tokens/app-config.token';
import { ImageApi } from '../../../../../sch/services/image-api';
import { catchError } from 'rxjs/operators';
import { concatMap, from, mergeMap, of } from 'rxjs';
import { Notification } from '../../../../../services/notification';
import { ConfirmDialog } from '../../../../../selectors/confirm-dialog/confirm-dialog';
import { MatDialog } from '@angular/material/dialog';
import { StudentPhotoCell } from '../../../selectors/student-photo-cell/student-photo-cell';
import { StudentGridRequest } from '../../../interfaces/student-grid-request';
import { Auth } from '../../../../../services/auth';
import { Policy } from '../../../../../enums/policy';
import { HasPolicy } from '../../../../../directives/has-policy.directive';
import { HasPolicyData } from '../../../../../interfaces/has-policy-data';

ModuleRegistry.registerModules([AllCommunityModule, ServerSideRowModelModule, ServerSideRowModelApiModule, SetFilterModule]);

@Component({
  selector: 'sch-student-list-page',
  imports: [AgGridAngular, HasPolicy],
  templateUrl: './student-list-page.html',
  styleUrl: './student-list-page.scss',
})
export class StudentListPage {
  protected readonly auth = inject(Auth);
  protected readonly Policy = Policy;
  protected readonly columnDefs: ColDef<
    Student,
    number | string | Date | boolean | null
  >[] = [
    {
      headerName: 'Photo',
      field: 'image',
      width: 80,
      cellRenderer: StudentPhotoCell,
      sortable: false,
      filter: false,
      suppressMovable: true,
    },
    {
      headerName: 'ID',
      field: 'id',
      sortable: true,
      filter: false,
    },
    {
      headerName: 'First Name',
      field: 'firstName',
      sortable: true,
      filter: 'agTextColumnFilter',
    },
    {
      headerName: 'Last Name',
      field: 'lastName',
      sortable: true,
      filter: 'agTextColumnFilter',
    },
    {
      headerName: 'Email',
      field: 'email',
      sortable: true,
      filter: 'agTextColumnFilter',
    },
    {
      headerName: 'Phone Number',
      field: 'phoneNumber',
      sortable: true,
      filter: 'agTextColumnFilter',
    },
    {
      headerName: 'SSN',
      field: 'ssn',
      sortable: true,
      filter: 'agTextColumnFilter',
    },
    {
      headerName: 'Start Date',
      field: 'startDate',
      sortable: true,
      filter: 'agDateColumnFilter',
    },
    {
      headerName: 'Active',
      field: 'isActive',
      sortable: true,
      filter: 'agSetColumnFilter',
      filterParams: {
        values: [true, false],
        valueFormatter: (params: any) => params.value ? 'Active' : 'Inactive',
      },
    },
    {
      headerName: 'Actions',
      cellRenderer: (params: any) => {

        const student: Student = params.data;
        const editPolicyData: HasPolicyData = { studentId: student?.id };
        const canEdit = this.auth.hasPolicy(Policy.EditStudents, editPolicyData);
        const canDelete = this.auth.hasPolicy(Policy.DeleteStudents);

        const editBtn = canEdit
          ? `<button type="button" class="edit-btn" data-action="edit">Edit</button>`
          : '';
        const deleteBtn = canDelete
          ? `<button type="button" class="delete-btn" data-action="delete">Delete</button>`
          : '';
        return `${editBtn}${deleteBtn}`;
      },
      width: 200,
      suppressMovable: true,
    },
  ];

  protected readonly rowData = signal<Student[]>([]);
  protected readonly gridDataLoading = signal(false);
  protected readonly isDeleting = signal(false);


  protected readonly paginationPageSize: number;
  protected readonly paginationPageSizeSelector: number[];
  protected readonly gridInitialState: GridState;
  protected readonly initialCacheBlockSize: number;

  private gridApi!: GridApi;
  private pendingPage = 0;
  private page1HasFakeData = false;

  constructor(
    private readonly router: Router,
    private readonly _avRoute: ActivatedRoute,
    private readonly studentApi: StudentApi,
    @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
    private readonly imageApi: ImageApi,
    private readonly notification: Notification,
    @Inject(MatDialog) private readonly dialog: MatDialog
  ) {
    this.paginationPageSize = appConfig.paginationPageSize;
    this.paginationPageSizeSelector = appConfig.paginationPageSizeSelector;
    this.gridInitialState = this.buildInitialState();
    this.initialCacheBlockSize = this.gridInitialState.pagination?.pageSize ?? this.paginationPageSize;
  }

  protected onPaginationChanged(event: PaginationChangedEvent): void {
    if (event.newPageSize) {
      (this.gridApi as any).setGridOption('cacheBlockSize', this.gridApi.paginationGetPageSize());
    }
    if (event.newPage && this.page1HasFakeData && this.gridApi.paginationGetCurrentPage() === 0) {
      this.page1HasFakeData = false;
      this.gridApi.refreshServerSide({ purge: true });
    }
  }

  protected onGridReady(params: GridReadyEvent): void {
    this.gridApi = params.api;
    const savedPage = this._avRoute.snapshot.queryParams['pageNumber'];
    if (savedPage && +savedPage > 1) {
      this.pendingPage = +savedPage;
    }
    this.gridApi.setGridOption('serverSideDatasource', this.createDatasource());
  }

  private createDatasource(): IServerSideDatasource {
    return {
      getRows: (params: IServerSideGetRowsParams) => {
        const { startRow, sortModel, filterModel } = params.request;
        const pageSize = this.gridApi.paginationGetPageSize();
        const pageNumber = Math.floor(startRow! / pageSize) + 1;

        // SSRM ignores initialState.pagination.page — it always starts at page 0.
        // Intercept the forced page-1 call: return a fake success so ag-grid
        // knows rows exist, then jump to the target page. One real API call total.
        if (this.pendingPage > 0 && pageNumber === 1) {
          const target = this.pendingPage - 1;
          this.pendingPage = 0;
          this.page1HasFakeData = true;
          params.success({ rowData: [], rowCount: (target + 1) * pageSize });
          this.gridApi.paginationGoToPage(target);
          return;
        }
        const sortCol = sortModel[0];
        const filterParams = this.buildFilterParams(filterModel as Record<string, any>);

        const request: StudentGridRequest = {
          ...filterParams,
          pageNumber,
          pageSize,
          sortBy: sortCol?.colId ?? null,
          sortByOperator: sortCol?.sort ?? null,
        };

        this.syncUrl(request);

        this.gridDataLoading.set(true);
        this.studentApi.getStudentGrid(request).subscribe({
          next: (result) => {
            this.rowData.set(result.items);
            this.gridDataLoading.set(false);
            params.success({ rowData: result.items, rowCount: result.totalCount });
          },
          error: () => {
            this.gridDataLoading.set(false);
            params.fail();
          },
        });
      },
    };
  }

  private buildInitialState(): GridState {
    const qp = this._avRoute.snapshot.queryParams;
    const state: GridState = { partialColumnState: true };

    const filterModel = this.buildTextFilterModel(qp);
    const dateFm = this.buildDateFilterModel(qp);
    if (dateFm) filterModel['startDate'] = dateFm;
    if (qp['isActive'] !== undefined) {
      filterModel['isActive'] = { filterType: 'set', values: [qp['isActive']] };
    }
    if (Object.keys(filterModel).length > 0) {
      state.filter = { filterModel };
    }

    if (qp['sortBy']) {
      state.sort = { sortModel: [{ colId: qp['sortBy'], sort: qp['sortByOperator'] ?? 'asc' }] };
    }

    const pageSize = qp['pageSize'] ? +qp['pageSize'] : this.paginationPageSize;
    state.pagination = { pageSize };

    return state;
  }

  private buildFilterParams(filterModel: Record<string, any>): StudentGridRequest {
    const p: StudentGridRequest = {};
    const textFields = ['firstName', 'lastName', 'email', 'phoneNumber', 'ssn'];
    for (const field of textFields) {
      const fm = filterModel[field];
      if (fm?.filterType === 'text') {
        this.applyTextFilterParams(p, field, fm);
      }
    }

    const dateFm = filterModel['startDate'];
    if (dateFm?.filterType === 'date') {
      this.applyDateFilterParams(p, dateFm);
    }

    const setFm = filterModel['isActive'];
    if (setFm?.filterType === 'set' && setFm.values?.length === 1) {
      const v = setFm.values[0];
      p.isActive = v === true || v === 'true';
    }

    return p;
  }

  private buildTextFilterModel(qp: Record<string, any>): Record<string, any> {
    const filterModel: Record<string, any> = {};
    const textFields = ['firstName', 'lastName', 'email', 'phoneNumber', 'ssn'];
    for (const field of textFields) {
      const op1      = qp[`${field}Operator1`];
      const val1     = qp[`${field}Value1`];
      const op2      = qp[`${field}Operator2`];
      const val2     = qp[`${field}Value2`];
      const concatOp = qp[`${field}FilterConcatOperator`];

      if (!op1) continue;

      const cond1 = this.buildTextCondition(op1, val1);
      if (!cond1) continue;

      if (op2 && concatOp) {
        const cond2 = this.buildTextCondition(op2, val2);
        filterModel[field] = cond2
          ? { filterType: 'text', operator: concatOp, conditions: [cond1, cond2] }
          : cond1;
      } else {
        filterModel[field] = cond1;
      }
    }
    return filterModel;
  }

  private applyTextFilterParams(p: StudentGridRequest, field: string, fm: any): void {
    if (fm.conditions) {
      const [c1, c2] = fm.conditions;
      if (c1.type === 'blank' || c1.type === 'notBlank') {
        (p as any)[`${field}Operator1`] = c1.type;
      } else if (c1.filter) {
        (p as any)[`${field}Value1`] = c1.filter;
        (p as any)[`${field}Operator1`] = c1.type;
      }
      if (c2.type === 'blank' || c2.type === 'notBlank') {
        (p as any)[`${field}Operator2`] = c2.type;
      } else if (c2.filter) {
        (p as any)[`${field}Value2`] = c2.filter;
        (p as any)[`${field}Operator2`] = c2.type;
      }
      (p as any)[`${field}FilterConcatOperator`] = fm.operator;
    } else if (fm.type === 'blank' || fm.type === 'notBlank') {
      (p as any)[`${field}Operator1`] = fm.type;
    } else if (fm.filter) {
      (p as any)[`${field}Value1`] = fm.filter;
      (p as any)[`${field}Operator1`] = fm.type;
    }
  }

  private buildTextCondition(op: string, val?: string): Record<string, any> | null {
    if (op === 'blank' || op === 'notBlank') {
      return { filterType: 'text', type: op };
    }
    return val ? { filterType: 'text', type: op, filter: val } : null;
  }

  private buildDateFilterModel(qp: Record<string, any>): Record<string, any> | null {
    const op1      = qp['startDateOperator1'];
    const val1     = qp['startDateValue1'];
    const val2     = qp['startDateValue2'];   // dateTo for inRange cond 1
    const concatOp = qp['startDateFilterConcatOperator'];
    const op2      = qp['startDateOperator2'];
    const val3     = qp['startDateValue3'];
    const val4     = qp['startDateValue4'];   // dateTo for inRange cond 2

    if (!op1) return null;

    const cond1 = this.buildDateCondition(op1, val1, val2);
    if (!cond1) return null;

    if (op2 && concatOp) {
      const cond2 = this.buildDateCondition(op2, val3, val4);
      return cond2
        ? { filterType: 'date', operator: concatOp, conditions: [cond1, cond2] }
        : cond1;
    }
    return cond1;
  }

  private buildDateCondition(op: string, dateFrom?: string, dateTo?: string): Record<string, any> | null {
    if (op === 'blank' || op === 'notBlank') {
      return { filterType: 'date', type: op, dateFrom: null, dateTo: null };
    }
    if (!dateFrom) return null;
    return { filterType: 'date', type: op, dateFrom, dateTo: dateTo ?? null };
  }

  private applyDateFilterParams(p: StudentGridRequest, fm: any): void {
    if (fm.conditions) {
      const [c1, c2] = fm.conditions;
      this.applyDateConditionToRequest(p, c1, '1', '2');
      this.applyDateConditionToRequest(p, c2, '3', '4');
      p.startDateFilterConcatOperator = fm.operator;
      p.startDateOperator2 = c2.type;
    } else {
      this.applyDateConditionToRequest(p, fm, '1', '2');
    }
  }

  private applyDateConditionToRequest(
    p: StudentGridRequest, cond: any, fromKey: string, toKey: string
  ): void {
    const opKey = fromKey === '1' ? 'startDateOperator1' : 'startDateOperator2';
    (p as any)[opKey] = cond.type;
    if (cond.type !== 'blank' && cond.type !== 'notBlank') {
      (p as any)[`startDateValue${fromKey}`] = cond.dateFrom ?? null;
      if (cond.type === 'inRange') {
        (p as any)[`startDateValue${toKey}`] = cond.dateTo ?? null;
      }
    }
  }

  private syncUrl(request: StudentGridRequest): void {
    const params: Record<string, string | null> = {
      pageNumber:          String(request.pageNumber ?? 1),
      pageSize:            String(request.pageSize ?? this.paginationPageSize),
      sortBy:              request.sortBy              ?? null,
      sortByOperator:      request.sortByOperator      ?? null,
      firstNameValue1:              request.firstNameValue1              ?? null,
      firstNameOperator1:           request.firstNameOperator1           ?? null,
      firstNameValue2:              request.firstNameValue2              ?? null,
      firstNameOperator2:           request.firstNameOperator2           ?? null,
      firstNameFilterConcatOperator: request.firstNameFilterConcatOperator ?? null,
      lastNameValue1:               request.lastNameValue1               ?? null,
      lastNameOperator1:            request.lastNameOperator1            ?? null,
      lastNameValue2:               request.lastNameValue2               ?? null,
      lastNameOperator2:            request.lastNameOperator2            ?? null,
      lastNameFilterConcatOperator:  request.lastNameFilterConcatOperator  ?? null,
      emailValue1:                  request.emailValue1                  ?? null,
      emailOperator1:               request.emailOperator1               ?? null,
      emailValue2:                  request.emailValue2                  ?? null,
      emailOperator2:               request.emailOperator2               ?? null,
      emailFilterConcatOperator:     request.emailFilterConcatOperator     ?? null,
      phoneNumberValue1:            request.phoneNumberValue1            ?? null,
      phoneNumberOperator1:         request.phoneNumberOperator1         ?? null,
      phoneNumberValue2:            request.phoneNumberValue2            ?? null,
      phoneNumberOperator2:         request.phoneNumberOperator2         ?? null,
      phoneNumberFilterConcatOperator: request.phoneNumberFilterConcatOperator ?? null,
      ssnValue1:                    request.ssnValue1                    ?? null,
      ssnOperator1:                 request.ssnOperator1                 ?? null,
      ssnValue2:                    request.ssnValue2                    ?? null,
      ssnOperator2:                 request.ssnOperator2                 ?? null,
      ssnFilterConcatOperator:       request.ssnFilterConcatOperator       ?? null,
      startDateOperator1:               request.startDateOperator1               ?? null,
      startDateValue1:                  request.startDateValue1                  ?? null,
      startDateValue2:                  request.startDateValue2                  ?? null,
      startDateFilterConcatOperator:    request.startDateFilterConcatOperator    ?? null,
      startDateOperator2:               request.startDateOperator2               ?? null,
      startDateValue3:                  request.startDateValue3                  ?? null,
      startDateValue4:                  request.startDateValue4                  ?? null,
      isActive:            request.isActive !== null && request.isActive !== undefined
                             ? String(request.isActive) : null,
    };

    this.router.navigate([], {
      relativeTo: this._avRoute,
      queryParams: params,
      queryParamsHandling: 'merge',
      replaceUrl: true,
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

  private onEdit(student: Student): void {
    this.router.navigate([`../detail/${student.id}`], {
      relativeTo: this._avRoute,
    });
  }

  protected onRemoveShown(): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        message: 'Are you sure you want to remove the shown students?',
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

  private onDeletes(students: Student[]): void {
    this.isDeleting.set(true);

    from(students)
      .pipe(
        concatMap((student) => {
          if (student.image) {
            return this.imageApi.deleteStudentProfile(student.image).pipe(
              catchError(() => of(null)),
              mergeMap(() => this.studentApi.deleteStudent(student.id))
            );
          } else {
            return this.studentApi.deleteStudent(student.id);
          }
        })
      )
      .subscribe({
        complete: () => {
          this.gridApi.refreshServerSide({ purge: true });
          this.notification.success('Student deleted successfully');
          this.isDeleting.set(false);
        },
        error: () => {
          this.notification.error('Failed to delete student');
          this.isDeleting.set(false);
        },
      });
  }

  protected onAdd(): void {
    this.router.navigate(['../detail/0'], { relativeTo: this._avRoute });
  }

  protected onReset(): void {
    this.pendingPage = 0;
    this.page1HasFakeData = false;
    const currentPageSize = this.gridApi.paginationGetPageSize();
    if (currentPageSize !== this.paginationPageSize) {
      (this.gridApi as any).setGridOption('cacheBlockSize', this.paginationPageSize);
      this.gridApi.setGridOption('paginationPageSize', this.paginationPageSize);
    }
    this.gridApi.setFilterModel(null);
    this.gridApi.applyColumnState({ defaultState: { sort: null } });
  }

  protected onRefresh(): void {
    // Purge clears all blocks (including any fake block 0), then reloads current page
    this.page1HasFakeData = false;
    this.gridApi.refreshServerSide({ purge: true });
  }
}

