import { Routes } from '@angular/router';
import { unsavedChangesGuard, studentDetailGuard } from '../../../guards';

export const studentDetailRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/student-detail-page/student-detail-page').then(
        (m) => m.StudentDetailPage
      ),
    canActivate: [studentDetailGuard],
    canDeactivate: [unsavedChangesGuard],     
    children: [
      {
        path: 'courses',
        loadComponent: () =>
          import('./courses/pages/student-course-page/student-course-page').then(
            (m) => m.StudentCoursePage
          ),
      },
    ]
  },
];
