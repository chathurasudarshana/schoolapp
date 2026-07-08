import { Component, inject, OnInit, signal } from '@angular/core';
import { CourseStudentCount } from '../../../interfaces/course-student-count';
import { DashboardApi } from '../../../services/dashboard-api';


@Component({
  selector: 'sch-dashboard-page',
  imports: [],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.scss'
})
export class DashboardPage implements OnInit {
  private readonly dashboardApi = inject(DashboardApi);

  protected readonly rows = signal<CourseStudentCount[]>([]);
  protected readonly loading = signal(false);

  ngOnInit(): void {
    this.loading.set(true);
    this.dashboardApi.getCourseStudentCount().subscribe({
      next: (data) => this.rows.set(data),
      complete: () => this.loading.set(false),
    });
  }
}
