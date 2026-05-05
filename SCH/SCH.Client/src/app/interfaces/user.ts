/**
 * User data transfer object
 */
export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  roles: string[];
  isActive: boolean;
  createdDate: string;
  lastLoginDate?: string;
  
  /**
   * Concurrency stamp for optimistic concurrency control
   */
  concurrencyStamp?: string;
}

