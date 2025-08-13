using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TinyTuya.Core;

namespace TinyTuya
{
    /// <summary>
    /// Scanner for discovering Tuya devices on the local network
    /// </summary>
    public static class Scanner
    {
        /// <summary>
        /// Discovers devices on the local network
        /// </summary>
        /// <param name="verbose">Whether to include verbose information</param>
        /// <param name="poll">Whether to poll for device status</param>
        /// <param name="forceScan">Whether to force a scan</param>
        /// <param name="byId">Whether to return devices by ID</param>
        /// <param name="wantIds">The device IDs to filter by</param>
        /// <param name="wantIps">The IP addresses to filter by</param>
        /// <returns>The discovered devices</returns>
        public static async Task<Dictionary<string, Dictionary<string, object>>> DiscoverDevices(
            bool verbose = false,
            bool poll = true,
            bool forceScan = false,
            bool byId = false,
            List<string> wantIds = null,
            List<string> wantIps = null)
        {
            var devices = new Dictionary<string, Dictionary<string, object>>();
            var client = new UdpClient();
            var endpoint = new IPEndPoint(IPAddress.Any, 6666);

            try
            {
                client.EnableBroadcast = true;
                client.Client.ReceiveTimeout = 5000;

                // Broadcast message
                var message = Encoding.ASCII.GetBytes("{ \"gwId\": \"\", \"devId\": \"\" }");
                await client.SendAsync(message, message.Length, new IPEndPoint(IPAddress.Broadcast, 6666));

                // Receive responses
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < 5000)
                {
                    try
                    {
                        var result = await client.ReceiveAsync();
                        var response = Encoding.ASCII.GetString(result.Buffer);
                        try
                        {
                            var device = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                            if (device != null && device.ContainsKey("gwId"))
                            {
                                var deviceId = device["gwId"].ToString();
                                var ip = result.RemoteEndPoint.Address.ToString();

                                // Filter by ID or IP if requested
                                if (wantIds != null && !wantIds.Contains(deviceId))
                                {
                                    continue;
                                }
                                if (wantIps != null && !wantIps.Contains(ip))
                                {
                                    continue;
                                }

                                device["ip"] = ip;
                                if (byId)
                                {
                                    devices[deviceId] = device;
                                }
                                else
                                {
                                    devices[ip] = device;
                                }

                                if (poll)
                                {
                                    // TODO: Poll device for status
                                }
                            }
                        }
                        catch (JsonException)
                        {
                            // Ignore invalid JSON
                        }
                    }
                    catch (SocketException)
                    {
                        // Timeout, continue
                    }
                }
            }
            finally
            {
                client.Close();
            }

            return devices;
        }

        /// <summary>
        /// Finds a device by ID or IP address
        /// </summary>
        /// <param name="deviceId">The device ID</param>
        /// <param name="address">The IP address</param>
        /// <returns>The device information</returns>
        public static async Task<Dictionary<string, object>> FindDevice(string deviceId = null, string address = null)
        {
            if (string.IsNullOrEmpty(deviceId) && string.IsNullOrEmpty(address))
            {
                return new Dictionary<string, object>
                {
                    { "ip", null },
                    { "version", null },
                    { "id", null },
                    { "product_id", null },
                    { "data", new Dictionary<string, object>() }
                };
            }

            var wantIds = string.IsNullOrEmpty(deviceId) ? null : new List<string> { deviceId };
            var wantIps = string.IsNullOrEmpty(address) ? null : new List<string> { address };
            var allResults = await DiscoverDevices(false, false, false, true, wantIds, wantIps);
            Dictionary<string, object> ret = null;

            foreach (var gwId in allResults.Keys)
            {
                // Check to see if we are only looking for one device
                if (!string.IsNullOrEmpty(deviceId) && gwId != deviceId)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(address) && address != allResults[gwId]["ip"].ToString())
                {
                    continue;
                }

                // We found it!
                var result = allResults[gwId];
                var productId = !result.ContainsKey("productKey") ? "" : result["productKey"].ToString();
                ret = new Dictionary<string, object>
                {
                    { "ip", result["ip"] },
                    { "version", result["version"] },
                    { "id", gwId },
                    { "product_id", productId },
                    { "data", result }
                };
                break;
            }

            if (ret == null)
            {
                ret = new Dictionary<string, object>
                {
                    { "ip", null },
                    { "version", null },
                    { "id", null },
                    { "product_id", null },
                    { "data", new Dictionary<string, object>() }
                };
            }

            return ret;
        }
    }
}

