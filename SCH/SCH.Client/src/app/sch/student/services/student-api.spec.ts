import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { APP_CONFIG } from '../../../injection-tokens/app-config.token';

import { StudentApi } from './student-api';

describe('StudentApi', () => {
  let service: StudentApi;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://test-api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        StudentApi,
        { provide: APP_CONFIG, useValue: { apiUrl } },
      ],
    });
    service = TestBed.inject(StudentApi);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getStudents adds isActive param when not null', () => {
    service.getStudents(true).subscribe();
    const reqTrue = httpMock.expectOne((req) => req.method === 'GET' && req.url === `${apiUrl}/students`);
    expect(reqTrue.request.params.get('isActive')).toBe('true');
    reqTrue.flush([]);

    service.getStudents(false).subscribe();
    const reqFalse = httpMock.expectOne((req) => req.method === 'GET' && req.url === `${apiUrl}/students`);
    expect(reqFalse.request.params.get('isActive')).toBe('false');
    reqFalse.flush([]);
  });

  it('getStudents omits isActive param when null', () => {
    service.getStudents(null).subscribe();
    const req = httpMock.expectOne((r) => r.method === 'GET' && r.url === `${apiUrl}/students`);
    expect(req.request.params.has('isActive')).toBeFalse();
    req.flush([]);
  });

  it('getStudent GETs by id', () => {
    service.getStudent(7).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/students/7`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('insertStudent POSTs student', () => {
    const body = { id: 0, firstName: 'A' } as any;
    service.insertStudent(body).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/students`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush(1);
  });

  it('updateStudent PATCHes student by id', () => {
    const body = { id: 5, firstName: 'B' } as any;
    service.updateStudent(body).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/students/5`);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual(body);
    req.flush({});
  });

  it('deleteStudent DELETEs by id', () => {
    service.deleteStudent(9).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/students/9`);
    expect(req.request.method).toBe('DELETE');
    req.flush({});
  });

  it('getCourses GETs student courses', () => {
    service.getCourses(3).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/students/3/courses`);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('insertCourse PUTs course mapping', () => {
    const course = { studentId: 3, courseId: 12 } as any;
    service.insertCourse(3, 12, course).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/students/3/courses/12`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(course);
    req.flush({});
  });

  it('deleteCourse DELETEs mapping', () => {
    service.deleteCourse(3, 12).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/students/3/courses/12`);
    expect(req.request.method).toBe('DELETE');
    req.flush({});
  });
});
