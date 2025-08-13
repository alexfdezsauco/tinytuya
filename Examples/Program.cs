using System;
using System.Threading.Tasks;
using TinyTuya;

namespace TinyTuya.Examples
{
    /// <summary>
    /// Example program for TinyTuya
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static async Task Main(string[] args)
        {
            Console.WriteLine("TinyTuya Example");
            Console.WriteLine("---------------");

            // Discover devices
            Console.WriteLine("Discovering devices...");
            var devices = await Scanner.DiscoverDevices();
            Console.WriteLine($"Found {devices.Count} devices:");
            foreach (var device in devices)
            {
                Console.WriteLine($"  {device.Key}: {device.Value["gwId"]} (version {device.Value["version"]})");
            }

            // Connect to a device
            if (devices.Count > 0)
            {
                var deviceIp = devices.Keys[0];
                var deviceId = devices[deviceIp]["gwId"].ToString();
                var deviceVersion = Convert.ToDouble(devices[deviceIp]["version"]);

                Console.WriteLine($"Connecting to device {deviceId} at {deviceIp}...");
                
                // You would need to provide the local key for the device
                var localKey = "YOUR_LOCAL_KEY";
                
                // Create a device instance
                var device = new Device(deviceId, deviceIp, localKey, "default", 5, deviceVersion);
                
                // Get device status
                var status = device.Status();
                Console.WriteLine($"Device status: {(status != null ? "Online" : "Offline")}");
                
                if (status != null)
                {
                    Console.WriteLine("Device data points:");
                    if (status.ContainsKey("dps") && status["dps"] is Dictionary<string, object> dps)
                    {
                        foreach (var dp in dps)
                        {
                            Console.WriteLine($"  {dp.Key}: {dp.Value}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No devices found.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}

