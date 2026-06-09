import { Routes } from '@angular/router';
import { unsavedChangesGuard, policyGuard } from '../../guards';
import { Policy } from '../../enums/policy';

export const courseRoutes: Routes = [
  {
    path: '',
    redirectTo: 'list',
    pathMatch: 'full',
  },
  {
    path: 'list',
    loadComponent: () =>
      import('./list/pages/course-list-page/course-list-page').then(
        (m) => m.CourseListPage
      ),
    canActivate: [policyGuard],
    data: { policy: Policy.ViewCourses },
  },
  {
    path: 'detail/:id',
    loadComponent: () =>
      import('./detail/pages/course-detail-page/course-detail-page').then(
        (m) => m.CourseDetailPage
      ),
    canDeactivate: [unsavedChangesGuard],
  },
];
