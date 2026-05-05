import { Inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Student } from '../../../sch/interfaces/student';
import { AppConfig } from '../../../interfaces/app-config';
import { APP_CONFIG } from '../../../injection-tokens/app-config.token';
import { StudentCourseMap } from '../../../sch/interfaces/student-course-map';

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
}
