import { Inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Student } from '../../../sch/interfaces/student';
import { AppConfig } from '../../../interfaces/app-config';
import { APP_CONFIG } from '../../../injection-tokens/app-config.token';
import { StudentCourseMap } from '../../../sch/interfaces/student-course-map';
import { PagedResult } from '../../../interfaces/paged-result';
import { StudentGridRequest } from '../interfaces/student-grid-request';

@Injectable()
export class StudentApi {
  private readonly apiUrl: string;

  constructor(
    @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
    private readonly http: HttpClient
  ) {
    this.apiUrl = this.appConfig.apiUrl;
  }

  public getStudents(isActive: boolean | null): Observable<Array<Student>> {
    let params = new HttpParams();

    if (isActive !== null) {
      params = params.set('isActive', isActive);
    }

    return this.http.get<Array<Student>>(`${this.apiUrl}/students`, { params });
  }

  public getStudent(id: number): Observable<Student> {
    return this.http.get<Student>(`${this.apiUrl}/students/${id}`);
  }

  public insertStudent(student: Student): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/students`, student);
  }

  public updateStudent(student: Student): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/students/${student.id}`,
      student
    );
  }

  public deleteStudent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/students/${id}`);
  }

  public getCourses(id: number): Observable<Array<StudentCourseMap>> {
    return this.http.get<Array<StudentCourseMap>>(
      `${this.apiUrl}/students/${id}/courses`
    );
  }

  public insertCourse(
    id: number,
    courseId: number,
    course: StudentCourseMap
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/students/${id}/courses/${courseId}`,
      course
    );
  }

  public deleteCourse(id: number, courseId: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/students/${id}/courses/${courseId}`
    );
  }

  public getStudentGrid(request: StudentGridRequest): Observable<PagedResult<Student>> {
    let params = new HttpParams();

    const entries: [string, string | boolean | number | null | undefined][] = [
      ['pageNumber',                    request.pageNumber],
      ['pageSize',                      request.pageSize],
      ['sortBy',                        request.sortBy],
      ['sortByOperator',                request.sortByOperator],
      ['firstNameValue1',               request.firstNameValue1],
      ['firstNameOperator1',            request.firstNameOperator1],
      ['firstNameValue2',               request.firstNameValue2],
      ['firstNameOperator2',            request.firstNameOperator2],
      ['firstNameFilterConcatOperator', request.firstNameFilterConcatOperator],
      ['lastNameValue1',                request.lastNameValue1],
      ['lastNameOperator1',             request.lastNameOperator1],
      ['lastNameValue2',                request.lastNameValue2],
      ['lastNameOperator2',             request.lastNameOperator2],
      ['lastNameFilterConcatOperator',  request.lastNameFilterConcatOperator],
      ['emailValue1',                   request.emailValue1],
      ['emailOperator1',                request.emailOperator1],
      ['emailValue2',                   request.emailValue2],
      ['emailOperator2',                request.emailOperator2],
      ['emailFilterConcatOperator',     request.emailFilterConcatOperator],
      ['phoneNumberValue1',             request.phoneNumberValue1],
      ['phoneNumberOperator1',          request.phoneNumberOperator1],
      ['phoneNumberValue2',             request.phoneNumberValue2],
      ['phoneNumberOperator2',          request.phoneNumberOperator2],
      ['phoneNumberFilterConcatOperator', request.phoneNumberFilterConcatOperator],
      ['ssnValue1',                     request.ssnValue1],
      ['ssnOperator1',                  request.ssnOperator1],
      ['ssnValue2',                     request.ssnValue2],
      ['ssnOperator2',                  request.ssnOperator2],
      ['ssnFilterConcatOperator',       request.ssnFilterConcatOperator],
      ['startDateOperator1',            request.startDateOperator1],
      ['startDateValue1',               request.startDateValue1],
      ['startDateValue2',               request.startDateValue2],
      ['startDateFilterConcatOperator', request.startDateFilterConcatOperator],
      ['startDateOperator2',            request.startDateOperator2],
      ['startDateValue3',               request.startDateValue3],
      ['startDateValue4',               request.startDateValue4],
      ['isActive',                      request.isActive],
    ];

    for (const [key, value] of entries) {
      if (value != null) {
        params = params.set(key, String(value));
      }
    }

    return this.http.get<PagedResult<Student>>(`${this.apiUrl}/students/grid`, { params });
  }
}
