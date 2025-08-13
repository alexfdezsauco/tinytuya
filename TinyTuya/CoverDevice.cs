using System.Collections.Generic;
using TinyTuya.Core;

namespace TinyTuya
{
    /// <summary>
    /// Represents a Tuya smart cover
    /// </summary>
    public class CoverDevice : Device
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoverDevice"/> class
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <param name="address">The device address</param>
        /// <param name="localKey">The local key</param>
        /// <param name="deviceType">The device type</param>
        /// <param name="connectionTimeout">The connection timeout</param>
        /// <param name="version">The protocol version</param>
        /// <param name="persist">Whether to keep the socket connection persistent</param>
        /// <param name="childId">The child ID (for sub-devices)</param>
        /// <param name="parent">The parent device</param>
        /// <param name="connectionRetryLimit">The connection retry limit</param>
        /// <param name="connectionRetryDelay">The connection retry delay</param>
        /// <param name="port">The port</param>
        /// <param name="maxSimultaneousDps">The maximum number of simultaneous data points</param>
        public CoverDevice(
            string deviceId,
            string address = null,
            string localKey = "",
            string deviceType = "default",
            int connectionTimeout = 5,
            double version = 3.1,
            bool persist = false,
            string childId = null,
            XenonDevice parent = null,
            int connectionRetryLimit = 5,
            int connectionRetryDelay = 5,
            int port = Constants.TCPPORT,
            int maxSimultaneousDps = 0)
            : base(
                deviceId,
                address,
                localKey,
                deviceType,
                connectionTimeout,
                version,
                persist,
                childId,
                parent,
                connectionRetryLimit,
                connectionRetryDelay,
                port,
                maxSimultaneousDps)
        {
        }

        /// <summary>
        /// Opens the cover
        /// </summary>
        /// <param name="switchIndex">The switch index</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> OpenCover(string switchIndex = "1")
        {
            return SetValue(switchIndex, "open");
        }

        /// <summary>
        /// Closes the cover
        /// </summary>
        /// <param name="switchIndex">The switch index</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> CloseCover(string switchIndex = "1")
        {
            return SetValue(switchIndex, "close");
        }

        /// <summary>
        /// Stops the cover
        /// </summary>
        /// <param name="switchIndex">The switch index</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> StopCover(string switchIndex = "1")
        {
            return SetValue(switchIndex, "stop");
        }
    }
}

