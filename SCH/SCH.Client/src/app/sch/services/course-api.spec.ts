import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { APP_CONFIG } from '../../injection-tokens/app-config.token';

import { CourseApi } from './course-api';

describe('CourseApi', () => {
  let service: CourseApi;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://test-api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        CourseApi,
        { provide: APP_CONFIG, useValue: { apiUrl } },
      ],
    });
    service = TestBed.inject(CourseApi);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getCourses GETs list', () => {
    service.getCourses().subscribe();
    const req = httpMock.expectOne(`${apiUrl}/courses`);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getCourse GETs by id', () => {
    service.getCourse(4).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/courses/4`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('insertCourse POSTs course', () => {
    const body = { id: 0, name: 'Math' } as any;
    service.insertCourse(body).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/courses`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush(1);
  });

  it('updateCourse PATCHes course by id', () => {
    const body = { id: 9, name: 'Science' } as any;
    service.updateCourse(body).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/courses/9`);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual(body);
    req.flush({});
  });

  it('deleteCourse DELETEs by id', () => {
    service.deleteCourse(9).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/courses/9`);
    expect(req.request.method).toBe('DELETE');
    req.flush({});
  });
});
