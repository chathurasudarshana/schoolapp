/**
 * Request model for user login
 */
export interface LoginRequest {
  username: string;
  password: string;
  rememberMe?: boolean;
}

