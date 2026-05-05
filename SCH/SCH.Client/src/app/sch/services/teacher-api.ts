import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig } from '../../interfaces/app-config';
import { APP_CONFIG } from '../../injection-tokens/app-config.token';
import { Observable } from 'rxjs';
import { Teacher } from '../interfaces/teacher';

@Injectable()
export class TeacherApi {

  private readonly apiUrl: string;

  constructor(
    @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
    private readonly http: HttpClient
  ) {
    this.apiUrl = this.appConfig.apiUrl;
  }

  public getTeachers(): Observable<Array<Teacher>> {

    return this.http.get<Array<Teacher>>(`${this.apiUrl}/teachers`);
  }

  public getTeacher(id: number): Observable<Teacher> {
    return this.http.get<Teacher>(`${this.apiUrl}/teachers/${id}`);
  }

  public insertTeacher(teacher: Teacher): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/teachers`, teacher);
  }

  public updateTeacher(teacher: Teacher): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/teachers/${teacher.id}`,
      teacher
    );
  }

  public deleteTeacher(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/teachers/${id}`);
  }


}
