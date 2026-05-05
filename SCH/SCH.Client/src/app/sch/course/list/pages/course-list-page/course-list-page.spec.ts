import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChangeDetectorRef } from '@angular/core';
import { of, throwError } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { Router, ActivatedRoute } from '@angular/router';
import { CellClickedEvent, ColDef, GridReadyEvent } from 'ag-grid-community';

import { CourseListPage } from './course-list-page';
import { CourseApi } from '../../../../../sch/services/course-api';
import { Notification } from '../../../../../services/notification';

describe('CourseListPage', () => {
  let component: CourseListPage;
  let fixture: ComponentFixture<CourseListPage>;

  let courseApiMock: jasmine.SpyObj<CourseApi>;
  let notificationMock: jasmine.SpyObj<Notification>;
  let dialogMock: jasmine.SpyObj<MatDialog>;
  let routerMock: jasmine.SpyObj<Router>;
  let cdrMock: jasmine.SpyObj<ChangeDetectorRef>;

  beforeEach(async () => {
    courseApiMock = jasmine.createSpyObj('CourseApi', [
      'getCourses',
      'deleteCourse',
    ]);
    notificationMock = jasmine.createSpyObj('Notification', [
      'success',
      'error',
    ]);
    dialogMock = jasmine.createSpyObj('MatDialog', ['open']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    cdrMock = jasmine.createSpyObj('ChangeDetectorRef', ['markForCheck']);

    await TestBed.configureTestingModule({
      imports: [CourseListPage],
      providers: [
        { provide: CourseApi, useValue: courseApiMock },
        { provide: Notification, useValue: notificationMock },
        { provide: MatDialog, useValue: dialogMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: {} },
        // ChangeDetectorRef is resolved from the component's element injector,
        // so providing it here does not affect the instance on the component.
      ],
    }).compileComponents();

    // Avoid loading external template/styles
    TestBed.overrideComponent(CourseListPage, { set: { template: '' } });

    fixture = TestBed.createComponent(CourseListPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('loads grid data on grid ready and updates state', () => {
    const courses = [{ id: 1, name: 'Math' } as any];
    courseApiMock.getCourses.and.returnValue(of(courses));

    component['rowData'] = [];
    const markSpy = spyOn((component as any).cdr, 'markForCheck');
    (component as any).onGridReady({} as GridReadyEvent);

    expect(courseApiMock.getCourses).toHaveBeenCalled();
    expect((component as any).rowData).toEqual(courses);
    expect((component as any).gridDataLoading).toBeFalse();
    expect(markSpy).toHaveBeenCalled();
  });

  it('sets empty rowData when API returns empty', () => {
    courseApiMock.getCourses.and.returnValue(of([]));

    (component as any).onGridReady({} as GridReadyEvent);

    expect((component as any).rowData).toEqual([]);
    expect((component as any).gridDataLoading).toBeFalse();
  });

  it('navigates to edit on edit action click', () => {
    const event = {
      colDef: { headerName: 'Actions' } as ColDef,
      data: { id: 123 },
      event: { target: { dataset: { action: 'edit' } } },
    } as unknown as CellClickedEvent;

    (component as any).onCellClicked(event);

    expect(routerMock.navigate).toHaveBeenCalledWith(['../detail/123'], {
      relativeTo: TestBed.inject(ActivatedRoute),
    });
  });

  it('calls onDeletes on delete action click', () => {
    const onDeletesSpy = spyOn<any>(component, 'onDeletes');
    const data = { id: 3 };
    const event = {
      colDef: { headerName: 'Actions' } as ColDef,
      data,
      event: { target: { dataset: { action: 'delete' } } },
    } as unknown as CellClickedEvent;

    (component as any).onCellClicked(event);

    expect(onDeletesSpy).toHaveBeenCalledWith([data]);
  });

  it('onRemoveAll deletes after confirmation', () => {
    const onDeletesSpy = spyOn<any>(component, 'onDeletes');
    (component as any).rowData = [{ id: 1 } as any, { id: 2 } as any];
    dialogMock.open.and.returnValue({ afterClosed: () => of(true) } as any);

    (component as any).onRemoveAll();

    expect(dialogMock.open).toHaveBeenCalled();
    expect(onDeletesSpy).toHaveBeenCalledWith((component as any).rowData);
  });

  it('onRemoveAll does nothing when cancelled', () => {
    const onDeletesSpy = spyOn<any>(component, 'onDeletes');
    dialogMock.open.and.returnValue({ afterClosed: () => of(false) } as any);

    (component as any).onRemoveAll();

    expect(onDeletesSpy).not.toHaveBeenCalled();
  });

  it('onAdd navigates to create page', () => {
    (component as any).onAdd();
    expect(routerMock.navigate).toHaveBeenCalledWith(['../detail/0'], {
      relativeTo: TestBed.inject(ActivatedRoute),
    });
  });

  it('onDeletes deletes courses and shows success', () => {
    spyOn<any>(component, 'setGridData');
    courseApiMock.deleteCourse.and.returnValue(of(void 0));
    const toDelete = [{ id: 10 } as any, { id: 11 } as any];
    const markSpy = spyOn((component as any).cdr, 'markForCheck');

    (component as any).onDeletes(toDelete);

    expect((component as any).isDeleting).toBeFalse();
    expect(courseApiMock.deleteCourse).toHaveBeenCalledTimes(2);
    expect(notificationMock.success).toHaveBeenCalled();
    expect(component['setGridData']).toHaveBeenCalled();
    expect(markSpy).toHaveBeenCalled();
  });

  it('onDeletes handles error and shows error notification', () => {
    spyOn<any>(component, 'setGridData');
    courseApiMock.deleteCourse.and.callFake((id: number) => {
      if (id === 99) {
        return throwError(() => new Error('fail'));
      }
      return of(void 0);
    });

    const toDelete = [{ id: 98 } as any, { id: 99 } as any];

    (component as any).onDeletes(toDelete);

    expect((component as any).isDeleting).toBeFalse();
    expect(notificationMock.error).toHaveBeenCalled();
  });
});
