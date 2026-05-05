import { Routes } from '@angular/router';
import { studentDetailRoutes } from './detail/student-detail.routes';

export const studentRoutes: Routes = [
  {
    path: '',
    redirectTo: 'list',
    pathMatch: 'full',
  },
  {
    path: 'list',
    loadComponent: () =>
      import('./list/pages/student-list-page/student-list-page').then(
        (m) => m.StudentListPage
      ),
  },
  {
    path: 'detail/:id',
    children: studentDetailRoutes,
  },
  {
    path: ':id/courses',
    loadComponent: () =>
      import('./courses/pages/student-course-page/student-course-page').then(
        (m) => m.StudentCoursePage
      ),
  },
];
