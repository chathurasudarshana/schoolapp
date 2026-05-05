namespace SCH.Models.Auth.Enums
{
    /// <summary>
    /// Defines the scope of logout operation
    /// </summary>
    public enum LogoutScope
    {
        /// <summary>
        /// Revoke only the current refresh token (most granular)
        /// </summary>
        CurrentSession = 1,

        /// <summary>
        /// Revoke all tokens in the same family (same login session/browser)
        /// </summary>
        CurrentBrowser = 2,

        /// <summary>
        /// Revoke all tokens for the user across all devices
        /// </summary>
        AllDevices = 3
    }
}

