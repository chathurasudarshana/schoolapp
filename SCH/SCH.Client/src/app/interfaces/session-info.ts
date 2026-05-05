/**
 * Active session information
 */
export interface SessionInfo {
  id: number;
  deviceName?: string;
  ipAddress?: string;
  userAgent?: string;
  createdDate: string;
  expiresDate: string;
  isCurrentSession: boolean;
}

