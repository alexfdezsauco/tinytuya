namespace TinyTuya.Core
{
    /// <summary>
    /// Command types used in Tuya protocol
    /// </summary>
    public static class CommandTypes
    {
        /// <summary>
        /// UDP command
        /// </summary>
        public const int UDP = 0;

        /// <summary>
        /// AP configuration command
        /// </summary>
        public const int AP_CONFIG = 1;

        /// <summary>
        /// Active status command
        /// </summary>
        public const int ACTIVE = 2;

        /// <summary>
        /// Bind command
        /// </summary>
        public const int BIND = 3;

        /// <summary>
        /// Rename GW command
        /// </summary>
        public const int RENAME_GW = 4;

        /// <summary>
        /// Rename device command
        /// </summary>
        public const int RENAME_DEVICE = 5;

        /// <summary>
        /// Unbind command
        /// </summary>
        public const int UNBIND = 6;

        /// <summary>
        /// Control command
        /// </summary>
        public const int CONTROL = 7;

        /// <summary>
        /// Status command
        /// </summary>
        public const int STATUS = 8;

        /// <summary>
        /// Heart beat command
        /// </summary>
        public const int HEART_BEAT = 9;

        /// <summary>
        /// Data point query command
        /// </summary>
        public const int DP_QUERY = 10;

        /// <summary>
        /// Control new command
        /// </summary>
        public const int CONTROL_NEW = 11;

        /// <summary>
        /// Enable WiFi command
        /// </summary>
        public const int ENABLE_WIFI = 12;

        /// <summary>
        /// Data point query new command
        /// </summary>
        public const int DP_QUERY_NEW = 13;

        /// <summary>
        /// Scene execute command
        /// </summary>
        public const int SCENE_EXECUTE = 14;

        /// <summary>
        /// Update DPS command
        /// </summary>
        public const int UPDATEDPS = 16;

        /// <summary>
        /// Update DPS JSON command
        /// </summary>
        public const int UPDATEDPS_JSON = 17;

        /// <summary>
        /// Update DPS JSON new command
        /// </summary>
        public const int UPDATEDPS_JSON_NEW = 18;

        /// <summary>
        /// LAN external stream command
        /// </summary>
        public const int LAN_EXT_STREAM = 19;

        /// <summary>
        /// LAN GW active command
        /// </summary>
        public const int LAN_GW_ACTIVE = 240;

        /// <summary>
        /// LAN sub device request command
        /// </summary>
        public const int LAN_SUB_DEV_REQUEST = 241;

        /// <summary>
        /// LAN delete sub device command
        /// </summary>
        public const int LAN_DELETE_SUB_DEV = 242;

        /// <summary>
        /// LAN report sub device command
        /// </summary>
        public const int LAN_REPORT_SUB_DEV = 243;

        /// <summary>
        /// LAN scene command
        /// </summary>
        public const int LAN_SCENE = 244;

        /// <summary>
        /// LAN publish cloud command
        /// </summary>
        public const int LAN_PUBLISH_CLOUD = 245;

        /// <summary>
        /// LAN publish app command
        /// </summary>
        public const int LAN_PUBLISH_APP = 246;

        /// <summary>
        /// LAN export app command
        /// </summary>
        public const int LAN_EXPORT_APP = 247;

        /// <summary>
        /// LAN publish gateway command
        /// </summary>
        public const int LAN_PUBLISH_GATEWAY = 248;

        /// <summary>
        /// LAN export gateway command
        /// </summary>
        public const int LAN_EXPORT_GATEWAY = 249;

        /// <summary>
        /// LAN broadcast command
        /// </summary>
        public const int LAN_BROADCAST = 250;

        /// <summary>
        /// LAN multiple broadcast command
        /// </summary>
        public const int LAN_MULTIPLE_BROADCAST = 251;

        /// <summary>
        /// LAN report command
        /// </summary>
        public const int LAN_REPORT = 252;

        /// <summary>
        /// LAN time command
        /// </summary>
        public const int LAN_TIME = 253;

        /// <summary>
        /// LAN control command
        /// </summary>
        public const int LAN_CONTROL = 254;

        /// <summary>
        /// LAN query command
        /// </summary>
        public const int LAN_QUERY = 255;
    }
}

