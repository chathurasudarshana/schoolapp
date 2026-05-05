import { User } from './user';

/**
 * Response model for successful login or registration
 */
export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  refreshTokenExpiresIn: number;
  tokenType: string;
  user: User;
}

