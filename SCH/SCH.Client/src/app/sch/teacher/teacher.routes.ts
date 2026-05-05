import { Routes } from '@angular/router';
import { unsavedChangesGuard } from '../../guards';

export const teacherRoutes: Routes = [
  {
    path: '',
    redirectTo: 'list',
    pathMatch: 'full',
  },
  {
    path: 'list',
    loadComponent: () =>
      import('./list/pages/teacher-list-page/teacher-list-page').then(
        (m) => m.TeacherListPage
      ),
  },
  {
    path: 'detail/:id',
    loadComponent: () =>
      import('./detail/pages/teacher-detail-page/teacher-detail-page').then(
        (m) => m.TeacherDetailPage
      ),
    canDeactivate: [unsavedChangesGuard],
  },
];
