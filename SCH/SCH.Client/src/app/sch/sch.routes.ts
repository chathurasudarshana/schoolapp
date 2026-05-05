import { Routes } from "@angular/router";
import { studentRoutes } from './student/student.routes';
import { StudentApi } from "./student/services/student-api";
import { ImageApi } from "./services/image-api";
import { courseRoutes } from "./course/course.routes";
import { CourseApi } from "./services/course-api";
import { teacherRoutes } from "./teacher/teacher.routes";
import { TeacherApi } from "./services/teacher-api";
import { dashboardRoutes } from "./dashboard/dashboard.routes";

export const schRoutes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  },
  {
    path: 'dashboard',
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
      }
    ],
    children: teacherRoutes
  }
];