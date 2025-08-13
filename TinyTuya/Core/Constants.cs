using System;

namespace TinyTuya.Core
{
    /// <summary>
    /// Constants used throughout the TinyTuya library
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Default TCP port for Tuya devices
        /// </summary>
        public const int TCPPORT = 6668;

        /// <summary>
        /// Default timeout for socket operations in milliseconds
        /// </summary>
        public const int DEFAULT_TIMEOUT = 5000;

        /// <summary>
        /// Default device file path
        /// </summary>
        public const string DEVICEFILE = "devices.json";

        /// <summary>
        /// Default retry count for socket operations
        /// </summary>
        public const int DEFAULT_RETRY_COUNT = 5;

        /// <summary>
        /// Default retry delay for socket operations in milliseconds
        /// </summary>
        public const int DEFAULT_RETRY_DELAY = 5000;

        /// <summary>
        /// Default send wait time in milliseconds
        /// </summary>
        public const int DEFAULT_SEND_WAIT = 10;
    }

    /// <summary>
    /// Error codes used throughout the TinyTuya library
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>
        /// Error code for connection errors
        /// </summary>
        public const string ERR_CONNECT = "ERR_CONNECT";

        /// <summary>
        /// Error code for device type errors
        /// </summary>
        public const string ERR_DEVTYPE = "ERR_DEVTYPE";

        /// <summary>
        /// Error code for JSON parsing errors
        /// </summary>
        public const string ERR_JSON = "ERR_JSON";

        /// <summary>
        /// Error code for key or version errors
        /// </summary>
        public const string ERR_KEY_OR_VER = "ERR_KEY_OR_VER";

        /// <summary>
        /// Error code for offline devices
        /// </summary>
        public const string ERR_OFFLINE = "ERR_OFFLINE";

        /// <summary>
        /// Error code for payload errors
        /// </summary>
        public const string ERR_PAYLOAD = "ERR_PAYLOAD";

        /// <summary>
        /// Error code for cloud key errors
        /// </summary>
        public const string ERR_CLOUDKEY = "ERR_CLOUDKEY";
    }
}

