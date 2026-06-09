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

  /**
   * Permission claim values embedded in JWT
   */
  permissions: string[];

  /**
   * ID of the Student record linked to this user (if any)
   */
  ownStudentId?: number;

  /**
   * ID of the Teacher record linked to this user (if any)
   */
  ownTeacherId?: number;
}

