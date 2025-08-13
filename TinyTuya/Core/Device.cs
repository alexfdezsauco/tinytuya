using System;
using System.Collections.Generic;
using System.Threading;

namespace TinyTuya.Core
{
    /// <summary>
    /// Represents a Tuya device
    /// </summary>
    public class Device : XenonDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Device"/> class
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
        public Device(
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
        /// Sets the status of the device
        /// </summary>
        /// <param name="on">Whether to turn the device on or off</param>
        /// <param name="switchIndex">The switch index</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetStatus(bool on, string switchIndex = "1", bool nowait = false)
        {
            if (int.TryParse(switchIndex, out int switchInt))
            {
                switchIndex = switchInt.ToString();
            }

            var payload = GeneratePayload(CommandTypes.CONTROL, new Dictionary<string, object> { { switchIndex, on } });
            return SendReceive(payload, getResponse: !nowait);
        }

        /// <summary>
        /// Gets the product information
        /// </summary>
        /// <returns>The product information</returns>
        public Dictionary<string, object> Product()
        {
            var payload = GeneratePayload(CommandTypes.AP_CONFIG);
            return SendReceive(payload, 0);
        }

        /// <summary>
        /// Sends a heartbeat to the device
        /// </summary>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> Heartbeat(bool nowait = true)
        {
            var payload = GeneratePayload(CommandTypes.HEART_BEAT);
            return SendReceive(payload, 0, getResponse: !nowait);
        }

        /// <summary>
        /// Updates the data points
        /// </summary>
        /// <param name="index">The data points to update</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> UpdateDps(List<int> index = null, bool nowait = false)
        {
            if (index == null)
            {
                index = new List<int> { 1 };
            }

            var payload = GeneratePayload(CommandTypes.UPDATEDPS, index);
            return SendReceive(payload, 0, getResponse: !nowait);
        }

        /// <summary>
        /// Sets a value for a data point
        /// </summary>
        /// <param name="index">The data point index</param>
        /// <param name="value">The value</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetValue(string index, object value, bool nowait = false)
        {
            if (int.TryParse(index, out int indexInt))
            {
                index = indexInt.ToString();
            }

            var payload = GeneratePayload(CommandTypes.CONTROL, new Dictionary<string, object> { { index, value } });
            return SendReceive(payload, getResponse: !nowait);
        }

        /// <summary>
        /// Sets multiple values for data points
        /// </summary>
        /// <param name="data">The data points and values</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetMultipleValues(Dictionary<string, object> data, bool nowait = false)
        {
            if (nowait)
            {
                if (MaxSimultaneousDps > 0 && data.Count > MaxSimultaneousDps)
                {
                    Dictionary<string, object> ret = null;
                    foreach (var kvp in data)
                    {
                        ret = SetValue(kvp.Key, kvp.Value, nowait);
                    }
                    return ret;
                }
                else
                {
                    var outData = new Dictionary<string, object>();
                    foreach (var kvp in data)
                    {
                        outData[kvp.Key.ToString()] = kvp.Value;
                    }
                    var payload = GeneratePayload(CommandTypes.CONTROL, outData);
                    return SendReceive(payload, getResponse: !nowait);
                }
            }

            if (MaxSimultaneousDps > 0 && data.Count > MaxSimultaneousDps)
            {
                var ret = new Dictionary<string, object>();
                foreach (var kvp in data)
                {
                    if (ret.Count > 0)
                    {
                        Thread.Sleep(1000);
                    }
                    var result = SetValue(kvp.Key, kvp.Value, nowait);
                    MergeDpsResults(ret, result);
                }
                return ret;
            }

            var outData2 = new Dictionary<string, object>();
            foreach (var kvp in data)
            {
                outData2[kvp.Key.ToString()] = kvp.Value;
            }

            var payload2 = GeneratePayload(CommandTypes.CONTROL, outData2);
            var result2 = SendReceive(payload2, getResponse: !nowait);

            if (result2 != null && result2.ContainsKey("Err") && outData2.Count > 1)
            {
                var firstKey = GetFirstKey(outData2);
                var res = SetValue(firstKey, outData2[firstKey], nowait);
                outData2.Remove(firstKey);
                if (res != null && !res.ContainsKey("Err"))
                {
                    MaxSimultaneousDps = 1;
                    result2 = res;
                    foreach (var kvp in outData2)
                    {
                        res = SetValue(kvp.Key, kvp.Value, nowait);
                        MergeDpsResults(result2, res);
                    }
                }
            }
            return result2;
        }

        /// <summary>
        /// Gets the first key in a dictionary
        /// </summary>
        /// <param name="dict">The dictionary</param>
        /// <returns>The first key</returns>
        private string GetFirstKey(Dictionary<string, object> dict)
        {
            foreach (var key in dict.Keys)
            {
                return key;
            }
            return null;
        }

        /// <summary>
        /// Merges DPS results
        /// </summary>
        /// <param name="dest">The destination</param>
        /// <param name="src">The source</param>
        public static void MergeDpsResults(Dictionary<string, object> dest, Dictionary<string, object> src)
        {
            if (src != null && !src.ContainsKey("Error") && !src.ContainsKey("Err"))
            {
                foreach (var kvp in src)
                {
                    if (kvp.Key == "dps" && kvp.Value is Dictionary<string, object> dps)
                    {
                        if (!dest.ContainsKey("dps") || !(dest["dps"] is Dictionary<string, object>))
                        {
                            dest["dps"] = new Dictionary<string, object>();
                        }
                        foreach (var dpsKvp in dps)
                        {
                            ((Dictionary<string, object>)dest["dps"])[dpsKvp.Key] = dpsKvp.Value;
                        }
                    }
                    else if (kvp.Key == "data" && kvp.Value is Dictionary<string, object> data && data.ContainsKey("dps") && data["dps"] is Dictionary<string, object> dataDps)
                    {
                        if (!dest.ContainsKey(kvp.Key) || !(dest[kvp.Key] is Dictionary<string, object>))
                        {
                            dest[kvp.Key] = new Dictionary<string, object> { { "dps", new Dictionary<string, object>() } };
                        }
                        if (!((Dictionary<string, object>)dest[kvp.Key]).ContainsKey("dps") || !(((Dictionary<string, object>)dest[kvp.Key])["dps"] is Dictionary<string, object>))
                        {
                            ((Dictionary<string, object>)dest[kvp.Key])["dps"] = new Dictionary<string, object>();
                        }
                        foreach (var dpsKvp in dataDps)
                        {
                            ((Dictionary<string, object>)((Dictionary<string, object>)dest[kvp.Key])["dps"])[dpsKvp.Key] = dpsKvp.Value;
                        }
                    }
                    else
                    {
                        dest[kvp.Key] = kvp.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Turns the device on
        /// </summary>
        /// <param name="switchIndex">The switch index</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> TurnOn(string switchIndex = "1", bool nowait = false)
        {
            return SetStatus(true, switchIndex, nowait);
        }

        /// <summary>
        /// Turns the device off
        /// </summary>
        /// <param name="switchIndex">The switch index</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> TurnOff(string switchIndex = "1", bool nowait = false)
        {
            return SetStatus(false, switchIndex, nowait);
        }

        /// <summary>
        /// Sets a timer
        /// </summary>
        /// <param name="numSecs">The number of seconds</param>
        /// <param name="dpsId">The data point ID</param>
        /// <param name="nowait">Whether to wait for a response</param>
        /// <returns>The response</returns>
        public Dictionary<string, object> SetTimer(int numSecs, string dpsId = "0", bool nowait = false)
        {
            if (dpsId == "0")
            {
                var status = Status();
                if (status != null && status.ContainsKey("dps") && status["dps"] is Dictionary<string, object> dps)
                {
                    var keys = new List<string>(dps.Keys);
                    keys.Sort();
                    dpsId = keys[keys.Count - 1];
                }
                else
                {
                    return status;
                }
            }

            var payload = GeneratePayload(CommandTypes.CONTROL, new Dictionary<string, object> { { dpsId, numSecs } });
            return SendReceive(payload, getResponse: !nowait);
        }
    }
}

