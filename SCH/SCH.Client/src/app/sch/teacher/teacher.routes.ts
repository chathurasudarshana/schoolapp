import { Routes } from '@angular/router';
import { unsavedChangesGuard, policyGuard, teacherDetailGuard } from '../../guards';
import { Policy } from '../../enums/policy';

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
    canActivate: [policyGuard],
    data: { policy: Policy.ViewTeachers },
  },
  {
    path: 'detail/:id',
    loadComponent: () =>
      import('./detail/pages/teacher-detail-page/teacher-detail-page').then(
        (m) => m.TeacherDetailPage
      ),
    canActivate: [teacherDetailGuard],
    canDeactivate: [unsavedChangesGuard],
  },
];
