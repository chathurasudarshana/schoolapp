import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { AppConfig } from '../../interfaces/app-config';
import { APP_CONFIG } from '../../injection-tokens/app-config.token';
import { Observable } from 'rxjs';
import { CourseStudentCount } from '../interfaces/course-student-count';

@Injectable()
export class DashboardApi {
    private readonly apiUrl: string;

    constructor(
        @Inject(APP_CONFIG) private readonly appConfig: AppConfig,
        private readonly http: HttpClient
    ) {
        this.apiUrl = this.appConfig.apiUrl;
    }

    public getCourseStudentCount(): Observable<Array<CourseStudentCount>> {
        return this.http.get<Array<CourseStudentCount>>(
            `${this.apiUrl}/dashboard/course-student-count`
        );
    }
}
