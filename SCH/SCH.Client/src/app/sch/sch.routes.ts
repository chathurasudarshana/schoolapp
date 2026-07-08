import { Routes } from "@angular/router";
import { studentRoutes } from './student/student.routes';
import { StudentApi } from "./student/services/student-api";
import { ImageApi } from "./services/image-api";
import { courseRoutes } from "./course/course.routes";
import { CourseApi } from "./services/course-api";
import { teacherRoutes } from "./teacher/teacher.routes";
import { TeacherApi } from "./services/teacher-api";
import { DashboardApi } from './services/dashboard-api';
import { dashboardRoutes } from "./dashboard/dashboard.routes";
import { IdentityUserApi } from "./services/identity-user-api";

export const schRoutes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  },
  {
    path: 'dashboard',
    providers: [
      {
        provide: DashboardApi,
        useClass: DashboardApi
      }
    ],
    children: dashboardRoutes
  },
  {
    path: 'student',
    providers: [
      {
        provide: StudentApi,
        useClass: StudentApi
      },
      {
        provide: ImageApi,
        useClass: ImageApi
      },
      {
        provide: CourseApi,
        useClass: CourseApi
      },
      {
        provide: IdentityUserApi,
        useClass: IdentityUserApi
      }
    ],
    children: studentRoutes
  },
  {
    path: 'course',
    providers: [
      {
        provide: CourseApi,
        useClass: CourseApi
      },
      {
        provide: ImageApi,
        useClass: ImageApi
      }
    ],
    children: courseRoutes
  },
  {
    path: 'teacher',
    providers: [
      {
        provide: TeacherApi,
        useClass: TeacherApi
      },
      {
        provide: ImageApi,
        useClass: ImageApi
      },
      {
        provide: IdentityUserApi,
        useClass: IdentityUserApi
      }
    ],
    children: teacherRoutes
  }
];