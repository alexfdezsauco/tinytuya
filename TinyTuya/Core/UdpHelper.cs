using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TinyTuya.Core
{
    /// <summary>
    /// Helper class for UDP operations
    /// </summary>
    public static class UdpHelper
    {
        /// <summary>
        /// Broadcasts a UDP message to discover Tuya devices
        /// </summary>
        /// <param name="port">The port to broadcast on</param>
        /// <param name="timeout">The timeout in milliseconds</param>
        /// <returns>A list of discovered devices</returns>
        public static async Task<List<Dictionary<string, object>>> BroadcastMessage(int port = 6666, int timeout = 5000)
        {
            var devices = new List<Dictionary<string, object>>();
            var client = new UdpClient();
            var endpoint = new IPEndPoint(IPAddress.Any, port);

            try
            {
                client.EnableBroadcast = true;
                client.Client.ReceiveTimeout = timeout;

                // Broadcast message
                var message = Encoding.ASCII.GetBytes("{ \"gwId\": \"\", \"devId\": \"\" }");
                await client.SendAsync(message, message.Length, new IPEndPoint(IPAddress.Broadcast, port));

                // Receive responses
                var receiveTask = Task.Run(async () =>
                {
                    var startTime = DateTime.Now;
                    while ((DateTime.Now - startTime).TotalMilliseconds < timeout)
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
                                    device["ip"] = result.RemoteEndPoint.Address.ToString();
                                    devices.Add(device);
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
                });

                await Task.WhenAny(receiveTask, Task.Delay(timeout));
            }
            finally
            {
                client.Close();
            }

            return devices;
        }

        /// <summary>
        /// Sends a UDP message to a specific IP address
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="ipAddress">The IP address to send to</param>
        /// <param name="port">The port to send to</param>
        /// <param name="timeout">The timeout in milliseconds</param>
        /// <returns>The response as a string</returns>
        public static async Task<string> SendMessage(string message, string ipAddress, int port = 6666, int timeout = 5000)
        {
            var client = new UdpClient();
            var endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            try
            {
                client.Client.ReceiveTimeout = timeout;

                // Send message
                var data = Encoding.ASCII.GetBytes(message);
                await client.SendAsync(data, data.Length, endpoint);

                // Receive response
                var result = await client.ReceiveAsync();
                return Encoding.ASCII.GetString(result.Buffer);
            }
            finally
            {
                client.Close();
            }
        }
    }
}

