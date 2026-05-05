namespace SCH.Models.Auth.ClientDtos
{
    /// <summary>
    /// Active session information
    /// </summary>
    public class SessionDto
    {
        /// <summary>
        /// Session ID (RefreshToken ID)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Device name (e.g., "Chrome on Windows")
        /// </summary>
        public string? DeviceName { get; set; }

        /// <summary>
        /// IP address
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Location (optional, based on GeoIP lookup)
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Last active date/time
        /// </summary>
        public DateTime LastActive { get; set; }

        /// <summary>
        /// Indicates if this is the current session
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Token expiry date
        /// </summary>
        public DateTime ExpiryDate { get; set; }
    }
}

