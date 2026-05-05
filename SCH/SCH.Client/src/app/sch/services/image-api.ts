import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppConfig } from '../../interfaces/app-config';
import { APP_CONFIG } from '../../injection-tokens/app-config.token';

@Injectable()
export class ImageApi {
  private readonly apiUrl: string;

  constructor(
    @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
    private readonly http: HttpClient
  ) {
    this.apiUrl = this.appConfig.apiUrl;
  }

  public uploadStudentProfile(file: File): Observable<{ filename: string }> {
    const form = new FormData();
    form.append('file', file);

    return this.http.post<{ filename: string }>(
      `${this.apiUrl}/Image/uploadStudentProfile`,
      form
    );
  }

  public deleteStudentProfile(fileName: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/Image/deleteStudentProfile/${fileName}`
    );
  }
}
