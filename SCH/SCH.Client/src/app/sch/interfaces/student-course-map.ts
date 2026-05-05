export interface StudentCourseMap {
    studentId: number;
    courseId: number;
    enrollmentDate: Date;
    studentFirstName: string | null;
    studentLastName: string | null;
    courseName: string | null;
}
