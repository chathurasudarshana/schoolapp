import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { APP_CONFIG } from '../../injection-tokens/app-config.token';

import { TeacherApi } from './teacher-api';

describe('TeacherApi', () => {
  let service: TeacherApi;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://test-api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        TeacherApi,
        { provide: APP_CONFIG, useValue: { apiUrl } },
      ],
    });
    service = TestBed.inject(TeacherApi);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getTeachers GETs list', () => {
    service.getTeachers().subscribe();
    const req = httpMock.expectOne(`${apiUrl}/teachers`);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getTeacher GETs by id', () => {
    service.getTeacher(4).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/teachers/4`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('insertTeacher POSTs teacher', () => {
    const body = { id: 0, name: 'Alice' } as any;
    service.insertTeacher(body).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/teachers`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush(1);
  });

  it('updateTeacher PATCHes teacher by id', () => {
    const body = { id: 9, name: 'Bob' } as any;
    service.updateTeacher(body).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/teachers/9`);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual(body);
    req.flush({});
  });

  it('deleteTeacher DELETEs by id', () => {
    service.deleteTeacher(9).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/teachers/9`);
    expect(req.request.method).toBe('DELETE');
    req.flush({});
  });
});


