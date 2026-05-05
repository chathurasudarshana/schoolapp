export interface Teacher {
    id: number;
    name: string;
    
    /**
     * Row version for optimistic concurrency control
     * Base64-encoded byte array from backend
     */
    rowVersion?: string;
}
