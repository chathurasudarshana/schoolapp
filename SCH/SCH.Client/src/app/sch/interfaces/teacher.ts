import { UserLookup } from './user-lookup';

export interface Teacher {
    id: number;
    name: string;
    userId?: number | null;
    user?: UserLookup | null;

    /**
     * Row version for optimistic concurrency control
     * Base64-encoded byte array from backend
     */
    rowVersion?: string;
}
