/**
 * Defines the scope of logout operation
 * Matches backend LogoutScope enum
 */
export enum LogoutScope {
  /**
   * Revoke only the current refresh token (most granular)
   */
  CurrentSession = 'CurrentSession',

  /**
   * Revoke all tokens in the same family (same login session/browser)
   */
  CurrentBrowser = 'CurrentBrowser',

  /**
   * Revoke all tokens for the user across all devices
   */
  AllDevices = 'AllDevices'
}

