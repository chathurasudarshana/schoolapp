import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig } from '../../interfaces/app-config';
import { APP_CONFIG } from '../../injection-tokens/app-config.token';
import { Observable } from 'rxjs';
import { Course } from '../interfaces/course';

@Injectable()
export class CourseApi {

  private readonly apiUrl: string;

  constructor(
    @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
    private readonly http: HttpClient
  ) {
    this.apiUrl = this.appConfig.apiUrl;
  }

  public getCourses(): Observable<Array<Course>> {

    return this.http.get<Array<Course>>(`${this.apiUrl}/courses`);
  }

  public getCourse(id: number): Observable<Course> {
    return this.http.get<Course>(`${this.apiUrl}/courses/${id}`);
  }

  public insertCourse(course: Course): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/courses`, course);
  }

  public updateCourse(course: Course): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/courses/${course.id}`,
      course
    );
  }

  public deleteCourse(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/courses/${id}`);
  }


}
