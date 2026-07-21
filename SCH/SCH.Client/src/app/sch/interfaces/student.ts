import { UserLookup } from './user-lookup';
import { StudentCourseMap } from './student-course-map';

export interface Student {
    id: number;
    firstName: string;
    lastName: string | null;
    email: string | null;
    phoneNumber: string | null;
    ssn: string | null;
    image: string | null;
    startDate: Date | null;
    isActive: boolean;
    userId?: number | null;
    user?: UserLookup | null;
    courses?: StudentCourseMap[];

    /**
     * Row version for optimistic concurrency control
     * Base64-encoded byte array from backend
     */
    rowVersion?: string;
}
