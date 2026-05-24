import { Component, Inject, signal } from '@angular/core';
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

ModuleRegistry.registerModules([AllCommunityModule, ServerSideRowModelModule, ServerSideRowModelApiModule, SetFilterModule]);

@Component({
  selector: 'sch-student-list-page',
  imports: [AgGridAngular],
  templateUrl: './student-list-page.html',
  styleUrl: './student-list-page.scss',
})
export class StudentListPage {
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
        return `
      <button type="button" class="edit-btn" data-action="edit">Edit</button>
      <button type="button" class="delete-btn" data-action="delete">Delete</button>
    `;
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

    const filterModel: Record<string, any> = {};
    const textFields = ['firstName', 'lastName', 'email', 'phoneNumber', 'ssn'];
    for (const field of textFields) {
      if (qp[field]) {
        filterModel[field] = {
          filterType: 'text',
          type: this.spOperatorToAgType(qp[`${field}Operator`] ?? 'eq'),
          filter: qp[field],
        };
      }
    }
    if (qp['startDate']) {
      filterModel['startDate'] = {
        filterType: 'date',
        type: this.spDateOperatorToAgType(qp['startDateOperator'] ?? 'eq'),
        dateFrom: qp['startDate'],
        dateTo: null,
      };
    }
    if (qp['isActive'] !== undefined) {
      filterModel['isActive'] = { filterType: 'set', values: [qp['isActive'] === 'true'] };
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

    const textFields: (keyof StudentGridRequest)[] = [
      'firstName', 'lastName', 'email', 'phoneNumber', 'ssn',
    ];
    for (const field of textFields) {
      const fm = filterModel[field as string];
      if (fm?.filterType === 'text' && fm.filter) {
        (p as any)[field] = fm.filter;
        (p as any)[`${field}Operator`] = this.agTypeToSpOperator(fm.type);
      }
    }

    const dateFm = filterModel['startDate'];
    if (dateFm?.filterType === 'date' && dateFm.dateFrom) {
      p.startDate = dateFm.dateFrom;
      p.startDateOperator = this.agDateTypeToSpOperator(dateFm.type);
    }

    const setFm = filterModel['isActive'];
    if (setFm?.filterType === 'set' && setFm.values?.length === 1) {
      p.isActive = setFm.values[0] === true;
    }

    return p;
  }

  private agTypeToSpOperator(type: string): string {
    const map: Record<string, string> = {
      equals: 'eq', notEqual: 'ne', contains: 'contains',
      startsWith: 'startswith', endsWith: 'endswith',
    };
    return map[type] ?? 'eq';
  }

  private agDateTypeToSpOperator(type: string): string {
    const map: Record<string, string> = {
      equals: 'eq', notEqual: 'ne',
      greaterThan: 'gt', greaterThanOrEqual: 'gte',
      lessThan: 'lt', lessThanOrEqual: 'lte',
    };
    return map[type] ?? 'eq';
  }

  private spOperatorToAgType(op: string): string {
    const map: Record<string, string> = {
      eq: 'equals', ne: 'notEqual', contains: 'contains',
      startswith: 'startsWith', endswith: 'endsWith',
    };
    return map[op] ?? 'equals';
  }

  private spDateOperatorToAgType(op: string): string {
    const map: Record<string, string> = {
      eq: 'equals', ne: 'notEqual',
      gt: 'greaterThan', gte: 'greaterThanOrEqual',
      lt: 'lessThan', lte: 'lessThanOrEqual',
    };
    return map[op] ?? 'equals';
  }

  private syncUrl(request: StudentGridRequest): void {
    const params: Record<string, string | null> = {
      pageNumber:          String(request.pageNumber ?? 1),
      pageSize:            String(request.pageSize ?? this.paginationPageSize),
      sortBy:              request.sortBy              ?? null,
      sortByOperator:      request.sortByOperator      ?? null,
      firstName:           request.firstName           ?? null,
      firstNameOperator:   request.firstNameOperator   ?? null,
      lastName:            request.lastName            ?? null,
      lastNameOperator:    request.lastNameOperator    ?? null,
      email:               request.email               ?? null,
      emailOperator:       request.emailOperator       ?? null,
      phoneNumber:         request.phoneNumber         ?? null,
      phoneNumberOperator: request.phoneNumberOperator ?? null,
      ssn:                 request.ssn                 ?? null,
      ssnOperator:         request.ssnOperator         ?? null,
      startDate:           request.startDate           ?? null,
      startDateOperator:   request.startDateOperator   ?? null,
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

