import { UserLookup } from './user-lookup';

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

    /**
     * Row version for optimistic concurrency control
     * Base64-encoded byte array from backend
     */
    rowVersion?: string;
}
